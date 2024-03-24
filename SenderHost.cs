using Microsoft.Extensions.Hosting;
namespace rabbit;
public class SenderHost:BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var broker=EventBrokerFactory.GetEventBroker(config.RabbitMqConfiguration,config.EventTopic);
        try
        {
            var i = 0;
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
        }
        
    }
}