using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace MixerLib.Helpers
{
	internal class JsonContent : StringContent
	{
		public JsonContent(object data)
			: base(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
		{
		}
	}
}
