using GestaoAvaliacao.LogFacade;
using System;

namespace OMRService
{
    public class AuthenticationService
	{
		private const string Route = "api/user/signin";

		public string Uri { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }

		public static string GetUri()
		{
			if (ConfigurationService.Instance != null && (!string.IsNullOrEmpty(ConfigurationService.Instance.Uri)))
            {
		        return ConfigurationService.Instance.Uri;
			}

			return null;
		}

		private void GetToken(AuthenticationService data)
		{
			try
			{
				
				string json = @"{""login"": """ + data.Login + @""",""password"": """ + data.Password + @"""}";

				data = ConfigurationService.Post<AuthenticationService>(data.Uri + Route, json, data);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
			}
		}

		public static AuthenticationService Connect()
		{
			AuthenticationService auth = new AuthenticationService();

			try
			{
				if (ConfigurationService.Instance != null)
				{
					if (!String.IsNullOrEmpty(ConfigurationService.Instance.Uri))
						auth.Uri = ConfigurationService.Instance.Uri;

					if (!String.IsNullOrEmpty(ConfigurationService.Instance.Login))
						auth.Login = ConfigurationService.Instance.Login;

					if (!String.IsNullOrEmpty(ConfigurationService.Instance.Password))
						auth.Password = ConfigurationService.Instance.Password;
				}

				string json = @"{""login"": """ + auth.Login + @""",""password"": """ + auth.Password + @"""}";

				AuthenticationService result = ConfigurationService.Post<AuthenticationService>(auth.Uri + Route, json, auth);

				if (result != null)
				{
					auth.AccessToken = result.AccessToken;
					auth.RefreshToken = result.RefreshToken;
				}
			}
			catch (Exception ex)
			{
				auth = null;
				LogFacade.SaveError(ex);
			}

			return auth;
		}
	}
}
