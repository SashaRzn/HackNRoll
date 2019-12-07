using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using OfficeFuture.Common;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OfficeFuture.ManualDevice
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

			Console.WriteLine("Press a key to perform an action:");
			Console.WriteLine("q: quits");
			Console.WriteLine("s: send telemetry to Hub");

			var initTelemetry = GetInitialTelemetry();

			var quitRequested = false;
			while (!quitRequested)
			{
				Console.Write("Action? ");
				var input = Console.ReadKey().KeyChar;
				Console.WriteLine();

				switch (Char.ToLower(input))
				{
					case 'q':
						quitRequested = true;
						break;
					case 's':
						break;
				}

				var telemetry = GetRealTelemetry(initTelemetry);
				var payload = JsonConvert.SerializeObject(telemetry);
				var message = new Message(Encoding.ASCII.GetBytes(payload));
				await _device.SendEventAsync(message);

				Console.WriteLine($"Temeletry sent. Temperature: {telemetry.Temperature:F}, Humidity: {telemetry.Humidity:F}");
			}

			Console.WriteLine("Disconnecting...");
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
