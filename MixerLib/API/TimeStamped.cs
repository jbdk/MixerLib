using System;

namespace MixerLib
{
	public static partial class API
	{
		public class TimeStamped
		{
			/// <summary>The creation date of the object.</summary>
			public DateTime? CreatedAt { get; set; }

			/// <summary>The update date of the object.</summary>
			public DateTime? UpdatedAt { get; set; }

			/// <summary>The deletion date of the object.</summary>
			public DateTime? DeletedAt { get; set; }
		}
	}
}
