using RabbitMQ.Client.Events;

namespace rabbit;
using RabbitMQ.Client;
public interface IEventBroker
{
    public delegate void OnEventRecieveDelegate(string message);
    public delegate void OnEventProcessingErrorDelegate(string message,Exception exception);

    public event IEventBroker.OnEventRecieveDelegate? OnEventRecieved;
    public event IEventBroker.OnEventProcessingErrorDelegate? OnEventProcessingError;
    public Task Subscribe(IEventBroker.OnEventRecieveDelegate onEventRecieveDelegate,
        IEventBroker.OnEventProcessingErrorDelegate? eventProcessingError);

    public Task Publish(string eventBody);
}