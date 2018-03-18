using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace MixerLib
{
	internal class MixerFactory : IMixerFactory
	{
		private readonly ILoggerFactory _loggerFactory;

		public MixerFactory(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
		}

		public IClientWebSocketProxy CreateClientWebSocket(bool isChat) => new ClientWebSocketProxy(isChat);
		public IMixerConstellation CreateConstellation(IEventParser parser, CancellationToken shutdownRequest) => new MixerConstellation(_loggerFactory, this, parser, shutdownRequest);
		public IMixerChat CreateChat(IMixerRestClient client, IEventParser parser, CancellationToken shutdownRequest) => new MixerChat(_loggerFactory, this, client, parser, shutdownRequest);
		public IJsonRpcWebSocket CreateJsonRpcWebSocket(ILogger logger, IEventParser parser) => new JsonRpcWebSocket(logger, this, parser);
		public IMixerRestClient CreateRestClient() => new MixerRestClient(_loggerFactory, new HttpClient());
	}
}
