using System;

namespace MixerLib.Events
{
	public class ResubscribedEventArgs : EventArgs
	{
		public string UserName { get; internal set; }
		public uint TotalMonths { get; internal set; }
		public DateTime Since { get; internal set; }
		public uint ChannelId { get; internal set; }
	}
}
