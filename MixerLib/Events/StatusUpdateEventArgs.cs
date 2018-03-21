using System;

namespace MixerLib.Events
{
	public class StatusUpdateEventArgs : EventArgs
	{
		public uint ChannelId { get; internal set; }
		public int? NewFollowers { get; internal set; }
		public int? NewViewers { get; internal set; }
		public bool? IsOnline { get; internal set; }
	}
}
