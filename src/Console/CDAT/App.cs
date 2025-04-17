using CDAT.core.Interfaces;

namespace CDAT.console;
public class App
{
    private readonly IProcessService _processService;

    public App(IProcessService processService)
    {
        _processService = processService;
    }

    public async Task StartAsync(string[] args)
    {
        await _processService.ProcessAsync();
    }
}
