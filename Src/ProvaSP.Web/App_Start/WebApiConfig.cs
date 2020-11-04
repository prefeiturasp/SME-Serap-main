using ProvaSP.Web.Mappers.Config;
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

            MapUploadFileEndpoints(config);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            AutoMapperConfig.RegisterMappings();
        }

        private static void MapUploadFileEndpoints(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("UploadFilesCancelActiveBatches", "api/UploadRevistasEBoletins/CancelActiveBatches", new { controller = "UploadRevistasEBoletins", action = "CancelActiveBatches" });
            config.Routes.MapHttpRoute("UploadFilesAddBatch", "api/UploadRevistasEBoletins/AddBatch", new { controller = "UploadRevistasEBoletins", action = "AddBatch" });
            config.Routes.MapHttpRoute("UploadFilesStartBatch", "api/UploadRevistasEBoletins/StartBatch", new { controller = "UploadRevistasEBoletins", action = "StartBatch" });
            config.Routes.MapHttpRoute("UploadFilesUploadFile", "api/UploadRevistasEBoletins/UploadFile", new { controller = "UploadRevistasEBoletins", action = "UploadFile" });
            config.Routes.MapHttpRoute("UploadFilesCancelBatch", "api/UploadRevistasEBoletins/CancelBatch", new { controller = "UploadRevistasEBoletins", action = "CancelBatch" });
            config.Routes.MapHttpRoute("UploadFilesFinalizeBatch", "api/UploadRevistasEBoletins/FinalizeBatch", new { controller = "UploadRevistasEBoletins", action = "FinalizeBatch" });
            config.Routes.MapHttpRoute("UploadRevistasEBoletinsHistoricoGet", "api/UploadRevistasEBoletinsHistorico/Get", new { controller = "UploadRevistasEBoletinsHistorico", action = "Get" });
        }
    }
}