using System.Threading;
using Microsoft.Extensions.Logging;

namespace MixerLib
{
   internal interface IMixerFactory
   {
      IClientWebSocketProxy CreateClientWebSocket(bool isChat);
      IMixerConstellation CreateConstellation(IEventParser parser, CancellationToken shutdownRequest);
      IMixerChat CreateChat(IMixerRestClient client, IEventParser parser, CancellationToken shutdownRequest);
      IJsonRpcWebSocket CreateJsonRpcWebSocket(ILogger logger, IEventParser parser);
      IMixerRestClient CreateRestClient();
   }
}
