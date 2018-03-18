using System;
using System.Linq;
using System.Text;
using MixerLib;
using MixerLib.Events;
using static System.Console;
using static System.ConsoleColor;

namespace ChatMonitor
{
   internal static class Program
   {
      private static void Main(string[] args)
      {
         Console.WriteLine("MixerLib ChatMonitor example\n");

         try
         {
            Console.Write("Connecting...");
            using (var mixer = MixerClient.StartAsync("eurogamerspain").Result)
            {
               Console.WriteLine("OK");

               var (title, gameTypeId) = mixer.RestClient.GetChannelInfoAsync().Result;
               var game = mixer.RestClient.LookupGameTypeByIdAsync(gameTypeId.GetValueOrDefault()).Result;
               Console.WriteLine($"Title: '{title}'");
               Console.WriteLine($"Game:  '{game?.Name}'");

               var uptime = mixer.GetUptime();
               if (uptime.HasValue)
                  Console.WriteLine($"Channel has been live for {uptime} with {mixer.CurrentViewers} viewers currently.");
               else
                  Console.WriteLine("Channel is offline.");

               mixer.ChatMessage += Mixer_ChatMessage;

               Console.WriteLine("\nPress ENTER to exit\n");
               Console.ReadLine();

               mixer.ChatMessage -= Mixer_ChatMessage;
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine($"\nERROR: {ex.Message}");
         }
      }

      private static void Mixer_ChatMessage(object sender, ChatMessageEventArgs e)
      {
         ForegroundColor = DarkGray;
         Write($"{DateTime.Now.ToLongTimeString()} ");
         ForegroundColor = Gray;
         if (e.IsOwner)
            ForegroundColor = White;
         else if (e.IsModerator)
            ForegroundColor = Green;
         else if (e.Roles.Contains("Subscriber"))
            ForegroundColor = Cyan;
         else if (e.Roles.Contains("Pro"))
            ForegroundColor = Magenta;
         Write($"{e.UserName} ");
         ForegroundColor = DarkGray;
         Write($"[{string.Join(',', e.Roles)}] ");
         ForegroundColor = Gray;
         WriteLine(TrimMessage(e.Message));
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
   }
}
