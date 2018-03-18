using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using MixerLib.Events;
using Newtonsoft.Json.Linq;

namespace MixerLib
{
   internal class ChatEventParser : IEventParser
   {
      private readonly ILogger _logger;
      private readonly Action<string, EventArgs> _fireEvent;

      public ChatEventParser(ILogger logger, Action<string, EventArgs> fireEvent)
      {
         _logger = logger ?? throw new ArgumentNullException(nameof(logger));
         _fireEvent = fireEvent ?? throw new ArgumentNullException(nameof(fireEvent));
      }

      public bool IsChat { get => true; }

      public void Process(string eventName, JToken data)
      {
         if (data == null || string.IsNullOrEmpty(eventName))
            return;

         switch (eventName)
         {
            case "ChatMessage":
               ParseChatMessage(data.GetObject<WS.ChatData>());
               break;
            case "UserJoin":
               ParseUserJoin(data.GetObject<WS.User>());
               break;
            case "UserLeave":
               ParseUserLeave(data.GetObject<WS.User>());
               break;
         }
      }

      private void ParseUserLeave(WS.User user)
      {
         _fireEvent(nameof(MixerClientInternal.UserLeft), new ChatUserInfoEventArgs {
            ChannelId = user.OriginatingChannel,
            UserId = user.Id,
            UserName = user.Username,
            Properties = {
          { "MixerRoles", user.Roles }
        }
         });
      }

      private void ParseUserJoin(WS.User user)
      {
         _fireEvent(nameof(MixerClientInternal.UserJoined), new ChatUserInfoEventArgs {
            ChannelId = user.OriginatingChannel,
            UserId = user.Id,
            UserName = user.Username,
            Properties = {
          { "MixerRoles", user.Roles }
        }
         });
      }

      private void ParseChatMessage(WS.ChatData data)
      {
         var combinedText = string.Concat(data.Messages.Message.Select(x => x.Text));

         var isWhisper = false;
         if (data.Messages?.Meta != null)
         {
            isWhisper = data.Messages.Meta.Whisper.GetValueOrDefault();
         }

         _fireEvent(nameof(MixerClientInternal.ChatMessage), new ChatMessageEventArgs {
            ChannelId = data.Channel,
            UserId = data.UserId,
            UserName = data.UserName,
            IsWhisper = isWhisper,
            IsModerator = data.UserRoles.Contains("Mod"),
            IsOwner = data.UserRoles.Contains("Owner"),
            Message = combinedText,
            Avatar = data.UserAvatar ?? string.Empty,
            Roles = data.UserRoles.ToArray(),
            UserLevel = data.UserLevel
         });
      }
   }
}
