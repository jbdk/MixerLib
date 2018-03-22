using System.Collections.Generic;

namespace MixerLib
{
	public static partial class API
	{
		//
		// https://mixer.com/api/v1/chats/<ChannelId>
		//

		public class Chats
		{
			/// <summary>List of chat server websocket urls.</summary>
			public IList<string> Endpoints { get; set; }

			/// <summary>The auth key required to join a channel's chatroom.</summary>
			public string Authkey { get; set; }

			/// <summary>List of roles he user will have once joined.</summary>
			public IList<string> Roles { get; set; }

			/// <summary>List of permissions the user will have once joined.</summary>
			public IList<string> Permissions { get; set; }

			/// <summary>Whether the channel is in load shedding mode.</summary>
			public bool IsLoadShed { get; set; }
		}
	}
}
