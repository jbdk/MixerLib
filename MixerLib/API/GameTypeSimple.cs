namespace MixerLib
{
	public static partial class API
	{
		/// <summary>
		/// Base game type.
		/// </summary>
		public class GameTypeSimple
		{
			/// <summary>The unique ID of the game type.</summary>
			public uint Id { get; set; }

			/// <summary>The name of the type.</summary>
			public string Name { get; set; }

			/// <summary>The url to the type's cover.</summary>
			public string CoverUrl { get; set; }

			/// <summary>The url to the type's background.</summary>
			public string BackgroundUrl { get; set; }
		}
	}
}
