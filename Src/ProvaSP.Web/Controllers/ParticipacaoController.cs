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
            string nivel = Convert.ToString(formData["Nivel"]).ToUpper();
            string edicao = Convert.ToString(formData["Edicao"]);
            int.TryParse(formData["AreaConhecimento"], out int areaConhecimento);
            string anoEscolar = Convert.ToString(formData["AnoEscolar"]);
            string lista_uad_sigla = Convert.ToString(formData["lista_uad_sigla"]);
            string lista_esc_codigo = Convert.ToString(formData["lista_esc_codigo"]);
            string lista_turmas = Convert.ToString(formData["lista_turmas"]);

            var resultado = new List<Participacao>();

            try
            {
                if (nivel == "SME")
                {
                    resultado = DataParticipacao.ParticipacaoSME(edicao, areaConhecimento, anoEscolar);
                }
                else if (nivel == "DRE")
                {
                    resultado = DataParticipacao.ParticipacaoDRE(edicao, areaConhecimento, anoEscolar, lista_uad_sigla, true);
                }
                else if (nivel == "ESCOLA")
                {
                    resultado = DataParticipacao.ParticipacaoEscola(edicao, areaConhecimento, anoEscolar, null, lista_esc_codigo, true);
                }
                else if (nivel == "TURMA")
                {
                    resultado = DataParticipacao.ParticipacaoTurma(edicao, areaConhecimento, anoEscolar, lista_esc_codigo, lista_turmas, true);
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