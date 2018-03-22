namespace MixerLib
{
	public static partial class API
	{
		/// <summary>
		/// The social information for a channel.
		/// </summary>
		public class SocialInfo
		{
			/// <summary>Twitter profile URL.</summary>
			public string Twitter { get; set; }

			/// <summary>Facebook profile URL.</summary>
			public string Facebook { get; set; }

			/// <summary>Youtube profile URL.</summary>
			public string Youtube { get; set; }

			/// <summary>Player.me profile URL.</summary>
			public string Player { get; set; }

			/// <summary>Discord username and tag.</summary>
			public string Discord { get; set; }

			/// <summary>A list of social keys which have been verified via linking the Mixer account with the account on the corresponding external service.</summary>
			public string[] Verified { get; set; }
		}
	}
}
