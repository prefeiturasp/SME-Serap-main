using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace GestaoAvaliacao.API.App_Start
{
    public class CustomAuthorizationAttribute : AuthorizeAttribute
	{
		#region IsAuthorized
		protected override bool IsAuthorized(HttpActionContext actionContext)
		{
			try
			{
				var token = this.GetToken(actionContext);

				if (string.IsNullOrEmpty(token))
				{
					actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
					return false;
				}

				var ticket = GestaoAvaliacao.Util.JwtHelper.ValidateToken(token);

				if (ticket != null && ticket.Identity != null && ticket.Identity.IsAuthenticated)
				{
					actionContext.ControllerContext.Request.Properties[Constants.user_info] = new ClaimsPrincipal(ticket.Identity);
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
				return false;
			}
		}
		#endregion

		#region Private Methods
		private string GetToken(HttpActionContext actionContext)
		{
			var token = actionContext.ControllerContext.Request.Headers.FirstOrDefault(k => k.Key.Equals(Constants.access_token)).Value;
			return token.FirstOrDefault();
		}
		#endregion
	}
}