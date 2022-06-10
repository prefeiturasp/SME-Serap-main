using GestaoAvaliacao.API.App_Start;
using GestaoAvaliacao.API.Middleware;
using GestaoAvaliacao.API.Models;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
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


        [Route("api/Item/Matrizes/AreaConhecimentoId")]
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