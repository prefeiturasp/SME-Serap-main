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
    public class NivelProficienciaCicloController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection formData)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                List<Proficiencia> lsProficiencia = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Proficiencia>>(formData["json"]);
                foreach (Proficiencia proficiencia in lsProficiencia)
                {
                    DataProficiencia.AtualizarProficienciaPorCiclo(proficiencia);
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