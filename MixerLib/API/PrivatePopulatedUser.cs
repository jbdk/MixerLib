namespace MixerLib
{
	public static partial class API
	{
		/// <summary>
		/// A fully populater user with channel, preferences, groups and private details.
		/// </summary>
		public class PrivatePopulatedUser : PrivateUser
		{
			/// <summary>The users channel.</summary>
			public Channel Channel { get; set; }

			/// <summary>The global user groups.</summary>
			public UserGroup[] Groups { get; set; }

			/// <summary>The preferences the user has.</summary>
			public UserPreferences Preferences { get; set; }
		}
	}
}
