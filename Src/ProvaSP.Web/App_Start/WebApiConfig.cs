using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ProvaSP.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Routes.MapHttpRoute("ResultadoPorNivelDownloadCsvDosAlunos", "api/ResultadoPorNivel/download-csv-dre-alunos", new { controller = "ResultadoPorNivelDownloadCsv", action = "DownloadCsvDreAlunos" });
            config.Routes.MapHttpRoute("ResultadoPorNivelDownloadCsvDreMediaConsolidado", "api/ResultadoPorNivel/download-csv-dre-media-consolidado", new { controller = "ResultadoPorNivelDownloadCsv", action = "DownloadCsvDreMediaConsolidado" });

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
