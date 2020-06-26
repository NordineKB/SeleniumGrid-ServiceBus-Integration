using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Selenium.Scheduler
{
	public static class Function1
	{
		const string ServiceBusConnectionString = "#ConnectionString#";
		const string QueueName = "#QueueName#";
		static IQueueClient queueClient;

		[FunctionName("Function1")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");

			queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

			await SendMessagesAsync();
			await queueClient.CloseAsync();

			string responseMessage = "Message Sent to Queue";

			return new OkObjectResult(responseMessage);
		}

		static async Task SendMessagesAsync()
		{
			try
			{
				string messageBody = $"Message Body";
				var message = new Message(Encoding.UTF8.GetBytes(messageBody));

				await queueClient.SendAsync(message);
			}
			catch (Exception exception)
			{
				Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
			}
		}
	}
}
