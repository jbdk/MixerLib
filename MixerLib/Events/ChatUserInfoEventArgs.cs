using System;
using System.Collections.Generic;

namespace MixerLib.Events
{
	public class ChatUserInfoEventArgs : EventArgs
	{
		/// <summary>Service specific properties (user roles etc)</summary>
		public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

		public uint ChannelId { get; internal set; }
		public uint UserId { get; internal set; }
		public string UserName { get; internal set; }
	}
}
