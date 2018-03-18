using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MixerLib.Events;
using MixerLib.Helpers;

namespace MixerLib
{
   public static class MixerClient
   {
      public static async Task<IMixerClient> StartAsync(string channelName, ILoggerFactory loggerFactory = null)
      {
         var client = new MixerClientInternal(channelName, loggerFactory);
         await client.StartAsync();
         return client;
      }
   }

   internal class MixerClientInternal : IMixerClient, IMixerClientInternal
   {
      public event EventHandler<ServiceUpdatedEventArgs> Updated;
      public event EventHandler<ChatMessageEventArgs> ChatMessage;
      public event EventHandler<ChatUserInfoEventArgs> UserJoined;
      public event EventHandler<ChatUserInfoEventArgs> UserLeft;
      public event EventHandler<FollowedEventArgs> Followed;
      public event EventHandler<HostedEventArgs> Hosted;
      public event EventHandler<SubscribedEventArgs> Subscribed;
      public event EventHandler<ResubscribedEventArgs> Resubscribed;

      public string ChannnelName { get; }
      public string Token { get; set; }

      public int CurrentFollowers { get => _liveParser.Followers; }
      public int CurrentViewers { get => _liveParser.Viewers; }
      public bool IsAuthenticated => ( _chat?.IsAuthenticated ).GetValueOrDefault();
      public uint? ChannelID { get => _restClient.ChannelId; }
      public uint? UserId { get => _restClient.UserId; }
      public string UserName { get => _restClient.UserName; }

      readonly ILogger _logger;
      IMixerChat _chat;
      IMixerConstellation _live;
      IMixerRestClient _restClient;
      IMixerFactory _factory;
      readonly CancellationTokenSource _shutdownRequested;
      readonly ConstellationEventParser _liveParser;
      readonly ChatEventParser _chatParser;
      private readonly ILoggerFactory _loggerFactory;

      public IMixerRestClient RestClient { get => _restClient; }

      IMixerFactory IMixerClientInternal.Factory { get => _factory; }
      IMixerConstellation IMixerClientInternal.Constellation { get => _live; }
      IMixerChat IMixerClientInternal.Chat { get => _chat; }

      public MixerClientInternal(string channelName, ILoggerFactory loggerFactory = null)
      {
         if (string.IsNullOrEmpty(channelName))
            throw new ArgumentException("Can't be null or empty", nameof(channelName));
         if (loggerFactory == null)
            loggerFactory = new NullLoggerFactory();

         ChannnelName = channelName;

         _shutdownRequested = new CancellationTokenSource();
         _loggerFactory = loggerFactory;
         _logger = loggerFactory.CreateLogger(nameof(MixerClientInternal));

         _liveParser = new ConstellationEventParser(_logger, FireEvent);
         _chatParser = new ChatEventParser(_logger, FireEvent);
      }

      // Used during testing
      internal MixerClientInternal(string channelName, ILoggerFactory loggerFactory, IMixerFactory factory,
                           ConstellationEventParser liveParser = null, ChatEventParser chatParser = null)
        : this(channelName, loggerFactory)
      {
         _factory = factory ?? new MixerFactory(loggerFactory);
         _liveParser = liveParser ?? _liveParser;
         _chatParser = chatParser ?? _chatParser;
      }

      public async Task<IMixerClient> StartAsync()
      {
         _factory = _factory ?? new MixerFactory(_loggerFactory);

         _restClient = _factory.CreateRestClient();
         _live = _factory.CreateConstellation(_liveParser, _shutdownRequested.Token);
         _chat = _factory.CreateChat(_restClient, _chatParser, _shutdownRequested.Token);

         // Get our current channel information
         var (online, viewers, followers) = await _restClient.InitAsync(ChannnelName, Token);
         _liveParser.IsOnline = online;
         _liveParser.Followers = followers;
         _liveParser.Viewers = viewers;

         _logger.LogInformation("JOINING CHANNEL '{0}' as {1}. {2} with {3} viewers", _restClient.ChannelName,
           _restClient.HasToken ? _restClient.UserName : "anonymous (monitor only)",
           _liveParser.IsOnline == true ? "ONLINE" : "OFFLINE",
           _liveParser.Viewers);

         // Connect to live events (viewer/follower count etc)
         await _live.ConnectAndJoinAsync(_restClient.ChannelId.Value);

         // Connect to chat server
         await _chat.ConnectAndJoinAsync(_restClient.UserId.GetValueOrDefault(), _restClient.ChannelId.Value);

         return this;
      }

      private void FireEvent(string name, EventArgs args) => ReflectionHelper.RaiseEvent(this, name, args);

      public Task<bool> BanUserAsync(string userName) => _restClient.BanUserAsync(userName);
      public Task<bool> UnbanUserAsync(string userName) => _restClient.UnbanUserAsync(userName);
      public Task<bool> SendWhisperAsync(string userName, string message) => _chat.SendWhisperAsync(userName, message);
      public Task<bool> SendMessageAsync(string message) => _chat.SendMessageAsync(message);
      public Task<bool> TimeoutUserAsync(string userName, TimeSpan time) => _chat.TimeoutUserAsync(userName, time);

      public TimeSpan? GetUptime()
      {
         if (_liveParser.IsOnline == false)
            return null;
         if (!_liveParser.StreamStartedAt.HasValue)
            _liveParser.StreamStartedAt = _restClient.GetStreamStartedAtAsync().Result;
         if (!_liveParser.StreamStartedAt.HasValue)
            return null;

         // Remove milliseconds
         var seconds = ( DateTime.UtcNow - _liveParser.StreamStartedAt.Value ).Ticks / TimeSpan.TicksPerSecond;
         return TimeSpan.FromSeconds(Math.Max(0, seconds));
      }

      public void Dispose()
      {
         _chat.Dispose();
         _live.Dispose();
         _restClient.Dispose();
         _shutdownRequested.Dispose();
         GC.SuppressFinalize(this);
      }
   }
}
