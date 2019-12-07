using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TelemetryFunc
{
	public static class CosmosDbTrigger
	{
		[FunctionName("CosmosDbTrigger")]
		public static async Task Run(
			[CosmosDBTrigger("TelemetryDb", "TelemetryContainer",
				ConnectionStringSetting = "DbConnection",
				LeaseCollectionName = "leases")]IReadOnlyList<Document> input,
			[SignalR(HubName = "officefuturehub")] IAsyncCollector<SignalRMessage> signalRMessages,
			ILogger log)
		{
			if (input != null && input.Count > 0)
			{
				foreach (var telemetry in input)
				{
					await signalRMessages.AddAsync(new SignalRMessage
					{
						Target = "broadcastMessage", // The name of the method to invoke on each client
						Arguments = new[] { telemetry } // The arguments to pass to the method
					});
					log.LogInformation("Document sended " + telemetry);
				}
				log.LogInformation("Documents modified " + input.Count);
				log.LogInformation("First document Id " + input[0].Id);
			}
		}
	}
}
