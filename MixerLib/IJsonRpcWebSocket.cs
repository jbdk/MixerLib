using System;
using System.Threading.Tasks;

namespace MixerLib
{
   internal interface IJsonRpcWebSocket
   {
      /// <summary>Is the connected user authenticated</summary>
      bool IsAuthenticated { get; }
      /// <summary>Roles of the authenticated user or null</summary>
      string[] Roles { get; }
      TimeSpan ReplyTimeout { get; set; }

      Task<bool> SendAsync(string method, params object[] args);
      Task<bool> TryConnectAsync(Func<string> resolveUrl, string accessToken, Func<Task> postConnectFunc);
      void Dispose();
   }
}
