using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using J = Newtonsoft.Json.JsonPropertyAttribute;

namespace MixerLib
{
	public static class API
	{
		//
		// https://mixer.com/api/v1/channels/<ChannelId>/manifest.light2
		//
		public class ChannelManifest2
		{
			public DateTime Now { get; set; }
			public bool IsTestStream { get; set; }
			public DateTime StartedAt { get; set; }
			public string AccessKey { get; set; }
			public string HlsSrc { get; set; }
		}

		public class Preferences
		{
		}

		public class Social
		{
		}

		public class Group
		{
			public uint Id { get; set; }
			public string Name { get; set; }
		}

		public class TimeStamped
		{
			/// <summary>The creation date of the object.</summary>
			public DateTime? CreatedAt { get; set; }
			/// <summary>The update date of the object.</summary>
			public DateTime? UpdatedAt { get; set; }
			/// <summary>The deletion date of the object.</summary>
			public DateTime? DeletedAt { get; set; }
		}

		public class User : TimeStamped
		{
			public uint Level { get; set; }
			public Social Social { get; set; }
			public uint Id { get; set; }
			public string Username { get; set; }
			public bool Verified { get; set; }
			public uint Experience { get; set; }
			public uint Sparks { get; set; }
			public string AvatarUrl { get; set; }
			//public object Bio { get; set; }
			//public object PrimaryTeam { get; set; }
			public IList<Group> Groups { get; set; }
		}

		public class Channel : TimeStamped
		{
			/// <summary>The unique ID of the channel.</summary>
			public uint? Id { get; set; }
			/// <summary>The ID of the user owning the channel.</summary>
			public uint? UserId { get; set; }
			/// <summary>The name and url of the channel.</summary>
			public string Token { get; set; }
			/// <summary>Indicates if the channel is active.</summary>
			public bool? Online { get; set; }
			/// <summary>True if featureLevel is > 0.</summary>
			public bool? Featured { get; set; }
			/// <summary>The featured level for this channel. Its value controls the position and order of channels in the featured carousel.</summary>
			public uint? FeatureLevel { get; set; }
			/// <summary>Indicates if the channel is partnered.</summary>
			public bool? Partnered { get; set; }
			/// <summary>The id of the transcoding profile.</summary>
			public uint? TranscodingProfileId { get; set; }
			/// <summary>Indicates if the channel is suspended.</summary>
			public bool? Suspended { get; set; }
			/// <summary>The title of the channel.</summary>
			public string Name { get; set; }
			/// <summary>The target audience of the channel. (family,teen,18+)</summary>
			public string Audience { get; set; }
			/// <summary>Amount of unique viewers that ever viewed this channel.</summary>
			public uint? ViewersTotal { get; set; }
			/// <summary>Amount of current viewers.</summary>
			public uint? ViewersCurrent { get; set; }
			/// <summary>Amount of followers.</summary>
			public uint? NumFollowers { get; set; }
			/// <summary>The description of the channel, can contain HTML.</summary>
			public string Description { get; set; }
			/// <summary>The ID of the game type.</summary>
			[J(DefaultValueHandling = DefaultValueHandling.Ignore)]
			public uint? TypeId { get; set; }
			/// <summary>Indicates if that channel is interactive.</summary>
			public bool? Interactive { get; set; }
			/// <summary>The ID of the interactive game used.</summary>
			[J(DefaultValueHandling = DefaultValueHandling.Ignore)]
			public uint? InteractiveGameId { get; set; }
			/// <summary>The ftl stream id.</summary>
			public int? Ftl { get; set; }
			/// <summary>Indicates if the channel has vod saved.</summary>
			public bool? HasVod { get; set; }
			/// <summary>ISO 639 language id.</summary>
			[J(DefaultValueHandling = DefaultValueHandling.Ignore)]
			public string LanguageId { get; set; }
			/// <summary>The ID of the cover resource.</summary>
			[J(DefaultValueHandling = DefaultValueHandling.Ignore)]
			public uint? CoverId { get; set; }
			/// <summary>The resource ID of the thumbnail.</summary>
			[J(DefaultValueHandling = DefaultValueHandling.Ignore)]
			public uint? ThumbnailId { get; set; }
			/// <summary>The resource ID of the subscriber badge.</summary>
			public uint? BadgeId { get; set; }
			/// <summary>The URL of the the banner image.</summary>
			public string BannerUrl { get; set; }
			/// <summary>The ID of the hostee channel.</summary>
			public uint? HosteeId { get; set; }
			/// <summary>Indicates if the channel has transcodes enabled.</summary>
			public bool? HasTranscodes { get; set; }
			/// <summary>Indicates if the channel has vod recording enabled.</summary>
			public bool? VodsEnabled { get; set; }
			/// <summary>The costream that the channel is in, if any.</summary>
			[J(DefaultValueHandling = DefaultValueHandling.Ignore)]
			public Guid? CostreamId { get; set; }
		}

		public class UserWithChannel : User
		{
			public Channel Channel { get; set; }
		}

		//
		// https://mixer.com/api/v1/chats/<ChannelId>
		//

		public class Chats
		{
			public IList<string> Roles { get; set; }
			public string Authkey { get; set; }
			public IList<string> Permissions { get; set; }
			public IList<string> Endpoints { get; set; }
			public bool IsLoadShed { get; set; }
		}

		public class GameTypeSimple
		{
			public uint Id { get; set; }
			public string Name { get; set; }
			public string CoverUrl { get; set; }
			public string BackgroundUrl { get; set; }
		}
	}
}
