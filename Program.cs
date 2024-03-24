using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
namespace rabbit;
public class Program
{
    public static async Task Main(string[] args)
    {
        
        var host = Initialize().ConfigureSeriLog().CreateDefaultApp(args).Build();
        
        Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));
        Console.WriteLine("starting");
        await host.RunAsync();
        Console.WriteLine("stopped");
    }

    private static Program Initialize()
    {
        return new Program();
    }
    private  Program ConfigureSeriLog()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose) // Set minimum level for Microsoft.* namespace
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, 
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}")            .CreateLogger();
        return this;
    }
    private  IHostBuilder CreateDefaultApp(string[] args)
    {
        var builder = Host.CreateDefaultBuilder();
      
        builder.ConfigureServices(conf =>
        {
            conf.AddSingleton(Log.Logger);
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
