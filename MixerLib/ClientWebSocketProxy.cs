using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace MixerLib
{
	/// <summary>
	/// Encapsulates a real ClientWebSocket so it can be accessed using a interface
	/// </summary>
	internal class ClientWebSocketProxy : IClientWebSocketProxy
	{
		private readonly ClientWebSocket _ws;

		public ClientWebSocketProxy(bool isChat, IWebProxy proxy = null)
		{
			IsChat = isChat;
			_ws = new ClientWebSocket();
			_ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);
			_ws.Options.Proxy = proxy;
		}

		public bool IsChat { get; }
		public WebSocketCloseStatus? CloseStatus { get => _ws.CloseStatus; }
		public void SetRequestHeader(string name, string value) => _ws.Options.SetRequestHeader(name, value);
		public Task ConnectAsync(Uri uri, CancellationToken cancellationToken) => _ws.ConnectAsync(uri, cancellationToken);
		public void Dispose() => _ws.Dispose();
		public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken) => _ws.ReceiveAsync(buffer, cancellationToken);
		public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
			=> _ws.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
		public void ProcessingDone() { /* nop */ }
	}
}
