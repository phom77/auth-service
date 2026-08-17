using System.Net;
using System.Net.Http.Json;
using AuthService.Api.Contracts.Auth;
using AuthService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace AuthService.IntegrationTests.Auth;

public sealed class RegisterEndpointTests
    : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder()
            .WithImage("postgres:17-alpine")
            .WithDatabase("auth_service_tests")
            .WithUsername("auth_service_tests")
            .WithPassword("integration_test_password")
            .Build();

    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.UseSetting(
                    "ConnectionStrings:Database",
                    _postgres.GetConnectionString());

                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<
                        DbContextOptions<AppDbContext>>();

                    services.RemoveAll<AppDbContext>();

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(
                            _postgres.GetConnectionString()));
                });
            });

        using var scope =
            _factory.Services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        await dbContext.Database.MigrateAsync();

        _client = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();

        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task Register_WithValidRequest_CreatesUser()
    {
        var email =
            $"student-{Guid.NewGuid():N}@example.com";

        const string password =
            "a-valid-password";

        var response = await _client!.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(email, password));

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var body =
            await response.Content
                .ReadFromJsonAsync<RegisterResponse>();

        Assert.NotNull(body);
        Assert.Equal(email, body.Email);

        using var scope =
            _factory!.Services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        var user = await dbContext.Users
            .SingleAsync(item => item.Id == body.Id);

        Assert.Equal(email, user.Email);

        Assert.Equal(
            email.ToUpperInvariant(),
            user.NormalizedEmail);

        Assert.NotEqual(
            password,
            user.PasswordHash);

        var duplicateResponse =
            await _client.PostAsJsonAsync(
                "/api/auth/register",
                new RegisterRequest(
                    email.ToUpperInvariant(),
                    password));

        Assert.Equal(
            HttpStatusCode.Conflict,
            duplicateResponse.StatusCode);
    }
}