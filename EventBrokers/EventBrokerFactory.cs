using rabbit.DataContracts;
using Serilog;

namespace rabbit.EventBrokers;

public static class EventBrokerFactory
{
    public static IEventBroker GetEventBroker(EventBrokerConfig config,ILogger logger)
    {
        switch (config.BrokerType)
        {
            case EventBroker.RabbitMq:
            {
                return new RabbitMq(logger,(RabbitMqConfiguration)config);
            }
        }
        throw new BrokerExceptionsException("Broker Not Implemented ");
    }
}