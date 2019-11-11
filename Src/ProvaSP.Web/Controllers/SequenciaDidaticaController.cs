using ProvaSP.Data.Data;
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
    [RoutePrefix("api/SequenciaDidatica")]
    public class SequenciaDidaticaController : ApiController
    {
        [HttpGet]
        [Route("GetCorte")]
        public HttpResponseMessage GetCorte()
        {
            var resultado = new List<Corte>();

            try
            {
                resultado = DataSequenciaDidatica.SelecionarCorte();
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ")
                };
                throw new HttpResponseException(resp);
            }

            var resultadoJson = Newtonsoft.Json.JsonConvert.SerializeObject(resultado);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(resultadoJson, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpGet]
        [Route("GetSequenciaDidatica")]
        public HttpResponseMessage GetSequenciaDidatica(string edicao, int anoEscolar, int areaConhecimentoId, int corteId)
        {
            var resultado = new SequenciaDidatica();

            try
            {
                resultado = DataSequenciaDidatica.BuscarSequenciaDidatica(edicao, anoEscolar, areaConhecimentoId, corteId);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ")
                };
                throw new HttpResponseException(resp);
            }

            var resultadoJson = Newtonsoft.Json.JsonConvert.SerializeObject(resultado);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(resultadoJson, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpGet]
        [Route("SelecionarSequenciasDidaticas")]
        public HttpResponseMessage SelecionarSequenciasDidaticas(string edicao, int cicloId, int areaConhecimentoId)
        {
            var resultado = new List<Corte>();

            try
            {
                resultado = DataSequenciaDidatica.SelecionarSequenciaDidatica(edicao, cicloId, areaConhecimentoId);
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    ReasonPhrase = ex.Message.Replace(Environment.NewLine, " ")
                };
                throw new HttpResponseException(resp);
            }

            var resultadoJson = Newtonsoft.Json.JsonConvert.SerializeObject(resultado);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(resultadoJson, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpPost]
        [Route("Salvar")]
        public HttpResponseMessage Post(FormDataCollection formData)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                SequenciaDidatica sequenciaDidatica = Newtonsoft.Json.JsonConvert.DeserializeObject<SequenciaDidatica>(formData["json"]);
                DataSequenciaDidatica.SalvarSequenciaDidatica(sequenciaDidatica);

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