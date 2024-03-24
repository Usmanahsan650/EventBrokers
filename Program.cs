using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Settings.Configuration;
using Serilog;
using Serilog.Events;
namespace rabbit;
public class Program
{
    public static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("./appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        var host = Initialize().ConfigureSeriLog(configuration).CreateDefaultApp(configuration).Build();
        
        Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));
        Console.WriteLine("starting");
        await host.RunAsync();
        Console.WriteLine("stopped");
    }

    private static Program Initialize()
    {
        return new Program();
    }
    private  Program ConfigureSeriLog(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        return this;
    }
    private  IHostBuilder CreateDefaultApp(IConfiguration configuration)
    {
        var builder = Host.CreateDefaultBuilder();
      
        builder.ConfigureServices(conf =>
        {
            conf.AddSingleton(Log.Logger);
            conf.AddSingleton(configuration);
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
