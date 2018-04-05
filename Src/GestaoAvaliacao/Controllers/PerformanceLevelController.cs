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
	public class PerformanceLevelController : Controller
	{
		private readonly IPerformanceLevelBusiness performanceLevelBusiness;

		public PerformanceLevelController(IPerformanceLevelBusiness performanceLevelBusiness)
		{
			this.performanceLevelBusiness = performanceLevelBusiness;
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
			PerformanceLevel performanceLevel = performanceLevelBusiness.Get(Id);

			var retorno = new
			{
				Id = performanceLevel.Id,
				Description = performanceLevel.Description,
				Code = performanceLevel.Code
			};

            return Json(new { success = true, performanceLevel = retorno }, JsonRequestBehavior.AllowGet);
		}

        [HttpGet]
        [Paginate]
        public JsonResult Load()
        {
            try
            {
                Pager pager = this.GetPager();
                IEnumerable<PerformanceLevel> lista = performanceLevelBusiness.Load(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var performanceLevelList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Code = x.Code,
                        IdBD = 0
                    }).ToList();

                    return Json(new { success = true, lista = performanceLevelList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Nível(is) de desempenho não encontrado(s)." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o(s) nível(is) de desempenho pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult Search(String search)
        {
            try
            {
                Pager pager1 = this.GetPager();
                IEnumerable<PerformanceLevel> lista = search != null ? performanceLevelBusiness.Search(search, ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id) : performanceLevelBusiness.Load(ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var itemLevelList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Code = x.Code,
                    }).ToList();

                    return Json(new { success = true, lista = itemLevelList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Nível(is) de desempenho não encontrado(s)." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o(s) nível(is) de desempenho pesquisado." }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetAll()
        {
            try
            {
                IEnumerable<PerformanceLevel> lista = performanceLevelBusiness.GetAll(SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var itemLevelList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Code = x.Code,
                    }).ToList();

                    return Json(new { success = true, lista = itemLevelList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Nível(is) de desempenho não encontrado(s)." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o(s) nível(is) de desempenho pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LoadLevels()
        {
            try
            {
                IEnumerable<PerformanceLevel> lista = performanceLevelBusiness.LoadLevels(SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var performanceLevelList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Ordem = x.Code,
                    }).ToList();

                    return Json(new { success = true, lista = performanceLevelList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Nível(is) de desempenho não encontrado(s)." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o(s) nível(is) de desempenho." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        [HttpPost]
		public JsonResult Save(PerformanceLevel entity)
		{
            try
            {
                if (entity.Id > 0)
                {
                    entity = performanceLevelBusiness.Update(entity.Id, entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
                else
                {
                    entity = performanceLevelBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} o nível de desempenho.", entity.Id > 0 ? "alterar" : "salvar");

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Delete(int Id)
		{
            PerformanceLevel entity = new PerformanceLevel();

            try
            {
                entity = performanceLevelBusiness.Delete(Id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir o nível de desempenho.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}
		
		#endregion
	}
}