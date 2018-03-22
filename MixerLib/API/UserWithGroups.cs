using System.Collections.Generic;

namespace MixerLib
{
	public static partial class API
	{
		public class UserWithGroups : User
		{
			/// <summary>The groups of the user.</summary>
			public IReadOnlyList<UserGroup> Groups { get; set; }
		}
	}
}
