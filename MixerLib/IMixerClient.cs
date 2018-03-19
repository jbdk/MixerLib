using System;
using MixerLib.Events;

namespace MixerLib
{
	public interface IMixerClient : IChatClient, IDisposable
	{
		uint? ChannelID { get; }
		string ChannnelName { get; }
		uint? UserId { get; }
		string UserName { get; }
		int CurrentFollowers { get; }
		int CurrentViewers { get; }

		IMixerRestClient RestClient { get; }

		event EventHandler<StatusUpdateEventArgs> StatusUpdate;
		event EventHandler<FollowedEventArgs> Followed;
		event EventHandler<HostedEventArgs> Hosted;
		event EventHandler<SubscribedEventArgs> Subscribed;
		event EventHandler<ResubscribedEventArgs> Resubscribed;

		TimeSpan? GetUptime();
	}
}
