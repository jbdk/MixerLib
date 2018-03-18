using Newtonsoft.Json.Linq;

namespace MixerLib
{
	internal interface IEventParser
	{
		bool IsChat { get; }
		void Process(string eventName, JToken data);
	}
}
