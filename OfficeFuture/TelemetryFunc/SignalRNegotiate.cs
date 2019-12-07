using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace TelemetryFunc
{
	public static class SignalRNegotiate
	{
		[FunctionName("negotiate")]
		public static SignalRConnectionInfo Negotiate(
			[HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
			[SignalRConnectionInfo(HubName = "officefuturehub")] SignalRConnectionInfo connectionInfo)
		{
			return connectionInfo;
		}

		//[FunctionName("negotiate")]
		//public static IActionResult Negotiate(
		//	[HttpTrigger()] HttpRequest req,
		//	[SignalRConnectionInfo(HubName = "officefuturehub")]
		//	SignalRConnectionInfo connectionInfo)
		//{
		//	return new OkObjectResult(connectionInfo);
		//}

		//[FunctionName("negotiate")]
		//public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequest req,
		//	[SignalRConnectionInfo(HubName = "officefuturehub")]SignalRConnectionInfo info, ILogger log)
		//{
		//	log.LogInformation("Running negotiate function");
		//	return info != null
		//		? (ActionResult)new OkObjectResult(info)
		//		: new NotFoundObjectResult("Failed to load SignalR Info.");
		//}
	}
}
