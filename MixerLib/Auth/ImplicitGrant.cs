using System.Threading.Tasks;

namespace MixerLib
{
	public static partial class Auth
	{
		public const string IMPLICIT_GRANT_METHOD = "ImplicitGrant";

		public class ImplicitGrant : IAuthorization, IAuthInternal
		{
			private readonly string _token;

			public string AuthMethod { get; } = IMPLICIT_GRANT_METHOD;

			public ImplicitGrant(string token)
			{
				if (string.IsNullOrEmpty(token))
					throw new System.ArgumentException("Can not be null or empty", nameof(token));
				_token = token;
			}

			Task IAuthInternal.AuthorizeAsync() => Task.CompletedTask;
			Task<bool> IAuthInternal.NeededRefresh() => Task.FromResult(false);
			string IAuthInternal.GetToken() => _token;
		}
	}
}
