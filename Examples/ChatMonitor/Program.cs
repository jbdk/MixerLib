using System;
using System.Linq;
using System.Text.RegularExpressions;
using MixerLib;
using MixerLib.Events;
using static System.ConsoleColor;

namespace ChatMonitor
{
	internal static class Program
	{
		const string CHANNEL_NAME = "xbox";
		const string TOKEN = null;

		private static void Main(string[] args)
		{
			PrintLine(Cyan, "MixerLib ChatMonitor example\n");

			IAuthorization auth = ( TOKEN != null ) ? new Auth.ImplicitGrant(TOKEN) : null;

			try
			{
				Print(Gray, $"Connecting to mixer.com/{CHANNEL_NAME}...");
				using (var mixer = MixerClient.StartAsync(CHANNEL_NAME, auth).Result)
				{
					Print(Green, "OK\n");

					Print(Gray, $"You are connected as ");
					if (mixer.IsAuthenticated)
						PrintLine(Green, mixer.UserName + "\n");
					else
						PrintLine(Red, "anonymous\n");

					var (title, gameTypeId) = mixer.RestClient.GetChannelInfoAsync().Result;
					PrintLine(DarkGray, $"Title: '{title}'");
					var game = mixer.RestClient.LookupGameTypeByIdAsync(gameTypeId.GetValueOrDefault()).Result;
					PrintLine(DarkGray, $"Game:  '{game?.Name}'");

					var uptime = mixer.GetUptime();
					if (uptime.HasValue)
						PrintLine(DarkGray, $"Channel has been live for {uptime} with {mixer.CurrentViewers} viewers currently.");
					else
						PrintLine(DarkGray, "Channel is OFFLINE.");

					mixer.ChatMessage += Mixer_ChatMessage;
					mixer.ChannelUpdate += Mixer_StatusUpdate;

					PrintLine(Gray, "\nPress ENTER to exit\n\n");
					Console.ReadLine();

					mixer.ChatMessage -= Mixer_ChatMessage;
					mixer.ChannelUpdate -= Mixer_StatusUpdate;
				}
			}
			catch (Exception ex)
			{
				Print(Red, $"\nERROR: {ex.Message}");
			}
		}

		private static void Mixer_StatusUpdate(object sender, ChannelUpdateEventArgs e)
		{
			if (e.Channel.Online == null)
				return;
			Print(DarkGray, $"{DateTime.Now.ToLongTimeString()} ");
			Print(Gray, "Channel is now ");
			if (e.Channel.Online == true)
				Print(Green, "ONLINE\n");
			else
				Print(Red, "OFFLINE\n");
		}

		private static void Mixer_ChatMessage(object sender, ChatMessageEventArgs e)
		{
			if (e.UserName.EndsWith("Bot", StringComparison.InvariantCultureIgnoreCase) && e.IsModerator)
				return;

			Print(DarkGray, $"{DateTime.Now.ToLongTimeString()} ");
			var c = Gray;
			if (e.IsOwner)
				c = White;
			else if (e.IsModerator)
				c = Green;
			else if (e.Roles.Contains("Subscriber"))
				c = Cyan;
			else if (e.Roles.Contains("Pro"))
				c = Magenta;
			Print(c, $"{e.UserName} ");
			Print(DarkGray, $"[{string.Join(',', e.Roles)}] ");
			PrintLine(Gray, TrimMessage(e.Message));
		}

		/// <summary>
		/// Trim preceding whitespace chars down to one
		/// </summary>
		/// <param name="text">The test to be trimmed</param>
		/// <returns>Trimmed version of the text</returns>
		private static string TrimMessage(string text)
		{
			return Regex.Replace(text, @"/\s+/g", " ");
		}

		private static void Print(ConsoleColor color, string format, params object[] arg)
		{
			var c = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write(format, arg);
			Console.ForegroundColor = c;
		}

		private static void PrintLine(ConsoleColor color, string format, params object[] arg) => Print(color, format + "\n", arg);
	}
}
