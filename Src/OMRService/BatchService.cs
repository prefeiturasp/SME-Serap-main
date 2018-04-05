using GestaoAvaliacao.LogFacade;
using System;

namespace OMRService
{
    public class BatchService
	{
		private AuthenticationService Auth { get; set; }

		public BatchService()
		{
			Auth = AuthenticationService.Connect();
		}

		public OMRHttpStatus CreateBatch(string body)
		{
			try
			{
				string route = "api/aggregation/compose";

				if (Auth != null && !string.IsNullOrEmpty(Auth.AccessToken))
				{
					var result = ConfigurationService.Post(Auth.Uri + route, body, Auth);
					return new OMRHttpStatus((int)result.StatusCode);
				}
                else
                {
                    throw new Exception("Erro ao realizar a conexão com o OMR");
                }
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
			}

			return null;
		}
	}
}
