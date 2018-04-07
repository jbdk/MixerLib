using System.IO;
using FluentAssertions;
using MixerLib;
using MixerLib.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Test
{
	public class Constellation : Base
	{
		[Fact]
		public void WillConnectAndJoin()
		{
			var sim = SimAnon.Value;
			var ws = sim.ConstellationWebSocket;
			using (var sut = new MixerClientInternal(ChannelName, LoggerFactory, sim))
			{
				sut.StartAsync().Wait(Simulator.TIMEOUT);

				var connectedAndJoined = ws.JoinedConstallation.Wait(Simulator.TIMEOUT);

				connectedAndJoined.Should().BeTrue();
				// {{"id": 1,"type": "method","method": "livesubscribe","params": {"events": ["channel:1234:update" ]}}}
				ws.LastPacket["method"].Should().NotBeNull();
				ws.LastPacket["method"].Value<string>().Should().Be("livesubscribe");
				var args = $"[{sim.ChannelInfo.Id},{sim.ChannelInfo.UserId},\"{sim.ChatAuthKey}\"]";
				ws.LastPacket["params"].Should().NotBeNull();
				ws.LastPacket["params"]["events"].Should().NotBeNull();
				var events = ws.LastPacket["params"]["events"].ToString(Formatting.None);
				events.Should().ContainAll($"channel:{sim.ChannelInfo.Id}:update");
			}
		}

		[Fact]
		public void WIllRetryFailedInitialConstellationConnection()
		{
			var sim = SimAuth.Value;
			var ws = sim.ConstellationWebSocket = new SimulatedClientWebSocket(false, true, failConnectCount: 4);
			using (var sut = new MixerClientInternal(ChannelName, LoggerFactory, sim))
			{
				var task = sut.StartAsync();
				var didRetry = ws.FailedConnectAttemptsReached.Wait(Simulator.TIMEOUT);
				ws.ConnectionAttempts.Should().BeGreaterOrEqualTo(4);
				didRetry.Should().BeTrue();
			}
		}

		[Fact]
		public void RaisesFollowerEvent()
		{
			var packet = BuildChannelStatusEvent("channel:1234:update", followers: 66);

			var sim = SimAnon.Value;
			var ws = sim.ConstellationWebSocket;
			using (var sut = new MixerClientInternal(ChannelName, LoggerFactory, sim))
			{
				sut.StartAsync().Wait(Simulator.TIMEOUT);
				using (var monitor = sut.Monitor())
				{
					ws.InjectPacket(packet);
					monitor.Should().Raise(nameof(sut.ChannelUpdate))
						  .WithArgs<ChannelUpdateEventArgs>(a => a.Channel.NumFollowers == 66 && a.Channel.ViewersCurrent == null && a.Channel.Online == null && a.ChannelId == sim.ChannelInfo.Id)
						  .WithSender(sut);
				}
			}
		}

		[Fact]
		public void RaiseViewersEvent()
		{
			var packet = BuildChannelStatusEvent("channel:1234:update", viewers: 735);

			var sim = SimAnon.Value;
			var ws = sim.ConstellationWebSocket;
			using (var sut = new MixerClientInternal(ChannelName, LoggerFactory, sim))
			{
				sut.StartAsync().Wait(Simulator.TIMEOUT);
				using (var monitor = sut.Monitor())
				{
					ws.InjectPacket(packet);
					monitor.Should().Raise(nameof(sut.ChannelUpdate))
						  .WithArgs<ChannelUpdateEventArgs>(a => a.Channel.NumFollowers == null && a.Channel.ViewersCurrent == 735 && a.Channel.Online == null && a.ChannelId == sim.ChannelInfo.Id)
						  .WithSender(sut);
				}
			}
		}

		[Fact]
		public void CanCombineEvent()
		{
			var packet = BuildChannelStatusEvent("channel:1234:update", followers: 22, viewers: 43, online: true);

			var sim = SimAnon.Value;
			var ws = sim.ConstellationWebSocket;
			using (var sut = new MixerClientInternal(ChannelName, LoggerFactory, sim))
			{
				sut.StartAsync().Wait(Simulator.TIMEOUT);
				using (var monitor = sut.Monitor())
				{
					ws.InjectPacket(packet);
					monitor.Should().Raise(nameof(sut.ChannelUpdate))
						  .WithArgs<ChannelUpdateEventArgs>(a => a.Channel.NumFollowers == 22 && a.Channel.ViewersCurrent == 43 && a.Channel.Online == true && a.ChannelId == sim.ChannelInfo.Id)
						  .WithSender(sut);
				}
			}
		}

		[Fact]
		public void WillReconnect()
		{
			var sim = SimAnon.Value;
			var ws = sim.ConstellationWebSocket;
			using (var sut = new MixerClientInternal(ChannelName, LoggerFactory, sim))
			{
				sut.StartAsync().Wait(Simulator.TIMEOUT);

				// Prepare new ClientWebSocket for consumption by client code, and dispose the old one
				sim.ConstellationWebSocket = new SimulatedClientWebSocket(false, false, Simulator.CONSTALLATION_WELCOME);
				ws.Dispose();
				ws = sim.ConstellationWebSocket;
				var connectedAndJoined = ws.JoinedConstallation.Wait(Simulator.TIMEOUT * 5);

				connectedAndJoined.Should().BeTrue();
			}
		}

		[Fact]
		public void AddCorrectHeaders()
		{
			var sim = SimAuth.Value;
			var ws = sim.ConstellationWebSocket;
			using (var sut = new MixerClientInternal(ChannelName, LoggerFactory, sim))
			{
				sut.Token = Token;
				sut.StartAsync().Wait(Simulator.TIMEOUT);

				ws.Headers.Should().Contain("Authorization", $"Bearer {Token}");
				ws.Headers.Should().Contain("x-is-bot", "true");
			}
		}

		[Fact]
		public void CanHandleRealDataDump()
		{
			var sim = SimAuth.Value;
			var ws = sim.ConstellationWebSocket;
			using (var sut = new MixerClientInternal(ChannelName, LoggerFactory, sim))
			{
				sut.StartAsync().Wait(Simulator.TIMEOUT);

				foreach (var line in File.ReadAllLines("Data/ConstellationDump.json"))
				{
					if (string.IsNullOrWhiteSpace(line))
						continue;
					ws.InjectPacket(line);
				}
			}
		}
	}
}
