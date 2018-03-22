namespace MixerLib
{
	public static partial class API
	{
		public class User : TimeStamped
		{
			/// <summary>The unique ID of the user.</summary>
			public uint Id { get; set; }

			/// <summary>The user's current level on Mixer, as determined by the number of experience points the user has.</summary>
			public uint Level { get; set; }

			/// <summary>Social links.</summary>
			public SocialInfo Social { get; set; }

			/// <summary>The user's name. This is unique on the site and is also their channel name.</summary>
			public string Username { get; set; }

			/// <summary>The user's email address. This is only shown if apropriate permissions are present.</summary>
			public string Email { get; set; }

			/// <summary>Indicates whether the user has verified their e-mail.</summary>
			public bool Verified { get; set; }

			/// <summary>The user's experience points.</summary>
			public uint Experience { get; set; }

			/// <summary>The amount of sparks the user has.</summary>
			public uint Sparks { get; set; }

			/// <summary>The user's profile URL.</summary>
			public string AvatarUrl { get; set; }

			/// <summary>The user's biography. This may contain HTML.</summary>
			public string Bio { get; set; }

			/// <summary>The ID of user's main team.</summary>
			public uint? PrimaryTeam { get; set; }
		}
	}
}
