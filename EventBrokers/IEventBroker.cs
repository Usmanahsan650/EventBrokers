using RabbitMQ.Client.Events;

namespace rabbit;
using RabbitMQ.Client;
public interface IEventBroker
{
    public delegate void OnEventRecieveDelegate(string message);
    public  event OnEventRecieveDelegate onEventReceived;
    public Task  Subscribe(OnEventRecieveDelegate onEventRecieveDelegate);
    public Task Publish(string eventBody);
}