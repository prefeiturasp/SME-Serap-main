using ProvaSP.Data;
using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace ProvaSP.Web.Controllers
{
    public class ConfiguracaoController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection formData)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                List<Configuracao> lsConfiguracao = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Configuracao>>(formData["json"]);
                foreach (Configuracao configuracao in lsConfiguracao)
                {
                    DataConfiguracao.AtualizarConfiguracao(configuracao);
                }
                response = this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                response = this.Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return response;
        }
    }
}