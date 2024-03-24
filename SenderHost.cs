using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using rabbit.DataContracts;
using Serilog;
using Newtonsoft.Json;
using rabbit.EventBrokers;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace rabbit;
public class SenderHost:BackgroundService
{
    private ILogger _logger;
    private readonly IConfiguration _configuration;
    public SenderHost(ILogger logger,IConfiguration configuration)
    {
        _configuration = configuration;
        _logger = logger.ForContext<SenderHost>();
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var brokerConfig=_configuration.GetSection("EventBroker").Get<RabbitMqConfiguration>();
            if (brokerConfig == null)
                throw new Exception("Broker Configuration missing");
            var topic=_configuration.GetSection("EventTopic").Value;

            var broker=EventBrokerFactory.GetEventBroker(brokerConfig,topic,_logger);

            var i = 0;
            _logger.Verbose("Sender started");
            
            Console.WriteLine("Define number of hops/minute :");
            var hops=Convert.ToInt32(Console.ReadLine());
        
            while (true)
            {
                Console.WriteLine("Sending Hop");
                await broker.Publish("Hop : "+ i++);
                await Task.Delay((60*1000)/hops,stoppingToken);   
            }
            return ;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.Error(e.Message, new { Exception = e });
            
        }
        
    }
}