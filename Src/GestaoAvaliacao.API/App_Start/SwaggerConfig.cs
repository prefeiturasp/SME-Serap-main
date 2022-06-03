using System.Web.Http;
using WebActivatorEx;
using GestaoAvaliacao.API;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace GestaoAvaliacao.API
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c => {c.SingleApiVersion("v1", "GestaoAvaliacao.API");})
                .EnableSwaggerUi();
        }
    }
}
