using System;
using MixerLib.Events;

namespace MixerLib
{
	public interface IMixerClient : IChatClient, IDisposable
	{
		/// <summary>The ID of the channel</summary>
		uint? ChannelID { get; }
		/// <summary>The name of the channel</summary>
		string ChannelName { get; }
		/// <summary>ID of the authorized user, or null if connected anonymously</summary>
		uint? UserId { get; }
		/// <summary>Name of the authorized user, or null if connected anonymously</summary>
		string UserName { get; }
		/// <summary>Current number of followers</summary>
		int CurrentFollowers { get; }
		/// <summary>Current number of viewers</summary>
		int CurrentViewers { get; }

		/// <summary>The rest client</summary>
		IMixerRestClient RestClient { get; }

		/// <summary>
		/// This event is raised when the channel state changes. This includes stream title, viewer/follower count changes.
		/// IMPORTANT: You need to check if the nullable properties on Channel actually has a value before using them
		/// </summary>
		event EventHandler<ChannelUpdateEventArgs> ChannelUpdate;
		/// <summary>Raised when somebody follows or unfollows you</summary>
		event EventHandler<FollowedEventArgs> Followed;
		/// <summary>Raised when somebody is hosting you</summary>
		event EventHandler<HostedEventArgs> Hosted;
		/// <summary>Raised when somebody subscribes to you</summary>
		event EventHandler<SubscribedEventArgs> Subscribed;
		/// <summary>Raised when somebody re-subscribes to you</summary>
		event EventHandler<ResubscribedEventArgs> Resubscribed;

		/// <summary>Get how long the stream has been live. Returns null if the channel is offline</summary>
		TimeSpan? GetUptime();
	}
}
