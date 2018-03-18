using System;
using System.Threading.Tasks;
using MixerLib.Events;

namespace MixerLib
{
   public interface IChatClient
   {
      bool IsAuthenticated { get; }

      event EventHandler<ChatMessageEventArgs> ChatMessage;
      event EventHandler<ChatUserInfoEventArgs> UserJoined;
      event EventHandler<ChatUserInfoEventArgs> UserLeft;

      Task<bool> SendMessageAsync(string message);
      Task<bool> SendWhisperAsync(string userName, string message);
      Task<bool> TimeoutUserAsync(string userName, TimeSpan time);
      Task<bool> BanUserAsync(string userName);
      Task<bool> UnbanUserAsync(string userName);
   }
}
