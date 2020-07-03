using ProvaSP.Data.Data;
using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;

namespace ProvaSP.Web.Controllers
{
    [RoutePrefix("api/FatorAssociado")]
    public class FatorAssociadoController : ApiController
    {
        [HttpGet]
        [Route("GetQuestionario")]
        public HttpResponseMessage GetQuestionario(string edicao)
        {
            var resultado = new List<Questionario>();

            try
            {
                resultado = DataFatorAssociado.RetornarQuestionario(edicao);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ")
                };
                throw new HttpResponseException(resp);
            }

            var resultadoJson = Newtonsoft.Json.JsonConvert.SerializeObject(resultado);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(resultadoJson, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpGet]
        [Route("GetConstructo")]
        public HttpResponseMessage GetConstructo(string edicao, int? cicloId, int? anoEscolar, int questionarioId)
        {
            var resultado = new List<Constructo>();

            try
            {
                resultado = DataFatorAssociado.RetornarConstructo(edicao, cicloId, anoEscolar, questionarioId);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ")
                };
                throw new HttpResponseException(resp);
            }

            var resultadoJson = Newtonsoft.Json.JsonConvert.SerializeObject(resultado);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(resultadoJson, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpGet]
        [Route("GetFatorAssociado")]
        public HttpResponseMessage GetFatorAssociado(string edicao, int? cicloId, int questionarioId, int constructoId)
        {
            var resultado = new FatorAssociado();

            try
            {
                resultado = DataFatorAssociado.RetornarFatorAssociado(edicao, cicloId, questionarioId, constructoId);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ")
                };
                throw new HttpResponseException(resp);
            }

            var resultadoJson = Newtonsoft.Json.JsonConvert.SerializeObject(resultado);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(resultadoJson, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpPost]
        [Route("GetResultadoItem")]
        public HttpResponseMessage GetResultadoItem(FormDataCollection formData)
        {
            string nivel = Convert.ToString(formData["Nivel"]).ToUpper();
            string edicao = Convert.ToString(formData["Edicao"]);
            var cicloId = int.TryParse(formData["CicloId"].ToString(), out var auxInt) ? auxInt : default(int?);
            var anoEscolar = int.TryParse(formData["AnoEscolar"].ToString(), out auxInt) ? auxInt : default(int?);
            int questionarioId = Convert.ToInt32(formData["QuestionarioId"]);
            string constructo = Convert.ToString(formData["ConstructoId"]);
            string uad_sigla = Convert.ToString(formData["uad_sigla"]);
            string esc_codigo = Convert.ToString(formData["esc_codigo"]);

            var resultado = new List<VariavelItem>();

            try
            {
                if (nivel.ToUpper() == "SME")
                {
                    int.TryParse(constructo, out var constructoId);
                    resultado = DataFatorAssociado.RetornarResultadoItemSME(edicao, cicloId, anoEscolar, questionarioId, constructoId);
                }
                else if (nivel.ToUpper() == "DRE")
                {
                    resultado = DataFatorAssociado.RetornarResultadoItemDRE(edicao, cicloId, anoEscolar, questionarioId, uad_sigla);
                }
                else if (nivel.ToUpper() == "ESCOLA")
                {
                    resultado = DataFatorAssociado.RetornarResultadoItemEscola(edicao, cicloId, anoEscolar, questionarioId, uad_sigla, esc_codigo);
                }
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ")
                };
                throw new HttpResponseException(resp);
            }

            var resultadoJson = Newtonsoft.Json.JsonConvert.SerializeObject(resultado);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(resultadoJson, Encoding.UTF8, "application/json");
            return response;
        }
    }
}