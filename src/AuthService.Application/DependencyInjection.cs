using AuthService.Application.Users.Register;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddSingleton<TimeProvider>(
            TimeProvider.System);

        services.AddScoped<RegisterHandler>();

        return services;
    }
}