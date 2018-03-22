namespace MixerLib
{
	public static partial class API
	{
		/// <summary>
		/// Augmented regular channel with additional data.
		/// </summary>
		public class ChannelAdvanced : Channel
		{
			/// <summary>A nested type showing information about this channel's currently selected type.</summary>
			public GameType Type { get; set; }

			/// <summary>This channel's owner.</summary>
			public UserWithGroups User { get; set; }
		}
	}
}
