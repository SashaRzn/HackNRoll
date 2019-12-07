using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using OfficeFuture.Common;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeFuture.Device
{
	class Program
	{
		private static DeviceClient _device;
		private static Random _random = new Random();
		private static int _delayBetweenTelemetry = 2000;
		private const string DeviceConnectionString = "HostName=OfficeFuture-IoTHub.azure-devices.net;DeviceId=device-01;SharedAccessKey=kcXjqouahxU1uRjHn1DRgIwBq5JtK9HvbvHFqkQGBU4=";

		static async Task Main(string[] args)
		{
			Console.WriteLine("Initializing device...");

			_device = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
			await _device.OpenAsync();

			Console.WriteLine("Device is connected!");
			Console.WriteLine("Device running...");

			var tokenSource2 = new CancellationTokenSource();
			CancellationToken ct = tokenSource2.Token;

			var task = new TaskFactory().StartNew(async () =>
			{
				// Were we already canceled?
				ct.ThrowIfCancellationRequested();

				var initTelemetry = GetInitialTelemetry();

				bool moreToDo = true;
				while (moreToDo)
				{
					var telemetry = GetRealTelemetry(initTelemetry);
					var payload = JsonConvert.SerializeObject(telemetry);
					var message = new Message(Encoding.ASCII.GetBytes(payload));
					await _device.SendEventAsync(message, ct);

					Console.WriteLine($"Temeletry sent. Temperature: {telemetry.Temperature:F}, Humidity: {telemetry.Humidity:F}");

					await Task.Delay(_delayBetweenTelemetry, ct);

					// Poll on this property if you have to do
					// other cleanup before throwing.
					if (ct.IsCancellationRequested)
					{
						// Clean up here, then...
						ct.ThrowIfCancellationRequested();
					}
				}
			}, tokenSource2.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);


			Console.ReadLine();
			Console.WriteLine("Press again");
			Console.ReadLine();

			tokenSource2.Cancel();

			// Just continue on this thread, or await with try-catch:
			try
			{
				await task;
			}
			catch (OperationCanceledException e)
			{
				Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
			}
			finally
			{
				tokenSource2.Dispose();
			}

			Console.WriteLine("Device stopped!");
			Console.ReadKey();
		}

		private static Telemetry GetRealTelemetry(Telemetry previousData)
		{
			return new Telemetry
			{
				Temperature = GenerateTelemetryValue(previousData.Temperature),
				Humidity = GenerateTelemetryValue(previousData.Humidity)
			};
		}

		private static Telemetry GetInitialTelemetry()
		{
			return new Telemetry
			{
				Humidity = 67,
				Temperature = 27
			};
		}

		private static double GenerateTelemetryValue(double previousValue)
		{
			if (_random.NextDouble() > 0.64)
			{
				return previousValue + _random.Next(1, 5) + _random.NextDouble();
			}

			return previousValue - _random.Next(1, 3) - _random.NextDouble();
		}
	}
}
