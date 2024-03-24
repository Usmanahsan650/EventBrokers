using System.Text;
using Microsoft.Extensions.Logging;
using rabbit.DataContracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbit;

public class RabbitMq:IEventBroker
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly string _topic;
    private readonly IModel _channel;
    public RabbitMq(RabbitMqConfiguration configuration,string topic)
    {
        _topic = topic;
        var factory = new ConnectionFactory();
        factory.UserName = configuration.User;
        factory.Password = configuration.Password;
        factory.VirtualHost = "/";
        factory.HostName = configuration.HostName;
        factory.Port = configuration.PortNumber;
        _connectionFactory = factory;
        _channel=Setup(topic:topic);
    }
    public event IEventBroker.OnEventRecieveDelegate? onEventReceived;
    public Task Subscribe(IEventBroker.OnEventRecieveDelegate onEventRecieveDelegate)
    {
        onEventReceived += onEventRecieveDelegate;
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            try
            {
                var body = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
                onEventReceived(body);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                //Todo: implement on error
                Console.WriteLine(e);
                _channel.BasicNack(ea.DeliveryTag, false,true);

            }
            
        };
        _channel.BasicConsume(queue: _topic,
            autoAck: false,
            consumer: consumer);
        return Task.CompletedTask;
    }

    private IModel Setup(string topic)
    {
        try
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: topic,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            return channel;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in intializing RabbitMq ", ex.Message);
            return null;
        }
    }

    public Task Publish(string eventBody)
    {
        var body = Encoding.UTF8.GetBytes(eventBody);
        IBasicProperties basicProperties = _channel.CreateBasicProperties();
        _channel.BasicPublish(exchange: string.Empty, routingKey: _topic, basicProperties, body);
        return Task.CompletedTask;
    }
    
}