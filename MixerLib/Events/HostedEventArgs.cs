using System;

namespace MixerLib.Events
{
	public class HostedEventArgs : EventArgs
	{
		public bool IsHosting { get; internal set; }
		public string HosterName { get; internal set; }
		public uint CurrentViewers { get; internal set; }
		public uint ChannelId { get; internal set; }
	}
}
