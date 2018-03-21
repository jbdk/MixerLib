using System;

namespace MixerLib.Events
{
	public class FollowedEventArgs : EventArgs
	{
		public bool IsFollowing { get; internal set; }
		public string UserName { get; internal set; }
		public uint ChannelId { get; internal set; }
	}
}
