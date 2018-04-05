using GestaoAvaliacao.Util;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace GestaoAvaliacao.API.App_Start
{
    public static class ControllerApiExtension
	{
		public static ClaimsPrincipal Principal(this ApiController controller)
		{
			return controller.ControllerContext.Request.Properties[Constants.user_info] as ClaimsPrincipal;
		}

		public static Guid UserId(this ApiController controller)
		{
			return new Guid(ControllerApiExtension.Principal(controller).Claims.First(c => c.Type.Equals(ClaimTypes.Name)).Value);
		}

		public static Guid PesId(this ApiController controller)
		{
			return new Guid(ControllerApiExtension.Principal(controller).Claims.First(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value);
		}

		public static EnumSYS_Visao VisId(this ApiController controller)
		{
			return (EnumSYS_Visao)byte.Parse(ControllerApiExtension.Principal(controller).Claims.First(c => c.Type.Equals(ClaimTypes.Role)).Value);
		}

		public static Guid EntityId(this ApiController controller)
		{
			return new Guid(ControllerApiExtension.Principal(controller).Claims.First(c => c.Type.Equals(ClaimTypes.System)).Value);
		}

		public static string UserData(this ApiController controller)
		{ 
			return ControllerApiExtension.Principal(controller).Claims.First(c => c.Type.Equals(ClaimTypes.UserData)).Value;
		}
	}
}