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
    public class CognitiveCompetenceController : Controller
	{
		private readonly ICognitiveCompetenceBusiness cognitiveCompetenceBusiness;

		public CognitiveCompetenceController(ICognitiveCompetenceBusiness cognitiveCompetenceBusiness)
		{
			this.cognitiveCompetenceBusiness = cognitiveCompetenceBusiness;
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
		[Paginate]
		public JsonResult Search(String search)
		{
			try
			{
				Pager pager = this.GetPager();
                IEnumerable<CognitiveCompetence> lista = search != null ? cognitiveCompetenceBusiness.Search(search, ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id) : cognitiveCompetenceBusiness.Load(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);
                if (lista != null && lista.Count() > 0)
                {
                    var cognitiveCompetenceList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                    });

                    return Json(new { success = true, lista = cognitiveCompetenceList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar competência cognitiva pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult FindSimple(int Id)
		{
            try
            {
			    CognitiveCompetence cognitiveCompetence = cognitiveCompetenceBusiness.Get(Id);

			    var retorno = new
			    {
				    Description = cognitiveCompetence.Description,
				    Id = cognitiveCompetence.Id
			    };

                return Json(new { success = true, cognitiveCompetence = retorno }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar a competência cognitiva para edição." }, JsonRequestBehavior.AllowGet);
            }
		}

        [HttpGet]
        public JsonResult Find()
        {
            try
            {
                IEnumerable<CognitiveCompetence> lista = cognitiveCompetenceBusiness.FindAll(SessionFacade.UsuarioLogado.Usuario.ent_id);
                if (lista != null && lista.Count() > 0)
                {
                    var cognitiveCompetenceList = lista.Select(x => new
                    {
                        Description = x.Description,
                        Id = x.Id
                    });

                    return Json(new { success = true, lista = cognitiveCompetenceList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar a(s) competência(s) cognitiva(s)." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        [HttpPost]
		public JsonResult Save(CognitiveCompetence entity)
		{
            try
            {
                if (entity.Id > 0)
                {
                    entity = cognitiveCompetenceBusiness.Update(entity.Id, entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
                else
                {
                    entity = cognitiveCompetenceBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} a competência cognitiva.", entity.Id > 0 ? "alterar" : "salvar");

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Delete(int Id)
		{
            CognitiveCompetence entity = new CognitiveCompetence();

            try
            {
                entity = cognitiveCompetenceBusiness.Delete(Id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir a competência cognitiva.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}