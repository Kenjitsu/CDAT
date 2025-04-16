namespace CDAT.console;
public class App
{
    public async Task StartAsync(string[] args)
    {
        await Task.Delay(200);
        Console.WriteLine("Hola mundo!");
    }
}
