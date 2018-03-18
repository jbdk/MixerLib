using System;
using System.Linq;
using System.Text;
using MixerLib;
using MixerLib.Events;
using static System.ConsoleColor;

namespace ChatMonitor
{
   internal static class Program
   {
      const string CHANNEL_NAME = "eurogamerspain";
      const string TOKEN = "";

      private static void Main(string[] args)
      {
         PrintLine(Cyan, "MixerLib ChatMonitor example\n");

         try
         {
            Print(Gray, $"Connecting to mixer.com/{CHANNEL_NAME}...");
            using (var mixer = MixerClient.StartAsync(CHANNEL_NAME, TOKEN).Result)
            {
               Print(Green, "OK\n");
               Print(Gray, $"You are connected as ");

               if (mixer.IsAuthenticated)
                  PrintLine(Green, mixer.UserName + "\n");
               else
                  PrintLine(Red, "anonymous\n");

               var (title, gameTypeId) = mixer.RestClient.GetChannelInfoAsync().Result;
               var game = mixer.RestClient.LookupGameTypeByIdAsync(gameTypeId.GetValueOrDefault()).Result;
               PrintLine(DarkGray, $"Title: '{title}'");
               PrintLine(DarkGray, $"Game:  '{game?.Name}'");

               var uptime = mixer.GetUptime();
               if (uptime.HasValue)
                  PrintLine(DarkGray, $"Channel has been live for {uptime} with {mixer.CurrentViewers} viewers currently.");
               else
                  PrintLine(DarkGray, "Channel is offline.");

               mixer.ChatMessage += Mixer_ChatMessage;

               PrintLine(Gray, "\nPress ENTER to exit\n\n");
               Console.ReadLine();
               mixer.ChatMessage -= Mixer_ChatMessage;
            }
         }
         catch (Exception ex)
         {
            Print(Red, $"\nERROR: {ex.Message}");
         }
      }

      private static void Mixer_ChatMessage(object sender, ChatMessageEventArgs e)
      {
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

      private static string TrimMessage(string text)
      {
         var sb = new StringBuilder(text.Length);
         bool wasWhitespace = false;
         foreach (char c in text)
         {
            if (char.IsWhiteSpace(c))
            {
               wasWhitespace = true;
            }
            else
            {
               if (wasWhitespace && sb.Length != 0)
                  sb.Append(' ');
               wasWhitespace = false;
               sb.Append(c);
            }
         }
         return sb.ToString();
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
