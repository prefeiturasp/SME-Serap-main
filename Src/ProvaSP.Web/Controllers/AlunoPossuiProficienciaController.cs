using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProvaSP.Data;
using System.Text;
using Newtonsoft.Json;

namespace ProvaSP.Web.Controllers
{
    public class AlunoPossuiProficienciaController : ApiController
    {
        public HttpResponseMessage Get(string alu_matricula)
        {
            
            var sbRetorno = new StringBuilder();
            sbRetorno.Append("{ possuiProficiencia:");
            sbRetorno.Append(DataProficiencia.AlunoPossuiProficiencia(alu_matricula).ToString().ToLower());
            sbRetorno.Append("}");

            object jsonObject = JsonConvert.DeserializeObject(sbRetorno.ToString());
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            return response;
            
        }
    }
}
