using Microsoft.Extensions.DependencyInjection;

namespace BEESHOP.AUTH.APPLICATION;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services here
        // Example: services.AddScoped<IUserService, UserService>();
        return services;
    }
}