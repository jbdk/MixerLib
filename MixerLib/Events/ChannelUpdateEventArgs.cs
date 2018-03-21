using System;

namespace MixerLib.Events
{
	public class ChannelUpdateEventArgs : EventArgs
	{
		public uint ChannelId { get; internal set; }
		public API.Channel Channel { get; internal set; }
	}
}
