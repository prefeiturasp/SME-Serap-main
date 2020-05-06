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
        public HttpResponseMessage DownloadCsvDreAlunos(FormDataCollection formData)
        {
            try
            {
                var Edicao = Convert.ToString(formData["Edicao"]);
                var AreaConhecimentoID = int.Parse(Convert.ToString(formData["AreaConhecimentoID"]));
                var AnoEscolar = Convert.ToString(formData["AnoEscolar"]);
                var lista_uad_sigla = Convert.ToString(formData["lista_uad_sigla"]);
                var Nivel = Convert.ToString(formData["Nivel"]);
                var Ciclo = Convert.ToString(formData["Ciclo"]);
                var lista_esc_codigo = Convert.ToString(formData["lista_esc_codigo"]);
                var lista_turmas = Convert.ToString(formData["lista_turmas"]);
                var lista_alu_matricula = Convert.ToString(formData["lista_alu_matricula"]);

                var myByteArrayContent = new byte[0];
                if (Nivel == "DRE")
                {
                    myByteArrayContent = DataResultado.ExportarDadosDreEscolasDosAlunos(Edicao, AreaConhecimentoID, AnoEscolar, lista_uad_sigla);
                }
                else if (Nivel == "ESCOLA")
                {
                    myByteArrayContent = DataResultado.ExportarDadosDreResultadoEscolaDosAlunos(Edicao, AreaConhecimentoID, AnoEscolar, lista_esc_codigo);
                }

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
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"{ex.Message ?? ex.StackTrace}")
                };
            }
        }

        [HttpPost]
        public HttpResponseMessage DownloadCsvDreMediaConsolidado(FormDataCollection formData)
        {
            try
            {
                var Edicao = Convert.ToString(formData["Edicao"]);
                var AreaConhecimentoID = int.Parse(Convert.ToString(formData["AreaConhecimentoID"]));
                var AnoEscolar = Convert.ToString(formData["AnoEscolar"]);
                var lista_uad_sigla = Convert.ToString(formData["lista_uad_sigla"]);
                var Nivel = Convert.ToString(formData["Nivel"]);
                var Ciclo = Convert.ToString(formData["Ciclo"]);
                var lista_esc_codigo = Convert.ToString(formData["lista_esc_codigo"]);
                var lista_turmas = Convert.ToString(formData["lista_turmas"]);
                var lista_alu_matricula = Convert.ToString(formData["lista_alu_matricula"]);

                var myByteArrayContent = new byte[0];
                if (Nivel == "DRE")
                {
                    myByteArrayContent = DataResultado.ExportarDadosDreEscolasConsolidados(Edicao, AreaConhecimentoID, AnoEscolar, lista_uad_sigla);
                }
                else if (Nivel == "ESCOLA")
                {
                    myByteArrayContent = DataResultado.ExportarDadosDreResultadoEscolaConsolidado(Edicao, AreaConhecimentoID, AnoEscolar, lista_esc_codigo);
                }

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
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"{ex.Message ?? ex.StackTrace}")
                };
            }
        }
    }
}
