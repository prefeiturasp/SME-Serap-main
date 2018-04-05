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
	public class ItemLevelController : Controller
	{
		private readonly IItemLevelBusiness itemLevelBusiness;

		public ItemLevelController(IItemLevelBusiness itemLevelBusiness)
		{
			this.itemLevelBusiness = itemLevelBusiness;
		}

		public ActionResult List()
		{
			return View();
		}

		public ActionResult Form()
		{
			return View();
		}

        #region Read

        [HttpGet]
        public JsonResult Find(int Id)
        {
            ItemLevel itemLevel = itemLevelBusiness.Get(Id);

            var retorno = new
            {
                Id = itemLevel.Id,
                Description = itemLevel.Description,
                Value = itemLevel.Value
            };

            return Json(new { success = true, itemLevel = retorno }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Paginate]
        public JsonResult Load()
        {
            try
            {
                Pager pager = this.GetPager();
                IEnumerable<ItemLevel> lista = itemLevelBusiness.Load(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var itemLevelList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Value = x.Value,
                        IdBD = 0
                    }).ToList();

                    return Json(new { success = true, lista = itemLevelList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Nível(is) de dificuldade do item não encontrado(s)." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o(s) nível(is) de dificuldade do item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult Search(String search)
        {
            try
            {
                Pager pager1 = this.GetPager();
                IEnumerable<ItemLevel> lista = search != null ? itemLevelBusiness.Search(search, ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id) : itemLevelBusiness.Load(ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var itemLevelList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Value = x.Value,
                    }).ToList();

                    return Json(new { success = true, lista = itemLevelList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Nível(is) de dificuldade do item não encontrado(s)." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o(s) nível(is) de dificuldade do item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult LoadLevels()
        {
            try
            {
                IEnumerable<ItemLevel> lista = itemLevelBusiness.LoadLevels(SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var itemLevelList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Ordem = x.Value,
                    }).ToList();

                    return Json(new { success = true, lista = itemLevelList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Nível(is) de dificuldade do item não encontrado(s)." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o(s) nível(is) de dificuldade do item." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

		[HttpPost]
        public JsonResult Save(ItemLevel entity)
		{
            try
            {
                if (entity.Id > 0)
                {
                    entity = itemLevelBusiness.Update(entity.Id, entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
                else
                {
                    entity = itemLevelBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} o nível de dificuldade do item.", entity.Id > 0 ? "alterar" : "salvar");

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Delete(int Id)
		{
            ItemLevel entity = new ItemLevel();

            try
            {
                entity = itemLevelBusiness.Delete(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir o nível de dificuldade do item.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}