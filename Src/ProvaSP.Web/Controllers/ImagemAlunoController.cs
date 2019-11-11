using ProvaSP.Data;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace ProvaSP.Web.Controllers
{
    public class ImagemAlunoController : ApiController
    {
        public HttpResponseMessage Get(string Edicao, byte AreaConhecimentoId, string alu_matricula)
        {
            var resultado = DataImagemAluno.SelecionarPorEdicaoAreaConhecimentoAluno(Edicao, AreaConhecimentoId, alu_matricula);

            var resultadoJson = Newtonsoft.Json.JsonConvert.SerializeObject(resultado);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(resultadoJson, Encoding.UTF8, "application/json");
            return response;
        }
    }
}