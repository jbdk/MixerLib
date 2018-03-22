namespace MixerLib
{
	public static partial class API
	{
		/// <summary>
		/// A GameType can be set on a channel and represents the title they are broadcasting.
		/// </summary>
		public class GameType : GameTypeSimple
		{
			/// <summary>The name of the parent type.</summary>
			public string Parent { get; set; }

			/// <summary>The description of the type.</summary>
			public string Description { get; set; }

			/// <summary>The source where the type has been imported from.</summary>
			public string Source { get; set; }

			/// <summary>Total amount of users watching this type of stream.</summary>
			public uint ViewersCurrent { get; set; }

			/// <summary>Amount of streams online with this type.</summary>
			public uint Online { get; set; }
		}
	}
}
