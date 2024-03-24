using Microsoft.Extensions.Hosting;
using Serilog;

namespace rabbit;
public class SenderHost:BackgroundService
{
    private ILogger _logger;
    public SenderHost(ILogger logger)
    {
        _logger = logger.ForContext<SenderHost>();
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var broker=EventBrokerFactory.GetEventBroker(config.RabbitMqConfiguration,config.EventTopic,_logger);

            var i = 0;
            _logger.Verbose("Sender started");
            
            Console.WriteLine("Define number of hops/minute :");
            var hops=Convert.ToInt32(Console.ReadLine());
        
            while (true)
            {
                Console.WriteLine("Sending Hop");
                await broker.Publish("Hop : "+ i++);
                await Task.Delay((60*1000)/hops,stoppingToken);   
            }
            return ;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.Error(e.Message, new { Exception = e });
            
        }
        
    }
}