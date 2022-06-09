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


        [Route("api/Item/Get")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get(int itemId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Ok");
        }
        

            [Route("api/Item/Save")]
        [HttpPost]
        [ResponseType(typeof(ItemResult))]
        public async Task<HttpResponseMessage> ItemSave([FromBody] ItemModel model)
        {
            Item entity = new Item();
            try
            {

                //entity = itemBusiness.Save(0, item);
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