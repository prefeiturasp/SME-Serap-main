using ProvaSP.Data;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace ProvaSP.Web.Controllers
{
    public class UsuarioController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage RespondeuQuestionario(string edicao, int questionarioID, string usu_id)
        {
            var retorno = false;

            try
            {
                retorno = DataUsuario.RespondeuQuestionario(edicao, questionarioID, usu_id);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed) { ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ") };
                throw new HttpResponseException(resp);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(retorno.ToString(), Encoding.UTF8, "text/plain");
            return response;
        }       
    }
}