using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using ProvaSP.Model.Entidades;
using ProvaSP.Data;

namespace ProvaSP.Web.Controllers
{
    public class ResultadoRecuperarAlunosController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection formData)
        {
            string Edicao = formData["Edicao"].ToString();
            int AreaConhecimentoID = int.Parse(formData["AreaConhecimentoID"].ToString());
            string AnoEscolar = formData["AnoEscolar"].ToString();
            string lista_esc_codigo = formData["lista_esc_codigo"].ToString();
            string lista_turmas = formData["lista_turmas"].ToString();


            var listaAlunos = new List<Aluno>();
            
            try
            {
                listaAlunos = DataAluno.RecuperarAlunos(Edicao, AreaConhecimentoID, AnoEscolar, lista_esc_codigo, lista_turmas);
            }
            catch(Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ")
                };
                throw new HttpResponseException(resp);
            }

            var listaAlunosJson = Newtonsoft.Json.JsonConvert.SerializeObject(listaAlunos);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(listaAlunosJson, Encoding.UTF8, "application/json");
            return response;

        }
    }
}
