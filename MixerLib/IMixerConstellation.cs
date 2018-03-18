using System;
using System.Threading.Tasks;

// https://dev.mixer.com/reference/constellation/index.html

namespace MixerLib
{
	internal interface IMixerConstellation : IDisposable
	{
		Task ConnectAndJoinAsync(uint channelId);
	}
}
