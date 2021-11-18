using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class ItemTypeController : Controller
	{
		private readonly IItemTypeBusiness itemTypeBusiness;

		public ItemTypeController(IItemTypeBusiness itemTypeBusiness)
		{
			this.itemTypeBusiness = itemTypeBusiness;
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult IndexForm()
		{
			return View();
		}

		#region Read

		[HttpGet]
		[Paginate]
		public JsonResult Find(long id)
		{
			try
			{
				ItemType entity = itemTypeBusiness.Get(id);


				if (entity != null)
				{
					var itemType = new
					{
						Id = entity.Id,
						Description = entity.Description,
						IsDefault = entity.IsDefault,
                        QuantityAlternative = entity.QuantityAlternative != null ? entity.QuantityAlternative : 0,
						State = (entity.State == Convert.ToByte(EnumState.ativo))
					};

					return Json(new { success = true, itemType = itemType }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Tipo do item não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HttpResponse.RemoveOutputCacheItem("/ItemType/Load");
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar tipo do item pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		[OutputCache(CacheProfile = "Cache1Day")]
		public JsonResult Load()
		{
			try
			{
				IEnumerable<ItemType> lista = itemTypeBusiness.FindSimple(SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var itemTypeList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
                        IsDefault = x.IsDefault,
                        QuantityAlternative = x.QuantityAlternative != null ? x.QuantityAlternative : 0
					});

					return Json(new { success = true, lista = itemTypeList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Tipo do item não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HttpResponse.RemoveOutputCacheItem("/ItemType/Load");
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar tipo do item pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpGet]
        [Paginate]
        [OutputCache(CacheProfile = "Cache1Day")]
        public JsonResult LoadForTestType()
        {
            try
            {
                IEnumerable<ItemType> lista = itemTypeBusiness.FindForTestType(SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var itemTypeList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                        IsDefault = x.IsDefault,
                        QuantityAlternative = x.QuantityAlternative != null ? x.QuantityAlternative : 0
                    });

                    return Json(new { success = true, lista = itemTypeList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Tipo do item não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                HttpResponse.RemoveOutputCacheItem("/ItemType/LoadForTestType");
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar tipo do item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

		[HttpGet]
		[Paginate]
		public JsonResult Search(string search)
		{
			try
			{
				Pager pager = this.GetPager();

				IEnumerable<ItemType> lista = itemTypeBusiness.Search(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id, search);

				if (lista != null && lista.Count() > 0)
				{
					var itemTypeList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
						IsDefault = x.IsDefault,
                        QuantityAlternative = x.QuantityAlternative != null ? x.QuantityAlternative : 0,
						State = (x.State == Convert.ToByte(EnumState.ativo))
					});

					return Json(new { success = true, lista = itemTypeList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Tipo do item não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HttpResponse.RemoveOutputCacheItem("/ItemType/Load");
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar tipo do item pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

        #endregion

        #region Write

        [HttpPost]
		public JsonResult Save(ItemType entity, bool State)
		{
			try
			{
				entity.EntityId = SessionFacade.UsuarioLogado.Usuario.ent_id;
				entity.State = Convert.ToByte(State ? EnumState.ativo : EnumState.inativo);

				if (entity.Id > 0)
				{
					entity = itemTypeBusiness.Update(entity);
				}
				else
                {
					entity = itemTypeBusiness.Save(entity);
				}

                if (entity.Validate.IsValid)
                {
                    HttpResponse.RemoveOutputCacheItem("/ItemType/Load");
                    HttpResponse.RemoveOutputCacheItem("/ItemType/LoadForTestType");
                }
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} o tipo de item.", entity.Id > 0 ? "alterar" : "salvar");

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		[HttpPost]
		public JsonResult Delete(long Id)
		{
			ItemType entity = new ItemType();

			try
			{
				entity = itemTypeBusiness.Delete(Id);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao tentar o tipo de item.";
				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}
	}
}