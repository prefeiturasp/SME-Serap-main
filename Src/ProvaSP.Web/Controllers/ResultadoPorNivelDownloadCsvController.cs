using Newtonsoft.Json;
using ProvaSP.Data;
using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace ProvaSP.Web.Controllers
{
    public class ResultadoPorNivelDownloadCsvController : ApiController
    {        
        [HttpPost]
        public HttpResponseMessage DownloadCsvDreDetalhandoEscolasDosAlunos(FormDataCollection formData)
        {
            var Edicao = Convert.ToString(formData["Edicao"]);
            var AreaConhecimentoID = int.Parse(Convert.ToString(formData["AreaConhecimentoID"]));
            var AnoEscolar = Convert.ToString(formData["AnoEscolar"]);
            var lista_uad_sigla = Convert.ToString(formData["lista_uad_sigla"]);

            var myByteArrayContent = DataResultado.ExportarDadosDreEscolasDosAlunos(Edicao,AreaConhecimentoID,AnoEscolar, lista_uad_sigla);

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(myByteArrayContent)
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "Proficiencia.csv"
            };

            return result;
        }

        [HttpPost]
        public HttpResponseMessage DownloadCsvDreDetalhandoEscolasConsolidado(FormDataCollection formData)
        {
            var Edicao = Convert.ToString(formData["Edicao"]);
            var AreaConhecimentoID = int.Parse(Convert.ToString(formData["AreaConhecimentoID"]));
            var AnoEscolar = Convert.ToString(formData["AnoEscolar"]);
            var lista_uad_sigla = Convert.ToString(formData["lista_uad_sigla"]);

            var myByteArrayContent = DataResultado.ExportarDadosDreEscolasConsolidados(Edicao, AreaConhecimentoID, AnoEscolar, lista_uad_sigla);

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(myByteArrayContent)
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "Proficiencia.csv"
            };

            return result;
        }
    }
}
