using ProvaSP.Data;
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
    public class ResultadoPorNivelController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection formData)
        {

            string Nivel = formData["Nivel"].ToString();
            string Edicao = formData["Edicao"].ToString();
            int AreaConhecimentoID = int.Parse(formData["AreaConhecimentoID"].ToString());
            string AnoEscolar = formData["AnoEscolar"].ToString();
            string lista_uad_sigla = formData["lista_uad_sigla"].ToString();
            string lista_esc_codigo = formData["lista_esc_codigo"].ToString();
            string lista_turmas = formData["lista_turmas"].ToString();
            string lista_alu_matricula = formData["lista_alu_matricula"].ToString();

            var resultado = new Resultado();

            try
            {
                if (Edicao == "ENTURMACAO_ATUAL")
                {
                    resultado = DataResultado.RecuperarResultadoEnturmacaoAtual(Edicao, AreaConhecimentoID, AnoEscolar, lista_turmas);
                }
                else if (Nivel == "SME")
                {
                    resultado = DataResultado.RecuperarResultadoSME(Edicao, AreaConhecimentoID, AnoEscolar);
                }
                else if (Nivel == "DRE")
                {
                    resultado = DataResultado.RecuperarResultadoDRE(Edicao, AreaConhecimentoID, AnoEscolar, lista_uad_sigla);
                }
                else if (Nivel == "ESCOLA")
                {
                    resultado = DataResultado.RecuperarResultadoEscola(Edicao, AreaConhecimentoID, AnoEscolar, lista_esc_codigo);
                }
                else if (Nivel == "TURMA")
                {
                    resultado = DataResultado.RecuperarResultadoTurma(Edicao, AreaConhecimentoID, AnoEscolar, lista_esc_codigo, lista_turmas);
                }
                else if (Nivel == "ALUNO")
                {
                    bool IncluirSme_e_Dre = (formData["ExcluirSme_e_Dre"] == "1");
                    resultado = DataResultado.RecuperarResultadoAluno(Edicao, AreaConhecimentoID, AnoEscolar, lista_alu_matricula, IncluirSme_e_Dre);
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
