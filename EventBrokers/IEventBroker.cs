namespace rabbit.EventBrokers;

public interface IEventBroker
{
    public delegate void OnEventRecieveDelegate(string message,string source);
    public delegate void OnEventProcessingErrorDelegate(string message,Exception exception,string source);

    public event IEventBroker.OnEventRecieveDelegate? OnEventRecieved;
    public event IEventBroker.OnEventProcessingErrorDelegate? OnEventProcessingError;
    public Task Subscribe(string topic,IEventBroker.OnEventRecieveDelegate onEventRecieveDelegate,
        IEventBroker.OnEventProcessingErrorDelegate? eventProcessingError);

    public Task Publish(string eventBody,string ?topic);
    public void ConfigureTopic(string topic);
}