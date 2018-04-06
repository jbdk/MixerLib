# MixerLib
Chat/event client for mixer.com

[![NuGet version (Bundgaard.MixerLib)](https://img.shields.io/nuget/v/Bundgaard.MixerLib.svg)](https://www.nuget.org/packages/Bundgaard.MixerLib/)
[![Build Status](https://travis-ci.org/jbdk/MixerLib.svg?branch=dev)](https://travis-ci.org/jbdk/MixerLib)
[![Build status](https://ci.appveyor.com/api/projects/status/evlyc4y2pmai2afa?svg=true)](https://ci.appveyor.com/project/jbdk/mixerlib)

## Authorization
This can run anonymously (no token) but to be able to do anything useful, it requires OAuth implicit grant authorization.
Go to http://www.mixerdevtools.com/gettoken, set the scopes needed: 
````
channel:update:self
chat:bypass_links
chat:bypass_slowchat
chat:change_ban
chat:chat
chat:connect
chat:timeout
chat:whisper
````

Click 'Get OAuth Token' and save the token in secrets (or where you need it).

You can use your own mixer account or create a dedicated user account.
The token can always be revoked under Account -> OAUTH-APPS, remove 'Mixer Dev Tools'

## Code sample
````csharp
namespace ChatMonitor
{
   internal static class Program
   {
      private static void Main(string[] args)
      {
         Console.WriteLine("MixerLib ChatMonitor example\n");

         const string CHANNEL_NAME = "xbox";
         const string TOKEN = null;

         try
         {
            Console.Write("Connecting...");
            IAuthorization auth = ( TOKEN != null ) ? new Auth.ImplicitGrant(TOKEN) : null;
            using (IMixerClient mixer = MixerClient.StartAsync(CHANNEL_NAME, auth).Result)
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
