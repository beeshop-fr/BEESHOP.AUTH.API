using BEESHOP.AUTH.APPLICATION.Interfaces;
using BEESHOP.AUTH.PERSISTENCE.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace BEESHOP.AUTH.PERSISTENCE;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {


        services.AddHttpClient<IKeycloakAdminRepository, KeycloakAdminRepository>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:8081");
        });

        return services;
    }
}
