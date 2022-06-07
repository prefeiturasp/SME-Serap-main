using GestaoAvaliacao.API.App_Start;
using GestaoAvaliacao.API.Models;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace GestaoAvaliacao.API.Controllers
{
    [CustomAuthorizationAttribute]
    public class ItemController : ApiController
    {
        private readonly IItemBusiness itemBusiness;

        public ItemController(IItemBusiness itemBusiness)
        {
            this.itemBusiness = itemBusiness;
        }

        [Route("api/Item/Save")]
        [HttpPost]
        [ResponseType(typeof(ItemResult))]
        public async Task<HttpResponseMessage> ItemSave([FromBody] ItemModel model)
        {
            Item entity = new Item();
            try
            {
                entity = MapearParaItemSerap(model);
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

        private Item MapearParaItemSerap(ItemModel model)
        {
            Item entity = new Item();
            entity.ItemCode = model.ItemCodigo;
            var textoBase = model.TextoBase;
            entity.BaseText = new BaseText { Description = textoBase.Descricao, Source = textoBase.Fonte };

            //verificar Eixo e Habilidade
            entity.SubSubject = new SubSubject { Id = model.AssuntoId };
            entity.SubSubject_Id = model.SubassuntoId;
            entity.ItemSituation_Id = model.SituacaoId;
            entity.ItemType_Id = model.TipoItemId;
            entity.IsRestrict = model.SigiloItem;
            entity.Keywords = String.Join(";", model.PalavrasChave);
            entity.proficiency = model.Proficiencia;
            entity.ItemCurriculumGrades = model.Series.Select(s => new ItemCurriculumGrade { TypeCurriculumGradeId = s }).ToList();
            entity.ItemLevel_Id = model.DificuldadeId;
            entity.Tips = model.Observacao;

            entity.TRIDiscrimination = model.TriDiscriminacao;
            entity.TRIDifficulty = model.TriDificuldade;
            entity.TRICasualSetting = model.TriAcertoCasual;

            entity.Statement = model.Enunciado;

            //videos
            //audios

            entity.ItemNarrated = false;
            entity.ItemVersion = 1;

            return entity;
        }
    }
}