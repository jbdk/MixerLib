using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MixerLib.Helpers;
using Polly;
using Polly.Retry;

namespace MixerLib
{
	internal class MixerRestClient : IMixerRestClient
	{
		const string API_URL = "https://mixer.com/api/v1/";
		public const int TIMEOUT_IN_SECONDS = 5;

		public int RetryDelay
		{
			get => _retryDelay;
			set {
				_retryDelay = value;
				UpdateRetryPolicy();
			}
		}
		public int MaxTries
		{
			get => _maxTries;
			set {
				_maxTries = value;
				UpdateRetryPolicy();
			}
		}
		public bool HasToken { get; private set; }
		public string ChannelName { get; private set; }
		public uint? ChannelId { get; private set; }
		public string UserName { get; private set; }
		public uint? UserId { get; private set; }

		readonly ILogger _logger;
		readonly HttpClient _client;
		private bool _initDone;
		private RetryPolicy<HttpResponseMessage> _retryPolicy;
		private int _retryDelay = 2000;
		private int _maxTries = 4;
		static readonly HttpStatusCode[] s_httpStatusCodesWorthRetrying = {
			HttpStatusCode.RequestTimeout, // 408
			HttpStatusCode.InternalServerError, // 500
			HttpStatusCode.BadGateway, // 502
			HttpStatusCode.ServiceUnavailable, // 503
			HttpStatusCode.GatewayTimeout // 504
		};

		/// <summary>
		/// Construct new MixerRestClient
		/// </summary>
		public MixerRestClient(ILoggerFactory loggerFactory, HttpClient client)
		{
			if (loggerFactory == null)
				throw new ArgumentNullException(nameof(loggerFactory));

			_logger = loggerFactory.CreateLogger(nameof(MixerRestClient));

			_client = client ?? throw new ArgumentNullException(nameof(client));
			_client.BaseAddress = new Uri(API_URL);
			_client.DefaultRequestHeaders.Add("Accept", "application/json");
			_client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoStore = true, NoCache = true };

			UpdateRetryPolicy();
		}

		private void UpdateRetryPolicy()
		{
			_retryPolicy = Policy
				.HandleInner<HttpRequestException>()
				.OrInner<TaskCanceledException>()
				.OrResult<HttpResponseMessage>(r => s_httpStatusCodesWorthRetrying.Contains(r.StatusCode))
				.WaitAndRetryAsync(MaxTries, (_) => TimeSpan.FromMilliseconds(RetryDelay));
		}

		public async Task<(bool online, int viewers, int followers)> InitAsync(string channelName, string oauthToken)
		{
			_initDone = false;
			UserId = null;
			ChannelId = 0;
			UserName = null;

			ChannelName = channelName;
			HasToken = !string.IsNullOrEmpty(oauthToken);
			if (HasToken)
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
			else
				_client.DefaultRequestHeaders.Authorization = null;

			var tryCounter = 0;
			while (true)
			{
				try
				{
					var req = $"channels/{WebUtility.UrlEncode(ChannelName)}?fields=id,numFollowers,viewersCurrent,online";
					var channelInfo = await GetObjectAsync<API.Channel>(req);
					ChannelId = channelInfo.Id;

					if (HasToken)
					{
						// User might not be joining own channel
						var me = await GetObjectAsync<API.User>("users/current");
						UserId = me.Id;
						UserName = me.Username;
					}
					_initDone = true;
					return (channelInfo.Online.GetValueOrDefault(), (int)channelInfo.ViewersCurrent.GetValueOrDefault(), (int)channelInfo.NumFollowers.GetValueOrDefault());
				}
				catch (HttpRequestException ex)
				{
					if (++tryCounter == MaxTries)
						throw new MixerException($"Can't find channel '{ChannelName}'", ex);
					await Task.Delay(RetryDelay);
				}
			}
		}

		/// <summary>
		/// Use the REST API to get id of username
		/// </summary>
		/// <param name="userName">Name of the user</param>
		/// <returns>Id of the user</returns>
		public async Task<uint?> LookupUserIdAsync(string userName)
		{
			if (!_initDone)
				throw new Exception("Call InitAsync() first!");

			try
			{
				var req = $"channels/{WebUtility.UrlEncode(userName)}?noCount=1";
				var channel = await GetObjectAsync<API.Channel>(req);
				return channel.UserId;
			}
			catch (HttpRequestException)
			{
				_logger.LogError("Unknown user '{0}'", userName);
				return null;
			}
		}

		/// <summary>
		/// Ban user from chat
		/// </summary>
		public async Task<bool> BanUserAsync(string userName)
		{
			if (string.IsNullOrWhiteSpace(userName))
				throw new ArgumentException("Must not be null or empty", nameof(userName));
			if (!_initDone)
				throw new Exception("Call InitAsync() first!");

			if (!HasToken)
				return false;

			try
			{
				if (string.IsNullOrWhiteSpace(userName))
					throw new ArgumentException("Must not be null or empty", nameof(userName));

				var userId = await LookupUserIdAsync(userName);

				// Add user as banned from our channel
				var req = $"channels/{ChannelId}/users/{userId}";
				await PatchAsync(req, new { add = new[] { "Banned" } });
				return true;
			}
			catch (Exception e)
			{
				_logger.LogError("Error banning user '{0}': {1}", userName, e.Message);
				return false;
			}
		}

		/// <summary>
		/// Unban user from chat
		/// </summary>
		public async Task<bool> UnbanUserAsync(string userName)
		{
			if (string.IsNullOrWhiteSpace(userName))
				throw new ArgumentException("Must not be null or empty", nameof(userName));
			if (!_initDone)
				throw new Exception("Call InitAsync() first!");

			if (!HasToken)
				return false;

			try
			{
				var userId = await LookupUserIdAsync(userName);

				// Add user as banned from our channel
				var req = $"channels/{ChannelId}/users/{userId}";
				await PatchAsync(req, new { remove = new[] { "Banned" } });
				return true;
			}
			catch (Exception e)
			{
				_logger.LogError("Error unbanning user '{0}': {1}", userName, e.Message);
				return false;
			}
		}

		/// <summary>
		/// Get stream start time from REST API
		/// </summary>
		/// <returns>Start time of stream, or null if stream is offline</returns>
		public async Task<DateTime?> GetStreamStartedAtAsync()
		{
			if (!_initDone)
				throw new Exception("Call InitAsync() first!");

			var req = $"channels/{ChannelId}/manifest.light2";
			_logger.LogTrace("GET {0}{1}", API_URL, req);
			var response = await GetAsync(req);
			if (response.StatusCode != HttpStatusCode.OK)
				return null;
			var json = await response.Content.ReadAsStringAsync();
			var manifest = MixerSerializer.Deserialize<API.ChannelManifest2>(json);
			return manifest.StartedAt.ToUniversalTime();
		}

		/// <summary>
		/// Get auth key and endpoints for connecting websocket to chat
		/// </summary>
		public Task<API.Chats> GetChatAuthKeyAndEndpointsAsync()
		{
			if (!_initDone)
				throw new Exception("Call InitAsync() first!");

			// Get chat authkey and chat endpoints
			var req = $"chats/{ChannelId}";
			return GetObjectAsync<API.Chats>(req);
		}

		/// <summary>
		/// Get channel current stream title and gameTypeId
		/// </summary>
		public async Task<(string title, uint? gameTypeId)> GetChannelInfoAsync()
		{
			if (!_initDone)
				throw new Exception("Call InitAsync() first!");

			var result = await GetObjectAsync<API.Channel>($"channels/{ChannelId}?fields=name,typeId");
			return (result.Name, result.TypeId);
		}

		/// <summary>
		/// Changes the channels steam title and game type
		/// NOTE: Requires OAuth 2.0 Scopes: channel:update
		/// </summary>
		public Task UpdateChannelInfoAsync(string title, uint? gameTypeId = null)
		{
			if (!_initDone)
				throw new Exception("Call InitAsync() first!");
			if (!HasToken)
				throw new Exception("Requires authorization token!");

			object data = null;
			if (gameTypeId != null)
				data = new { name = title, typeId = gameTypeId };
			else
				data = new { name = title };

			return PatchAsync($"channels/{ChannelId}", data);
		}

		/// <summary>
		/// Lookup game type info.
		/// </summary>
		/// <param name="query">Game name</param>
		/// <returns>GameType info or null if not found</returns>
		public async Task<API.GameTypeSimple> LookupGameTypeByIdAsync(uint gameTypeId)
		{
			if (!_initDone)
				throw new Exception("Call InitAsync() first!");

			var result = await GetAsync($"types/{gameTypeId}");
			if (result.StatusCode != HttpStatusCode.OK)
				return null;
			var json = await result.Content.ReadAsStringAsync();
			return MixerSerializer.Deserialize<API.GameTypeSimple>(json);
		}

		/// <summary>
		/// Lookup game type info from id.
		/// </summary>
		/// <param name="query">Game name</param>
		/// <returns>Up to 10 gameTypes matching the query</returns>
		public async Task<IEnumerable<API.GameTypeSimple>> LookupGameTypeAsync(string query)
		{
			if (!_initDone)
				throw new Exception("Call InitAsync() first!");

			return await GetObjectAsync<API.GameTypeSimple[]>($"types?limit=10&noCount=1&scope=all&query={WebUtility.UrlEncode(query)}");
		}

		#region HttpClient helpers

		async Task<HttpResponseMessage> GetAsync(string requestUri)
		{
			_logger.LogTrace("GET {0}{1}", API_URL, requestUri);

			try
			{
				return await _retryPolicy.ExecuteAsync(() => _client.GetAsync(requestUri));
			}
			catch (Exception e)
			{
				_logger.LogError("GET request failed: {0}", e.Message);
				throw;
			}
		}

		async Task<T> GetObjectAsync<T>(string requestUri)
		{
			var response = await GetAsync(requestUri);
			response.EnsureSuccessStatusCode();
			var json = await response.Content.ReadAsStringAsync();
			return MixerSerializer.Deserialize<T>(json);
		}

		async Task<string> PatchAsync<T>(string requestUri, T data)
		{
			_logger.LogTrace("PATCH {0}{1}", API_URL, requestUri);

			var message = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri) {
				Content = new JsonContent(data)
			};

			try
			{
				var response = await _retryPolicy.ExecuteAsync(() => _client.SendAsync(message));
				response.EnsureSuccessStatusCode();
				if (response.Content != null)
					return await response.Content.ReadAsStringAsync();
				return null;
			}
			catch (Exception e)
			{
				_logger.LogError("PATCH request failed: {0}", e.Message);
				throw;
			}
		}

		#endregion

		public void Dispose()
		{
			_client?.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
