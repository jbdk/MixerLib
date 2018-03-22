namespace MixerLib
{
	public static partial class API
	{
		public class UserWithChannel : User
		{
			/// <summary>The user's channel.</summary>
			public Channel Channel { get; set; }
		}
	}
}
