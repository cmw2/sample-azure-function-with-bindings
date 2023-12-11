using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace pdo_appearance
{
    public class IncomingFiles
    {
        private readonly ILogger<IncomingFiles> _logger;
        private TelemetryClient _telemetryClient;

        public IncomingFiles(ILogger<IncomingFiles> logger, TelemetryClient tc)
        {
            _logger = logger;
            _telemetryClient = tc;
        }

        [Function(nameof(IncomingFiles))]
        [ServiceBusOutput("outputQueue", Connection = "SBCon")]
        public async Task<string> Run([BlobTrigger("incoming-local/{name}", Connection = "storcascappinspoc_STORAGE")] Stream stream, string name)
        { 
            //_telemetryClient.Context.Operation.Id
            // using (_telemetryClient.StartOperation<RequestTelemetry>("operation"))
            // {                
                using var blobStreamReader = new StreamReader(stream);
                var content = await blobStreamReader.ReadToEndAsync();
                _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");

                return content;
            // }
        }
    }
}
