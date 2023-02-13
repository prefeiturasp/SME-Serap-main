using System.Web.Http;
using WebActivatorEx;
using GestaoAvaliacao.API;
using Swashbuckle.Application;
using GestaoAvaliacao.API.Middleware;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace GestaoAvaliacao.API
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "GestaoAvaliacao.API");
                    c.OperationFilter<ProducesOperationFilter>();
                    //c.Schemes(new[] { "https" });
                })
                .EnableSwaggerUi();
        }
    }
}
