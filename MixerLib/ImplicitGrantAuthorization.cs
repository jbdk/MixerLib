namespace MixerLib
{
	public class ImplicitGrantAuthorization : IAuthorization
	{
		private readonly string _token;

		public ImplicitGrantAuthorization(string token)
		{
			_token = token;
		}
	}
}
