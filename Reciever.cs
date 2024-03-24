using Microsoft.Extensions.Hosting;
using Serilog;

namespace rabbit;

public class Reciever:BackgroundService
{
    private ILogger _logger;
    private IEventBroker _eventBroker;
    public Reciever(ILogger logger)
    {
        _logger = logger.ForContext<Reciever>();
        _eventBroker=EventBrokerFactory.GetEventBroker(config.RabbitMqConfiguration,config.EventTopic,_logger);

    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {            
        try
        {        await _eventBroker.Subscribe(OnEventReceived,OnEventError);

            while (true)
            {
                await Task.Delay(5000,stoppingToken);
            }
        }
        catch (Exception e)
        {
            RemoveEventBindings();
            Console.WriteLine(e);
           
            _logger.Error(e.Message, new { Exception = e });

        }
        return ;
    }

    private void RemoveEventBindings()
    {
        _eventBroker.OnEventRecieved -= OnEventReceived;
        _eventBroker.OnEventProcessingError -= OnEventError;
    }
    private void OnEventReceived(string message)
    {
        Console.WriteLine("Recieved "+message);
    }
    private void OnEventError(string message,Exception exception)
    {
        Console.WriteLine("Recieved "+message);
    }
}