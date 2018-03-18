using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// https://dev.mixer.com/reference/constellation/index.html

namespace MixerLib
{
	internal class MixerConstellation : IMixerConstellation
	{
		const string WS_URL = "wss://constellation.mixer.com";

		public string Token { get; set; }

		readonly ILoggerFactory _loggerFactory;
		readonly IMixerFactory _factory;
		readonly CancellationToken _shutdown;
		readonly ILogger _logger;
		IJsonRpcWebSocket _channel;
		readonly IEventParser _parser;

		public MixerConstellation(ILoggerFactory loggerFactory, IMixerFactory factory, IEventParser parser, CancellationToken shutdown)
		{
			_loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
			_factory = factory ?? throw new ArgumentNullException(nameof(factory));
			_logger = loggerFactory.CreateLogger(nameof(MixerConstellation));
			_shutdown = shutdown;
			_parser = parser ?? throw new ArgumentNullException(nameof(parser));
		}

		/// <summary>
		/// Connect to the live event server, and join our channel
		/// </summary>
		/// <param name="channelId">Our channelId</param>
		/// <returns></returns>
		public async Task ConnectAndJoinAsync(uint channelId)
		{
			_channel = _factory.CreateJsonRpcWebSocket(_logger, _parser);

			// Connect to the chat endpoint
			var continueTrying = true;
			while (continueTrying && !await _channel.TryConnectAsync(() => WS_URL, Token, async () => {
				// Join the channel and subscribe to events
				continueTrying = await _channel.SendAsync("livesubscribe",
						$"channel:{channelId}:update",
						$"channel:{channelId}:followed",
						$"channel:{channelId}:hosted",
						$"channel:{channelId}:unhosted",
						$"channel:{channelId}:subscribed",
						$"channel:{channelId}:resubscribed",
						$"channel:{channelId}:resubShared"
					);
			}))
				;

			if (!continueTrying)
			{
				_logger.LogError("Failed to connect to live endpoint {0}, giving up! (Channel wrong?)", WS_URL);
				_channel.Dispose();
				_channel = null;
				return;
			}
		}

		public void Dispose()
		{
			_channel?.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
