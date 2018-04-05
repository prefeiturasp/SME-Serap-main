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
    public class ResultadoRecuperarTurmasController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection formData)
        {
            string ResultadoNivel =formData["ResultadoNivel"].ToString();
            string Edicao = formData["Edicao"].ToString();
            int AreaConhecimentoID = int.Parse(formData["AreaConhecimentoID"].ToString());
            string AnoEscolar = formData["AnoEscolar"].ToString();
            string lista_esc_codigo = formData["lista_esc_codigo"].ToString();
            
            var listTurmas = new List<Turma>();

            try
            {
                listTurmas = DataTurma.RecuperarCodigoTurmas(ResultadoNivel, Edicao, AreaConhecimentoID, AnoEscolar, lista_esc_codigo);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ")
                };
                throw new HttpResponseException(resp);
            }

            var listaAlunosJson = Newtonsoft.Json.JsonConvert.SerializeObject(listTurmas);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(listaAlunosJson, Encoding.UTF8, "application/json");
            return response;

        }
    }
}
