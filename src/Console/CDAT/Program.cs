using CDAT.console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = ServiceExtensions.CreateHostbuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    //_log.Debug("La aplicación se ha iniciado...");
    await services.GetRequiredService<App>().StartAsync(args);
}
catch (Exception ex)
{
    //_log.ErrorFormat("Ha ocurrido un error al ejecutar la aplicación. Errores: {0} - {1}", ex.Message, ex.StackTrace);
    Console.WriteLine("Ha ocurrido un error al ejecutar la aplicación. Errores: {0} - {1}", ex.Message, ex.StackTrace);
}
