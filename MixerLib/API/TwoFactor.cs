namespace MixerLib
{
	public static partial class API
	{
		public class TwoFactor
		{
			/// <summary>Indicates whether 2fa is enabled.</summary>
			public bool Enabled { get; set; }

			/// <summary>Indicates whether recovery codes have been viewed.</summary>
			public bool CodesViewed { get; set; }
		}
	}
}
