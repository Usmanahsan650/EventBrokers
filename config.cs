using rabbit.DataContracts;

namespace rabbit;

public static class config
{
    public static string EventTopic="bunny";
    public static RabbitMqConfiguration RabbitMqConfiguration = new DataContracts.RabbitMqConfiguration
    {
        HostName = "localhost",
        PortNumber = 5672,
        User = "guest",
        Password = "guest",
        BrokerType = EventBroker.RabbitMq
    };
}