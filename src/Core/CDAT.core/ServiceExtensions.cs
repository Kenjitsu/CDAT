using CDAT.core.Interfaces;
using CDAT.core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CDAT.core;
public static class ServiceExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IProcessService, ProcessService>();
        services.AddSingleton<IUserPromts, UserPromts>();
        services.AddSingleton<IGoogleTextSpeechServices, GoogleTextSpeechServices>();
        services.AddTransient<ICSVManagement, CSVManagement>();


        return services;
    }
}
