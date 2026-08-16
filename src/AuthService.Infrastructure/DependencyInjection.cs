using AuthService.Application.Abstractions.Authentication;
using AuthService.Application.Abstractions.Persistence;
using AuthService.Infrastructure.Authentication;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database")
                ?? throw new InvalidOperationException("Connection string 'Database' not found.");

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IUnitOfWork>(serviceProvider =>
                serviceProvider.GetRequiredService<AppDbContext>());

            services.Configure<PasswordHasherOptions>(options =>
            {
                options.CompatibilityMode =
                    PasswordHasherCompatibilityMode.IdentityV3;

                options.IterationCount = 600_000;
            });

            services.AddSingleton<PasswordHasher<object>>();
            services.AddSingleton<IPasswordHasher, AspNetCorePasswordHasher>();

            return services;
        }
    }
}
