namespace MixerLib
{
	public static partial class API
	{
		/// <summary>
		/// The result from looking up a game type.
		/// </summary>
		public class GameTypeLookup : GameTypeSimple
		{
			/// <summary>Whether this game type is an exact match to the query.</summary>
			public bool Exact { get; set; }
		}
	}
}
