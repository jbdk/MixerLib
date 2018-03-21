using System;

namespace MixerLib.Events
{
	public class SubscribedEventArgs : EventArgs
	{
		public string UserName { get; internal set; }
		public uint ChannelId { get; internal set; }
	}
}
