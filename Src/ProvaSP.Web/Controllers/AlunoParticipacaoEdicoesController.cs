using ProvaSP.Data;
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
    
    public class AlunoParticipacaoEdicoesController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection formData)
        {
            string alu_matricula = formData["alu_matricula"].ToString();

            alu_matricula = alu_matricula.Replace("RA", "");

            string[] retorno = null;

            try
            {
                retorno = DataAluno.RecuperarParticipacoesEmEdicoes(alu_matricula);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed) { ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ") };
                throw new HttpResponseException(resp);
            }

            var retornoJson = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(retornoJson, Encoding.UTF8, "application/json");
            return response;
        }
    }
}
