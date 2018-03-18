using System;
using System.Threading.Tasks;

// https://dev.mixer.com/reference/chat/index.html

namespace MixerLib
{
	public interface IMixerChat : IDisposable
	{
		bool IsAuthenticated { get; }
		string[] Roles { get; }
		Task ConnectAndJoinAsync(uint userId, uint channelId);
		Task<bool> SendWhisperAsync(string userName, string message);
		Task<bool> SendMessageAsync(string message);
		Task<bool> TimeoutUserAsync(string userName, TimeSpan time);
	}
}
