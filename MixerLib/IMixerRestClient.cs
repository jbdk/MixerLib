using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MixerLib
{
	public interface IMixerRestClient : IDisposable
	{
		bool HasToken { get; }
		string ChannelName { get; }
		uint? ChannelId { get; }

		/// <summary>My user name or null if anonymously connected</summary>
		string UserName { get; }
		uint? UserId { get; }

		/// <summary>
		/// Get initial needed info from the mixer API
		/// </summary>
		/// <param name="channelName">Name of the channel</param>
		/// <param name="oauthToken">The users oauth token or null</param>
		/// <returns>Current number of viewers and followers as a tuple</returns>
		Task<(bool online, int viewers, int followers)> InitAsync(string channelName, string oauthToken);

		Task<API.Chats> GetChatAuthKeyAndEndpointsAsync();
		Task<uint?> LookupUserIdAsync(string userName);
		Task<bool> BanUserAsync(string userName);
		Task<bool> UnbanUserAsync(string userName);
		Task<DateTime?> GetStreamStartedAtAsync();
		Task<IEnumerable<API.GameTypeSimple>> LookupGameTypeAsync(string query);
		Task<API.GameTypeSimple> LookupGameTypeByIdAsync(uint gameTypeId);
		Task UpdateChannelInfoAsync(string title, uint? gameTypeId = null);
		Task<(string title, uint? gameTypeId)> GetChannelInfoAsync();
	}
}
