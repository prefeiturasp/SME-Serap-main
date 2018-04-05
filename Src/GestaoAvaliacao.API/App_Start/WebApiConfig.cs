using System.Web.Http;
using System.Web.Http.Cors;

namespace GestaoAvaliacao.API
{
    public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.Indent = true;

			// Web API routes
			config.MapHttpAttributeRoutes();

			string origins = "*";

			var cors = new EnableCorsAttribute(origins, "*", "*");

			config.EnableCors(cors);

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

		}
	}
}
