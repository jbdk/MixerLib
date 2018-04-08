using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MixerLib
{
	public interface IMixerRestClient : IDisposable
	{
		/// <summary>True if a authorization token has been supplied to the library</summary>
		bool HasToken { get; }

		/// <summary>Name of the channel</summary>
		string ChannelName { get; }

		/// <summary>Id of the channel</summary>
		uint? ChannelId { get; }

		/// <summary>My user name or null if anonymously connected</summary>
		string UserName { get; }

		/// <summary>My user id or null if anonymously connected</summary>
		uint? UserId { get; }

		/// <summary>
		/// Get initial needed info from the mixer API
		/// </summary>
		/// <param name="channelName">Name of the channel</param>
		/// <param name="oauthToken">The users oauth token or null</param>
		/// <returns>Current number of viewers and followers as a tuple</returns>
		Task<(bool online, int viewers, int followers)> InitAsync(string channelName, string oauthToken);

		/// <summary>Get chat endpoints and the chat websocket auth key</summary>
		Task<API.Chats> GetChatAuthKeyAndEndpointsAsync();

		/// <summary>Find the id of a user</summary>
		Task<uint?> LookupUserIdAsync(string userName);

		/// <summary>Ban a user from the channel</summary>
		Task<bool> BanUserAsync(string userName);

		/// <summary>Unban a user from the channel</summary>
		Task<bool> UnbanUserAsync(string userName);

		/// <summary>Get the time when the stream went online</summary>
		/// <returns>Start time for stream, or null if offline</returns>
		Task<DateTime?> GetStreamStartedAtAsync();

		/// <summary>Search for a gametype</summary>
		/// <returns>List of matching games (max 10)</returns>
		Task<IEnumerable<API.GameTypeSimple>> LookupGameTypeAsync(string query);

		/// <summary>Get game type with a give id</summary>
		/// <returns>The game type, or null if not found</returns>
		Task<API.GameTypeSimple> LookupGameTypeByIdAsync(uint gameTypeId);

		/// <summary>
		/// Set stream title and maybe game type
		/// </summary>
		/// <param name="title">Title of the stream</param>
		/// <param name="gameTypeId">Game type id or null if it should not be changed</param>
		Task UpdateChannelInfoAsync(string title, uint? gameTypeId = null);

		/// <summary>
		/// Get current stream title and gametype id
		/// </summary>
		/// <returns>Tuple with title and gametype id (might be null)</returns>
		Task<(string title, uint? gameTypeId)> GetChannelInfoAsync();
	}
}
