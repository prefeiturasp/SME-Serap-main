using GestaoAvaliacao.API.Middleware;
using GestaoAvaliacao.API.Models;
using GestaoAvaliacao.Dtos.ItemApi;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;


namespace GestaoAvaliacao.API.Controllers
{
    [AutAttribute]
    public class ItemController : ApiController
    {
        private readonly IItemBusiness itemBusiness;
        private readonly IACA_TipoNivelEnsinoBusiness levelEducationBusiness;

        public ItemController(IItemBusiness itemBusiness)
        {
            this.itemBusiness = itemBusiness;
        }


        [Route("api/Item/AreasConhecimento")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllKnowledgeAreaActive()
        {
            try
            {
                var result = itemBusiness.LoadAllKnowledgeAreaActive();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return null;
            }
        }


        [Route("api/Item/Disciplinas/AreaConhecimentoId")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAlDisciplinebyknowledgearea(int areaConhecimentoId)
        {
            try
            {
                var result = itemBusiness.LoadDisciplineByKnowledgeArea(areaConhecimentoId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return null;
            }
        }

        [Route("api/Item/Matrizes/DisciplinaId")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetEvaluationMatrixbyDiscipline(int disciplinaId)
        {
            try
            {
                if (disciplinaId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "O parametro disciplinaId é obrigatório e tem que ser maior que zero.");


              var lista = itemBusiness.LoadMatrixByDiscipline(disciplinaId);

                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Matriz de avaliação não encontrada.");

                // TODO return dto

           

                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel retornar a lista de matrizes dessa disciplina");
            }
        }


        [Route("api/Item/Eixos/MatrizId")]
        [HttpGet]
        [ResponseType(typeof(SkillDto))]
        public async Task<HttpResponseMessage> GetSkillbymatriz(long matrizId)
        {
            try
            {
                if (matrizId <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "O parametro matrizId é obrigatório e tem que ser maior que zero.");


                var lista = itemBusiness.LoadSkillByMatrix(matrizId);

                if (lista == null || !lista.Any())
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Eixos não encontrados.");
                // TODO return dto 
                return Request.CreateResponse(HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Não foi possivel retornar a lista de eixos dessa matriz");
            }
        }



        [Route("api/Item/Save")]
        [HttpPost]
        [ResponseType(typeof(ItemResult))]
        public async Task<HttpResponseMessage> ItemSave([FromBody] ItemModel model)
        {
            Item entity = new Item();
            try
            {

                // entity = itemBusiness.Save(0, item);
                var result = new ItemResult();

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                var result = new ItemResult();

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
        }
    }
}