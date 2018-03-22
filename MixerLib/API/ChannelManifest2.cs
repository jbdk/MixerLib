using System;

namespace MixerLib
{
	public static partial class API
	{
		//
		// https://mixer.com/api/v1/channels/<ChannelId>/manifest.light2
		//
		public class ChannelManifest2
		{
			/// <summary>A URL for the HLS manifest for this channel.</summary>
			public string HlsSrc { get; set; }

			/// <summary>A URL for the FTL manifest of this channel.</summary>
			public string FtlSrc { get; set; }

			/// <summary>The UTC timestamp that the request was made.</summary>
			public DateTime Now { get; set; }

			/// <summary>The UTC timestamp for when the stream started.</summary>
			public DateTime StartedAt { get; set; }

			//public bool IsTestStream { get; set; }
			//public string AccessKey { get; set; }
		}
	}
}
