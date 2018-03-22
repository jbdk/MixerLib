using System;
using Newtonsoft.Json;
using J = Newtonsoft.Json.JsonPropertyAttribute;

namespace MixerLib
{
	public static partial class API
	{
		/// <summary>
		/// A single channel within Mixer. Each channel is owned by a user, and a channel can be broadcasted to.
		/// </summary>
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
	}
}
