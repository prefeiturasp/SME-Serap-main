using ProvaSP.Data;
using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Http;

namespace ProvaSP.Web.Controllers
{
    public class SincronizarQuestionarioController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection formData)
        {
            string jsonPreenchimentos = formData["json"].ToString();


            List<QuestionarioUsuario> preenchimentos = null;
            string ip = "";
            string userAgent = "";
            try
            {
                preenchimentos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<QuestionarioUsuario>>(jsonPreenchimentos);
                ip = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                userAgent = HttpContext.Current.Request.UserAgent;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            List<string> listaGuidsSincronizados = new List<string>();

            try
            {
                if (preenchimentos.Count>0)
                {
                    listaGuidsSincronizados = DataPreenchimentoDeQuestionario.Sincronizar(preenchimentos, ip, userAgent);
                }
            }
            catch(Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ")
                };
                throw new HttpResponseException(resp);
            }

            var listaGuidsSincronizadosJson = Newtonsoft.Json.JsonConvert.SerializeObject(listaGuidsSincronizados);

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(listaGuidsSincronizadosJson, Encoding.UTF8, "application/json");
            return response;
        }
    }
}
