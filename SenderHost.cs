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
            
            Console.WriteLine("Define number of Events/minute :");
            var hops=Convert.ToInt32(Console.ReadLine());
        
            while (true)
            {
                Console.WriteLine("Sending Event");
                await broker.Publish(i++ + "{\r\n    \"id\": 1171979,\r\n    \"addedBy\": {\r\n        \"id\": 655837\r\n    },\r\n    \"author\": {\r\n        \"firstName\": \"Huda\",\r\n        \"lastName\": \"Rauf\",\r\n        \"id\": 0\r\n    },\r\n    \"tenant\": {\r\n        \"id\": 52700,\r\n        \"tenantName\": \"multi-language\",\r\n        \"subDomainUrl\": \"multi-language.beta.vidizmo.com\",\r\n        \"isSSLEnable\": true,\r\n        \"weight\": 0,\r\n        \"uuid\": \"6ddb148b-8e63-4678-8c74-9952ddd19e51\"\r\n    },\r\n    \"estimatedDuration\": 1079908,\r\n    \"title\": \"Language-Check\",\r\n    \"version\": 13,\r\n    \"addedDate\": \"2024-03-25T08:52:56.88Z\",\r\n    \"updatedDate\": \"2024-03-25T11:12:24.507Z\",\r\n    \"updatedBy\": {\r\n        \"id\": 0\r\n    },\r\n    \"size\": 24722945,\r\n    \"publishedOn\": \"2024-03-25T08:52:56.88Z\",\r\n    \"defaultViewingAccess\": \"None\",\r\n    \"status\": \"Published\",\r\n    \"weight\": 0,\r\n    \"format\": \"Folder\",\r\n    \"player\": {\r\n        \"id\": 0,\r\n        \"displayTimedData\": false,\r\n        \"isVideo360\": false\r\n    },\r\n    \"order\": 0,\r\n    \"inheritParentAccessRights\": true,\r\n    \"storageAccessTier\": \"None\",\r\n    \"lastAccessTime\": \"2024-03-25T11:12:21.08Z\",\r\n    \"lastTierUpdateTime\": \"2024-03-25T08:52:56.88Z\",\r\n    \"uuid\": \"db3feae3-9794-4ae4-ad87-9cda5fb791a3\"\r\n}");
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