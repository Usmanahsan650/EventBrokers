using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace rabbit;
public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateDefaultApp(args).Build();

        Console.WriteLine("starting");
        await host.RunAsync();
        Console.WriteLine("stopped");
    }

    private static IHostBuilder CreateDefaultApp(string[] args)
    {
        var builder = Host.CreateDefaultBuilder();
        builder.ConfigureServices(conf =>
        {
            conf.AddHostedService<SenderHost>();
            conf.AddHostedService<Reciever>();
        });
        builder.ConfigureLogging(conf =>
        {
            conf.ClearProviders();
            conf.AddConsole();
        });
        return builder;
    }
}
