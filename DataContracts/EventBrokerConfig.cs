using System.Runtime.Serialization;

namespace rabbit.DataContracts;

public class EventBrokerConfig
{
    public string HostName { get; set; }
    public int PortNumber { get; set; }

    public EventBroker BrokerType {
        get;
        set;
    }
}

public enum EventBroker
{
    [EnumMember]
    RabbitMq=1
}
public class RabbitMqConfiguration :EventBrokerConfig
{
    public string User { get; set; }
    public string Password { get; set; }
}