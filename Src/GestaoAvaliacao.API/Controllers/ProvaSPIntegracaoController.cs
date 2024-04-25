using GestaoAvaliacao.API.Middleware;
using GestaoAvaliacao.WebProject.Facade;
using ProvaSP.Data;
using ProvaSP.Model.Entidades;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace GestaoAvaliacao.API.Controllers
{
    [AutAttribute]
    public class ProvaSPIntegracaoController : ApiController
    {
        [Route("api/prova-sp/edicoes/{anoEdicao}/alunos/{codigoAluno}")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ResultadoAluno>))]
        public HttpResponseMessage ObterResultadoAluno(int anoEdicao, string codigoAluno)
        {
            try
            {
                var result = DataResultado.ObterResultadoAluno(codigoAluno, anoEdicao);
                if (result == null) 
                    return new HttpResponseMessage(HttpStatusCode.NoContent);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel obter o resultado do Aluno na Edição.");
            }
        }

    }

}