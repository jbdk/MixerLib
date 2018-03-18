using System;

namespace MixerLib.Events
{
	public class ChatMessageEventArgs : EventArgs
	{
		public uint ChannelId { get; set; }
		public uint UserId { get; set; }
		public string UserName { get; set; }
		public bool IsWhisper { get; set; }
		public bool IsOwner { get; set; }
		public bool IsModerator { get; set; }
		public string Message { get; set; }

		public string[] Roles { get; set; }
		public string Avatar { get; set; }
		public uint UserLevel { get; set; }
	}
}
