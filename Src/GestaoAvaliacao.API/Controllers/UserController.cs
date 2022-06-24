using GestaoAvaliacao.API.App_Start;
using GestaoAvaliacao.API.Models;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using OMRService;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace GestaoAvaliacao.API.Controllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	public class UserController : ApiController
	{
		[HttpPost]
		[Route("api/user/signin")]
		public HttpResponseMessage SignIn(LoginModel model)
		{
			try
			{
				if (model != null && !string.IsNullOrEmpty(model.Login) && !string.IsNullOrEmpty(model.Password))
				{
					var login = ConfigurationService.Instance.Login;
					var password = ConfigurationService.Instance.Password;

					if (model.Login.Equals(login) && model.Password.Equals(password))
					{
						var token = JwtHelper.CreateToken(userName: login);

						return Request.CreateResponse(HttpStatusCode.OK,
							new
							{
								AccessToken = token
							});
					}
					else
						return Request.CreateResponse(HttpStatusCode.NotFound, "Usuário/senha não encontrado.");
				}
				else
					return Request.CreateResponse(HttpStatusCode.NotFound, "Campos Usuário/password são obrigatórios.");
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, "Erro ao autenticar o usuário");
			}
		}

		[CustomAuthorizationAttribute]
		[Route("api/user/validatetoken")]
		[HttpPost]
		public HttpResponseMessage ValidateToken()
		{
			return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Autorizado" });
		}
	}
}
