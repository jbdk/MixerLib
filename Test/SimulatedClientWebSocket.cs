using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MixerLib;
using Newtonsoft.Json.Linq;

namespace Test
{
	public class SimulatedClientWebSocket : IClientWebSocketProxy
	{
		static int _connectionId = 0;

		public WebSocketCloseStatus? CloseStatus { get; internal set; }
		public bool IsChat { get; }
		public ManualResetEventSlim JoinedChat { get; } = new ManualResetEventSlim();
		public ManualResetEventSlim JoinedConstallation { get; } = new ManualResetEventSlim();
		public JToken LastPacket { get; private set; }
		public int? LastId { get; private set; }

		public string ConnectUrl { get; set; }
		public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

		readonly ManualResetEventSlim _clientDoneProcessing = new ManualResetEventSlim();
		readonly string _welcomeMessage;
		bool _isFirstSend = true;
		readonly bool _isAuthenticated;
		readonly int _myId;
		readonly TcpClient _injectSocket;
		TcpClient _serverSocket;
		Random _random = new Random();
		private readonly int _failConnectCount;
		public int ConnectionAttempts { get; internal set; }
		public ManualResetEventSlim FailedConnectAttemptsReached { get; } = new ManualResetEventSlim();

		public SimulatedClientWebSocket(bool isChat, bool isAuthenticated, string welcomeMessage = null, int failConnectCount = 0)
		{
			IsChat = isChat;
			_welcomeMessage = welcomeMessage;
			_isAuthenticated = isAuthenticated;
			_failConnectCount = failConnectCount;

			_myId = Interlocked.Increment(ref _connectionId);

			// Setup internal loopback socket connection
			int port = 10000 + _random.Next(25000);
			var server = new TcpListener(IPAddress.Loopback, port);
			server.Start();
			var t = server.AcceptTcpClientAsync();
			_injectSocket = new TcpClient();
			_injectSocket.NoDelay = true;
			_injectSocket.Connect(IPAddress.Loopback, port);
			if (t.Wait(Simulator.TIMEOUT))
			{
				_serverSocket = t.Result;
				_serverSocket.NoDelay = true;
			}
			server.Stop();
		}

		virtual public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
		{
			ConnectionAttempts++;

			CloseStatus = null;
			ConnectUrl = uri.ToString();

			if (_failConnectCount != 0)
			{
				await Task.Yield();
				if (ConnectionAttempts >= _failConnectCount)
					FailedConnectAttemptsReached.Set();
				throw new Exception("TEST TEST TEST");
			}

			// Enqueue welcome messages
			if (!string.IsNullOrEmpty(_welcomeMessage))
			{
				var bytes = Encoding.UTF8.GetBytes(_welcomeMessage);
				_ = _injectSocket.Client.SendAsync(bytes, SocketFlags.None);
			}

			Log($"SimWebSocket CONNECTED {uri}");
		}

		virtual public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
		{
			if (CloseStatus.HasValue)
				throw new WebSocketException("WebSocket is closed");

			int n = await _serverSocket.Client.ReceiveAsync(buffer, SocketFlags.None);
			_serverSocket.Client.Send(new byte[] { 1 });
			if (cancellationToken.IsCancellationRequested)
				return null;
			if (n == 0)
				throw new WebSocketException("WebSocket closed");

			return new WebSocketReceiveResult(n, WebSocketMessageType.Text, true);
		}

		virtual public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			var json = Encoding.UTF8.GetString(buffer.Array, 0, buffer.Count);
			LastPacket = JToken.Parse(json);
			LastId = LastPacket["id"]?.Value<int>();

			if (_isFirstSend)
			{
				_isFirstSend = false;
				if (IsChat)
				{
					JoinedChat.Set();
					if (_isAuthenticated)
					{
						InjectPacket("{'type':'reply','error':null,'id':<MSGID>,'data':{'authenticated':true,'roles':[]}}"
								  .Replace("'", "\"")
								  .Replace("<MSGID>", LastId.ToString())
						);
					}
					else
					{
						InjectPacket("{'type':'reply','error':null,'id':<MSGID>,'data':null}"
								  .Replace("'", "\"")
								  .Replace("<MSGID>", LastId.ToString())
						);
					}
				}
				else
				{
					JoinedConstallation.Set();
					if (_isAuthenticated)
						InjectPacket("{'type':'reply','error':null,'id':<MSGID>,'data':{'authenticated':true,'roles':['Owner','User']}}".Replace("'", "\"").Replace("<MSGID>", LastId.ToString()));
					else
						InjectPacket("{'id':<MSGID>,'type':'reply','result':null,'error':null}".Replace("'", "\"").Replace("<MSGID>", LastId.ToString()));
				}
			}

			return Task.CompletedTask;
		}

		virtual public void SetRequestHeader(string name, string value)
		{
			Headers.Remove(name); // Remove if its already there
			Headers.Add(name, value);
		}

		public void InjectPacket(string json, bool waitForReply = false)
		{
			if (!_injectSocket.Connected)
			{
				Log("Cant inject packet, not connected!");
				return;
			}

			_clientDoneProcessing.Reset();

			var bytes = Encoding.UTF8.GetBytes(json);
			_injectSocket.Client.Send(bytes);
			if (waitForReply)
				_injectSocket.Client.ReceiveAsync(new ArraySegment<byte>(new byte[10]), SocketFlags.None).Wait(Simulator.TIMEOUT);

			// Wait until client code has processed the message (its back waiting for more)
			var timeout = Debugger.IsAttached ? Timeout.Infinite : Simulator.TIMEOUT;
			_clientDoneProcessing.Wait(timeout);
		}

		private void Log(string format, params object[] args)
		{
			// Output?.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.ffff")} - {string.Format(format, args)}");
		}

		public void ProcessingDone() => _clientDoneProcessing.Set();

		public void Dispose()
		{
			Log("SimWebSocket Disposing!");
			CloseStatus = WebSocketCloseStatus.NormalClosure;
			_injectSocket.Dispose();
			_serverSocket.Dispose();
		}
	}
}
