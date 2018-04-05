using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class ItemSituationController : Controller
    {
        private readonly IItemSituationBusiness itemSituationBusiness;

        public ItemSituationController(IItemSituationBusiness itemSituationBusiness)
        {
            this.itemSituationBusiness = itemSituationBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Read

        [HttpGet]
        [Paginate]
        [OutputCache(CacheProfile = "Cache1Day")]
        public JsonResult Load()
        {
            try
            {
                Pager pager = this.GetPager();
                IEnumerable<ItemSituation> lista = itemSituationBusiness.Load(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var itemSituationList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                    });

                    return Json(new { success = true, lista = itemSituationList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Situação do item não encontrada." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar situação do item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}