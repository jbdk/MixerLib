using System;
using System.Collections.Generic;

namespace MixerLib.Events
{
	public class ChatUserInfoEventArgs : EventArgs
	{
		/// <summary>Service specific properties (user roles etc)</summary>
		public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

		public uint ChannelId { get; set; }
		public uint UserId { get; set; }
		public string UserName { get; set; }
	}
}
