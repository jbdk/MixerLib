namespace MixerLib
{
	public static partial class API
	{
		public class PrivateUser : User
		{
			/// <summary>The users password.</summary>
			public string Password { get; set; }

			/// <summary>The 2 factor authentication settings for this user.</summary>
			public TwoFactor TwoFactor { get; set; }
		}
	}
}
