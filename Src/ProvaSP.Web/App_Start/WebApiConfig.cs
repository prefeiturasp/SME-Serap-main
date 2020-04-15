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
            config.Routes.MapHttpRoute("ResultadoPorNivelDreDetalhandoEscolasDownloadCsvDosAlunos", "api/ResultadoPorNivel/download-csv-dre-detalhando-escolas-alunos", new { controller = "ResultadoPorNivelDownloadCsv", action = "DownloadCsvDreDetalhandoEscolasDosAlunos" });
            config.Routes.MapHttpRoute("ResultadoPorNivelDreDetalhandoEscolasDownloadCsvConsolidado", "api/ResultadoPorNivel/download-csv-dre-detalhando-escolas-consolidado", new { controller = "ResultadoPorNivelDownloadCsv", action = "DownloadCsvDreDetalhandoEscolasConsolidado" });

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
