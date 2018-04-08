using System;
using System.Threading.Tasks;
using MixerLib.Events;

namespace MixerLib
{
	public interface IChatClient
	{
		/// <summary>True if the user is successfully authenticated against the endpoint</summary>
		bool IsAuthenticated { get; }

		/// <summary>Raised when a new chat/whisper message is received from the channel</summary>
		event EventHandler<ChatMessageEventArgs> ChatMessage;

		/// <summary>Raised when a user joins the channel</summary>
		event EventHandler<ChatUserInfoEventArgs> UserJoined;

		/// <summary>Raised when a user leaves the channel</summary>
		event EventHandler<ChatUserInfoEventArgs> UserLeft;

		/// <summary>Send a chat message</summary>
		Task<bool> SendMessageAsync(string message);

		/// <summary>Send a whisper message to a specific user</summary>
		Task<bool> SendWhisperAsync(string userName, string message);

		/// <summary>Timeout a user</summary>
		Task<bool> TimeoutUserAsync(string userName, TimeSpan time);

		/// <summary>Ban a user from the channel</summary>
		Task<bool> BanUserAsync(string userName);

		/// <summary>Unban a user from the channel</summary>
		Task<bool> UnbanUserAsync(string userName);
	}
}
