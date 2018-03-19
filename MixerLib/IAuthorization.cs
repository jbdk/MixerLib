using System.Threading.Tasks;

namespace MixerLib
{
	public interface IAuthorization
	{
		string AuthMethod { get; }
	}

	internal interface IAuthInternal
	{
		Task AuthorizeAsync();
		Task<bool> NeededRefresh();
		string GetToken();
	}
}
