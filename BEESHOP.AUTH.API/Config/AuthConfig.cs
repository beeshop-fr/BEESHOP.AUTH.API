using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BEESHOP.AUTH.API.Config;

public static class AuthConfig
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = "http://localhost:8081/realms/beeshop";
            options.Audience = "beeshop-auth-api";
            options.RequireHttpsMetadata = false;
        });

        return services;
    }
}
