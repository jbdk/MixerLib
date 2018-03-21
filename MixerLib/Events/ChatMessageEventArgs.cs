using System;

namespace MixerLib.Events
{
	public class ChatMessageEventArgs : EventArgs
	{
		public uint ChannelId { get; internal set; }
		public uint UserId { get; internal set; }
		public string UserName { get; internal set; }
		public bool IsWhisper { get; internal set; }
		public bool IsOwner { get; internal set; }
		public bool IsModerator { get; internal set; }
		public string Message { get; internal set; }

		public string[] Roles { get; internal set; }
		public string Avatar { get; internal set; }
		public uint UserLevel { get; internal set; }
	}
}
