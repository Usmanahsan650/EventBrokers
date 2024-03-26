using System.Security.Principal;
using System.Text;
using rabbit.DataContracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace rabbit.EventBrokers;

public class RabbitMq:IEventBroker
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly IModel _channel;
    private ILogger _logger;
    public RabbitMq(ILogger logger,RabbitMqConfiguration configuration)
    {
        _logger = logger.ForContext<RabbitMq>();
        var factory = new ConnectionFactory();
        factory.UserName = configuration.User;
        factory.Password = configuration.Password;
        factory.VirtualHost = "/";
        factory.HostName = configuration.HostName;
        factory.Port = configuration.PortNumber;
        _connectionFactory = factory;
        _channel=Setup();
    }

    public event IEventBroker.OnEventRecieveDelegate? OnEventRecieved;
    public event IEventBroker.OnEventProcessingErrorDelegate? OnEventProcessingError;


    public Task Subscribe(string topic,IEventBroker.OnEventRecieveDelegate onEventRecieveDelegate,IEventBroker.OnEventProcessingErrorDelegate? eventProcessingError)
    {
        OnEventRecieved += onEventRecieveDelegate;
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            try
            {
                var body = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
                OnEventRecieved?.Invoke(body,ea.Exchange);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, new { Exception = e });
                _channel.BasicNack(ea.DeliveryTag, false,true);
                OnEventProcessingError?.Invoke(e.Message, exception: e,ea.Exchange);
            }
            
        };
        _channel.BasicConsume(queue: topic,
            autoAck: false,
            consumer: consumer);
        return Task.CompletedTask;
    }

    private IModel Setup()
    {
        try
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(Constant.FANOUT_EXCHANGE_NAME, ExchangeType.Fanout);
            return channel;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in intializing RabbitMq ", e.Message);
            _logger.Error(e.Message, new { Exception = e });
            return null;
        }
    }

    public void ConfigureTopic(string topic)
    {
        try
        {
            _channel.QueueDeclare(queue: topic,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);
            _channel.QueueBind(topic, Constant.FANOUT_EXCHANGE_NAME, "");
        }
        catch(Exception ex)
        {
            _logger.Error("Failed to add topic ex:",ex);
            throw;
        }


    }
    public Task Publish(string eventBody,string? topic)
    {
        try
        {
            var body = Encoding.UTF8.GetBytes(eventBody);
            IBasicProperties basicProperties = _channel.CreateBasicProperties();
            if (topic == null)
                _channel.BasicPublish(exchange: Constant.FANOUT_EXCHANGE_NAME, routingKey: string.Empty, basicProperties, body);
            else
                _channel.BasicPublish(exchange: string.Empty, routingKey: topic, basicProperties, body);
        }
        catch (Exception ex)
        {
            _logger.Error("Error while producing event ex:",ex);
            throw;
        }
        
        return Task.CompletedTask;
    }
    
}