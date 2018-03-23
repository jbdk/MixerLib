using System;
using MixerLib;
using Serilog;
using Serilog.AspNetCore;

namespace PacketLogger
{
	internal static class Program
	{
		//
		// This will show all the packets coming in on the web-socket's on the console, by installing a ConsoleLogger
		//
		private static void Main(string[] args)
		{
			const string CHANNEL_NAME = "xbox";
			const string TOKEN = null;

			var logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.WriteTo.ColoredConsole()
				.CreateLogger();
			var loggerFactory = new SerilogLoggerFactory(logger);

			IAuthorization auth = ( TOKEN != null ) ? new Auth.ImplicitGrant(TOKEN) : null;

			try
			{
				Console.WriteLine($"Connecting to mixer.com/{CHANNEL_NAME}...");
				using (var mixer = MixerClient.StartAsync(CHANNEL_NAME, auth, loggerFactory).Result)
				{
					Console.WriteLine("OK\n");

					Console.WriteLine($"You are connected as ");
					if (mixer.IsAuthenticated)
						Console.WriteLine(mixer.UserName + "\n");
					else
						Console.WriteLine("anonymous\n");

					Console.WriteLine("\nPress ENTER to exit\n\n");
					Console.ReadLine();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\nERROR: {ex.Message}");
			}
		}
	}
}
