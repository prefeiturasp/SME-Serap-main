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
using ProvaSP.Web.ViewModel;

namespace ProvaSP.Web.Controllers
{
    public class ProficienciaTurmaController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection formData)
        {
            int tur_id = int.Parse(formData["tur_id"].ToString());
            int AreaConhecimentoID = int.Parse(formData["AreaConhecimentoID"].ToString());

            var retorno = new ResultadoEnturmacao();

            try
            {
                retorno.Proficiencia = DataProficiencia.CalcularProficienciaAtualTurma(tur_id, AreaConhecimentoID);
                
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ")
                };
                throw new HttpResponseException(resp);
            }

            var retornoJson = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(retornoJson, Encoding.UTF8, "application/json");
            return response;

        }
    }
}