using System.Globalization;
using rabbit.DataContracts;

namespace rabbit;

public static class EventBrokerFactory
{
    public static IEventBroker GetEventBroker(EventBrokerConfig config, string topic)
    {
        switch (config.BrokerType)
        {
            case EventBroker.RabbitMq:
            {
                return new RabbitMq((RabbitMqConfiguration)config,topic);
            }
        }

        return null;
    }
}