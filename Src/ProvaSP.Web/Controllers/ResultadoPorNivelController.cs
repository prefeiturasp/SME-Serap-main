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
            string Nivel = Convert.ToString(formData["Nivel"]);
            string Edicao = Convert.ToString(formData["Edicao"]);
            int AreaConhecimentoID = int.Parse(Convert.ToString(formData["AreaConhecimentoID"]));
            string AnoEscolar = Convert.ToString(formData["AnoEscolar"]);
            string Ciclo = Convert.ToString(formData["Ciclo"]);
            string lista_uad_sigla = Convert.ToString(formData["lista_uad_sigla"]);
            string lista_esc_codigo = Convert.ToString(formData["lista_esc_codigo"]);
            string lista_turmas = Convert.ToString(formData["lista_turmas"]);
            string lista_alu_matricula = Convert.ToString(formData["lista_alu_matricula"]);

            var resultado = new Resultado();

            try
            {
                if (Edicao == "ENTURMACAO_ATUAL")
                {
                    if (string.IsNullOrEmpty(Ciclo))
                    {
                        resultado = DataResultado.RecuperarResultadoEnturmacaoAtual(Edicao, AreaConhecimentoID, AnoEscolar, lista_turmas);
                    }
                    else
                    {
                        resultado = DataResultado.RecuperarResultadoCicloEnturmacaoAtual(Edicao, AreaConhecimentoID, Ciclo, lista_turmas);
                    }
                }
                else if (Nivel == "SME")
                {
                    if (string.IsNullOrEmpty(Ciclo))
                    {
                        resultado = DataResultado.RecuperarResultadoSME(Edicao, AreaConhecimentoID, AnoEscolar);
                    }
                    else
                    {
                        resultado = DataResultado.RecuperarResultadoCicloSME(Edicao, AreaConhecimentoID, Ciclo);
                    }
                }
                else if (Nivel == "DRE")
                {
                    if (string.IsNullOrEmpty(Ciclo))
                    {
                        resultado = DataResultado.RecuperarResultadoDRE(Edicao, AreaConhecimentoID, AnoEscolar, lista_uad_sigla);
                    }
                    else
                    {
                        resultado = DataResultado.RecuperarResultadoCicloDRE(Edicao, AreaConhecimentoID, Ciclo, lista_uad_sigla);
                    }
                }
                else if (Nivel == "ESCOLA")
                {
                    if (string.IsNullOrEmpty(Ciclo))
                    {
                        resultado = DataResultado.RecuperarResultadoEscola(Edicao, AreaConhecimentoID, AnoEscolar, lista_esc_codigo);
                    }
                    else
                    {
                        resultado = DataResultado.RecuperarResultadoCicloEscola(Edicao, AreaConhecimentoID, Ciclo, lista_esc_codigo);
                    }
                }
                else if (Nivel == "TURMA")
                {
                    if (string.IsNullOrEmpty(Ciclo))
                    {
                        resultado = DataResultado.RecuperarResultadoTurma(Edicao, AreaConhecimentoID, AnoEscolar, lista_esc_codigo, lista_turmas);
                    }
                    else
                    {
                        resultado = DataResultado.RecuperarResultadoCicloTurma(Edicao, AreaConhecimentoID, Ciclo, lista_esc_codigo, lista_turmas);
                    }
                }
                else if (Nivel == "ALUNO")
                {
                    bool IncluirSme_e_Dre = (formData["ExcluirSme_e_Dre"] == "1");
                    if (string.IsNullOrEmpty(Ciclo))
                    {
                        resultado = DataResultado.RecuperarResultadoAluno(Edicao, AreaConhecimentoID, AnoEscolar, lista_alu_matricula, IncluirSme_e_Dre);
                    }
                    else
                    {
                        resultado = DataResultado.RecuperarResultadoCicloAluno(Edicao, AreaConhecimentoID, Ciclo, lista_alu_matricula, IncluirSme_e_Dre);
                    }
                }

                if (resultado != null && resultado.Itens.Any())
                {
                    if (string.IsNullOrEmpty(Ciclo))
                    {
                        resultado.Proficiencias = DataProficiencia.PesquisarPorAnoLetivo(AnoEscolar);
                    }
                    else
                    {
                        resultado.Proficiencias = DataProficiencia.PesquisarPorCiclo(Ciclo);
                    }
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
