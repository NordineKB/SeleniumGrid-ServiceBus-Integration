using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Selenium.Listener
{
	public class Program
	{
		const string ServiceBusConnectionString = "#ConnectionString#";
		const string QueueName = "#QueueName#";
		static IQueueClient queueClient;
		static Selenium _selenium;

		public static async Task Main()
		{
			queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

			_selenium = new Selenium();

			RegisterOnMessageHandlerAndReceiveMessages();

			var hostBuilder = new HostBuilder()
			  .ConfigureServices((hostContext, services) =>
			  {
				  // Add your services with depedency injection.
			  });

			await hostBuilder.RunConsoleAsync();
		}

		static void RegisterOnMessageHandlerAndReceiveMessages()
		{
			// Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
			var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
			{
				// Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
				// Set it according to how many messages the application wants to process in parallel.
				MaxConcurrentCalls = 1,

				// Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
				// False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
				AutoComplete = false
			};

			// Register the function that will process messages
			queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
		}

		static async Task ProcessMessagesAsync(Message message, CancellationToken token)
		{
			await _selenium.UseSeleniumDriver();

			await queueClient.CompleteAsync(message.SystemProperties.LockToken);
		}

		static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
		{
			var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
			return Task.CompletedTask;
		}
	}

	public class TimedHostedService : IHostedService, IDisposable
	{
		private Timer _timer;

		public TimedHostedService()
		{
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("Timed Background Service is starting.");

			_timer = new Timer(DoWork, null, TimeSpan.Zero,
				TimeSpan.FromSeconds(5));

			return Task.CompletedTask;
		}

		private void DoWork(object state)
		{
			Console.WriteLine("Timed Background Service is working.");
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("Timed Background Service is stopping.");

			_timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}
}
