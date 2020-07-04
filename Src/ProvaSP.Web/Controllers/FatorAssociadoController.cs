using ProvaSP.Data.Data;
using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int _segundoAno = 2;
        private const string _edicao2019 = "2019";
        private const int _questionarioDaFamiliaDoAluno = 6;
        private static IEnumerable<int> _questionariosQueNaoDevemSerExibidosNoFatoresAssociados = new List<int> { 3, 4, 6, 7, 8, 9 };
        private static IEnumerable<int> _questionariosQueNaoDevemSerExibidosNaCaracterizacaoDeFamilias = new List<int> { 3, 4, 7, 8, 9 };

        [HttpGet]
        [Route("GetQuestionario")]
        public HttpResponseMessage GetQuestionarioParaFatoresAssociados(string edicao)
        {
            var resultado = new List<Questionario>();

            try
            {
                resultado = DataFatorAssociado.RetornarQuestionario(edicao);
                resultado = resultado.Where(x => !_questionariosQueNaoDevemSerExibidosNoFatoresAssociados.Contains(x.QuestionarioID)).ToList();
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
        [Route("GetQuestionario")]
        public HttpResponseMessage GetQuestionarioParaCaracterizacaoDasFamilias(string edicao, int? anoEscolar)
        {
            var resultado = new List<Questionario>();

            try
            {
                resultado = DataFatorAssociado.RetornarQuestionario(edicao);

                if (edicao == _edicao2019 && anoEscolar.GetValueOrDefault() == _segundoAno)
                {
                    resultado = resultado.Where(x => x.QuestionarioID == _questionarioDaFamiliaDoAluno).ToList();
                }
                else
                {
                    resultado = resultado.Where(x => !_questionariosQueNaoDevemSerExibidosNaCaracterizacaoDeFamilias.Contains(x.QuestionarioID)).ToList();
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