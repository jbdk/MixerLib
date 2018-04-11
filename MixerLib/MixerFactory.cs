using System;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace MixerLib
{
	internal class MixerFactory : IMixerFactory
	{
		private readonly ILoggerFactory _loggerFactory;

		public IWebProxy Proxy { get; set; }

		public MixerFactory(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
		}

		public IClientWebSocketProxy CreateClientWebSocket(bool isChat) => new ClientWebSocketProxy(isChat, Proxy);
		public IMixerConstellation CreateConstellation(IEventParser parser, CancellationToken shutdownRequest) => new MixerConstellation(_loggerFactory, this, parser, shutdownRequest);
		public IMixerChat CreateChat(IMixerRestClient client, IEventParser parser, CancellationToken shutdownRequest) => new MixerChat(_loggerFactory, this, client, parser, shutdownRequest);
		public IJsonRpcWebSocket CreateJsonRpcWebSocket(ILogger logger, IEventParser parser) => new JsonRpcWebSocket(logger, this, parser);
		public IMixerRestClient CreateRestClient()
		{
			HttpClient client;
			if (Proxy != null)
			{
				var handler = new HttpClientHandler {
					Proxy = Proxy,
					UseProxy = true
				};
				client = new HttpClient(handler);
			}
			else
			{
				client = new HttpClient();
			}
			client.Timeout = TimeSpan.FromSeconds(MixerRestClient.TIMEOUT_IN_SECONDS);

			return new MixerRestClient(_loggerFactory, client);
		}
	}
}
