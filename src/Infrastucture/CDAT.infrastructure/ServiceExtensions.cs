using CDAT.core.Interfaces.Infrastructure;
using CDAT.infrastructure.ThirdParty;
using Microsoft.Extensions.DependencyInjection;

namespace CDAT.infrastructure;
public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IGoogleApiServices, GoogleApiServices>();

        return services;
    }
}
