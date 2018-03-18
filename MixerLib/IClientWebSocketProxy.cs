using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace MixerLib
{
	internal interface IClientWebSocketProxy : IDisposable
	{
		bool IsChat { get; }
		WebSocketCloseStatus? CloseStatus { get; }
		void SetRequestHeader(string name, string value);
		Task ConnectAsync(Uri uri, CancellationToken cancellationToken);
		/// <summary>
		/// Receives the next packet from the websocket. Task completes when there are data available, or the websocket was closed
		/// IMPORTANT: Call ProcessingDone() after processing the received data
		/// </summary>
		Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);
		Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);
		/// <summary>Signal that client code is done processing the received packet. This is to help testing work flow!</summary>
		void ProcessingDone();
	}
}
