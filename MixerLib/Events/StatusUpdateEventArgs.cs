using System;

namespace MixerLib.Events
{
	public class StatusUpdateEventArgs : EventArgs
	{
		public uint ChannelId { get; set; }
		public int? NewFollowers { get; set; }
		public int? NewViewers { get; set; }
		public bool? IsOnline { get; set; }
	}
}
