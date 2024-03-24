using System.Text;
using rabbit.DataContracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace rabbit;

public class RabbitMq:IEventBroker
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly string _topic;
    private readonly IModel _channel;
    private ILogger _logger;
    public RabbitMq(ILogger logger,RabbitMqConfiguration configuration,string topic)
    {
        _logger = logger.ForContext<RabbitMq>();
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

    public event IEventBroker.OnEventRecieveDelegate? OnEventRecieved;
    public event IEventBroker.OnEventProcessingErrorDelegate? OnEventProcessingError;


    public Task Subscribe(IEventBroker.OnEventRecieveDelegate onEventRecieveDelegate,IEventBroker.OnEventProcessingErrorDelegate? eventProcessingError)
    {
        OnEventRecieved += onEventRecieveDelegate;
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            try
            {
                var body = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
                OnEventRecieved?.Invoke(body);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                //Todo: implement on error
                _logger.Error(e.Message, new { Exception = e });
                _channel.BasicNack(ea.DeliveryTag, false,true);
    
                OnEventProcessingError?.Invoke(e.Message, exception: e);
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
            channel.ExchangeDeclare("my_fanout_exchange", ExchangeType.Fanout);
            channel.QueueDeclare(queue: topic,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            channel.QueueBind(topic, "my_fanout_exchange", "");
            return channel;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in intializing RabbitMq ", e.Message);
            _logger.Error(e.Message, new { Exception = e });
            return null;
        }
    }


    public Task Publish(string eventBody)
    {
        var body = Encoding.UTF8.GetBytes(eventBody);
        IBasicProperties basicProperties = _channel.CreateBasicProperties();
        _channel.BasicPublish(exchange: "my_fanout_exchange", routingKey: string.Empty, basicProperties, body);
        return Task.CompletedTask;
    }
    
}