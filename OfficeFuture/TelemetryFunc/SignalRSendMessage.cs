using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Threading.Tasks;

namespace TelemetryFunc
{
	public static class SignalRSendMessage
	{
		[FunctionName("SendMessage")]
		public static Task SendMessage(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")]object message,
			[SignalR(HubName = "officefuturehub")]IAsyncCollector<SignalRMessage> signalRMessages)
		{
			return signalRMessages.AddAsync(
				new SignalRMessage
				{
					Target = "newMessage",
					Arguments = new[] { message }
				});
		}
	}
}
