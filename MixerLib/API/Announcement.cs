using Newtonsoft.Json.Linq;

namespace MixerLib
{
	public static partial class API
	{
		/// <summary>
		/// Announcements are triggered by Mixer staff and inform users of important information and news on Mixer.
		/// </summary>
		public class Announcement
		{
			/// <summary>Message to display in the toast / desktop notification.</summary>
			public string Message { get; set; }

			/// <summary>How long the message should appear for, defaults to 10 seconds.</summary>
			public int? Timeout { get; set; }

			/// <summary>
			/// Any sound that should be played with the announcement, like "whoosh", defaults to null.
			/// (whoosh,chimes,click,dudup,harp,heads-up,here-you-go,hi,incoming,ping,ring,urgent,powers)
			/// </summary>
			public string Sound { get; set; }

			/// <summary>Level of the toast to display, defaults to success.</summary>
			public string Level { get; set; }

			/// <summary>ID of confetti to display, or true to use the default.</summary>
			public JProperty Confetti { get; set; }
		}
	}
}
