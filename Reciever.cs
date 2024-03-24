using Microsoft.Extensions.Hosting;

namespace rabbit;

public class Reciever:BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {            
        var broker=EventBrokerFactory.GetEventBroker(config.RabbitMqConfiguration,config.EventTopic);
        await broker.Subscribe(OnEventReceived);
        while (true)
        {
            await Task.Delay(5000,stoppingToken);
        }
        return ;
    }

    private void OnEventReceived(string message)
    {
        Console.WriteLine("Recieved "+message);
    }
}