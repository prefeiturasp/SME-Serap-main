using Newtonsoft.Json;
using ProvaSP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Hosting;
using System.Web.Http;

namespace ProvaSP.Web.Controllers
{
    public class RetornarAppJsonController : ApiController
    {            
        public HttpResponseMessage Get(string edicao, string usu_id)
        {
            var listaConfiguracao = DataConfiguracao.RetornarConfiguracao();
            var json = new StringBuilder();
            json.Append("{");
            foreach(var configuracao in listaConfiguracao)
            {
                json.Append(@"""");
                json.Append(configuracao.Chave);
                json.Append(@""": """);
                json.Append(configuracao.Valor);
                json.Append(@""",");
            }
            json.Append(@"""PossuiPerfilEdicao"": """);
            json.Append(DataUsuario.PossuiPerfil(edicao, usu_id));
            json.Append(@"""");
            json.Append("}");
            json = json.Replace(",}", "}");
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            return response;
        }
    }
}
