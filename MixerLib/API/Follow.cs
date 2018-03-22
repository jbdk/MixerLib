namespace MixerLib
{
	public static partial class API
	{
		/// <summary>
		/// Represents a follow relationship between a user and a channel.
		/// </summary>
		public class Follow : TimeStamped
		{
			/// <summary>The follower user id.</summary>
			public uint User { get; set; }

			/// <summary>The followee channel id.</summary>
			public uint Channel { get; set; }
		}
	}
}
