# MixerLib
Chat/event client for mixer.com

## Sample
````csharp
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
            using (var mixer = MixerClient.StartAsync("xbox").Result)
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
         Console.WriteLine($"{e.UserName} : {e.Message}");
      }
   }
}

````
