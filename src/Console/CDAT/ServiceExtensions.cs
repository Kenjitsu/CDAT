using CDAT.core;
using CDAT.infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace CDAT.console;
public static class ServiceExtensions
{
    public static IHostBuilder CreateHostbuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((_, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
        })
            .ConfigureServices((_, services) =>
            {
                services.AddCoreServices();
                services.AddInfrastructureServices();
                services.AddSingleton<App>();
            });
}