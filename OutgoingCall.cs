using System;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace pdo_appearance
{
    public class OutgoingCall
    {
        private readonly ILogger<OutgoingCall> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private TelemetryClient _telemetryClient;

        public OutgoingCall(ILogger<OutgoingCall> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration, TelemetryClient tc)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _telemetryClient = tc;
        }

        [Function(nameof(OutgoingCall))]
        public async Task Run([ServiceBusTrigger("outputQueue", Connection = "SBCon")] ServiceBusReceivedMessage message)
        {
            // using (_telemetryClient.StartOperation<RequestTelemetry>("operation"))
            // {
                _logger.LogInformation("Message ID: {id}", message.MessageId);
                _logger.LogInformation("Message Body: {body}", message.Body);
                _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
                var client = _httpClientFactory.CreateClient();
                var content = new ByteArrayContent(message.Body.ToArray()); 
                content.Headers.Add("Ocp-Apim-Subscription-Key",_configuration["TargetKey"]);           
                await client.PostAsync("https://apim-casc-appins-vbd.azure-api.net/target/service", content);
            // }
        }
    }
}
