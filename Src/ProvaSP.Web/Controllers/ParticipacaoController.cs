using ProvaSP.Data;
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
    public class ParticipacaoController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Get(FormDataCollection formData)
        {
            string Nivel = Convert.ToString(formData["Nivel"]).ToUpper();
            string Edicao = Convert.ToString(formData["Edicao"]);
            string AnoEscolar = Convert.ToString(formData["AnoEscolar"]);
            string lista_uad_sigla = Convert.ToString(formData["lista_uad_sigla"]);
            string lista_esc_codigo = Convert.ToString(formData["lista_esc_codigo"]);
            string lista_turmas = Convert.ToString(formData["lista_turmas"]);

            var resultado = new List<Participacao>();

            try
            {
                if (Nivel == "SME")
                {
                    resultado = DataParticipacao.ParticipacaoSME(Edicao, AnoEscolar);
                }
                else if (Nivel == "DRE")
                {
                    resultado = DataParticipacao.ParticipacaoDRE(Edicao, AnoEscolar, lista_uad_sigla, true);
                }
                else if (Nivel == "ESCOLA")
                {
                    resultado = DataParticipacao.ParticipacaoEscola(Edicao, AnoEscolar, null, lista_esc_codigo, true);
                }
                else if (Nivel == "TURMA")
                {
                    resultado = DataParticipacao.ParticipacaoTurma(Edicao, AnoEscolar, lista_esc_codigo, lista_turmas);
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