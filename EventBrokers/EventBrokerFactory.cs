using rabbit.DataContracts;
using Serilog;

namespace rabbit.EventBrokers;

public static class EventBrokerFactory
{
    public static IEventBroker GetEventBroker(EventBrokerConfig config, string topic,ILogger logger)
    {
        switch (config.BrokerType)
        {
            case EventBroker.RabbitMq:
            {
                return new RabbitMq(logger,(RabbitMqConfiguration)config,topic);
            }
        }

        return null;
    }
}