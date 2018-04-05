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
	public class KnowledgeAreaController : Controller
	{
		private readonly IKnowledgeAreaBusiness knowledgeAreaBusiness;

        public KnowledgeAreaController(IKnowledgeAreaBusiness knowledgeAreaBusiness)
		{
            this.knowledgeAreaBusiness = knowledgeAreaBusiness;	
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
            KnowledgeArea knowledgeArea = knowledgeAreaBusiness.Get(Id);
            return Json(new { success = true, knowledgeArea = knowledgeArea }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Paginate]
        public JsonResult Load()
        {
            try
            {
                Pager pager = this.GetPager();
                IEnumerable<KnowledgeArea> lista = knowledgeAreaBusiness.Load(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var knowledgeAreaList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description
                    });

                    return Json(new { success = true, lista = knowledgeAreaList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Área de conhecimento não encontrada." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a área de conhecimento pesquisada." }, JsonRequestBehavior.AllowGet);
            }
        }
   
        [HttpGet]
        [Paginate]
        public JsonResult Search(String search)
        {
            try
            {
                Pager pager1 = this.GetPager();
                IEnumerable<KnowledgeArea> lista = search != null ? knowledgeAreaBusiness.Search(search, ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id) : knowledgeAreaBusiness.Load(ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id);

                if (lista != null && lista.Count() > 0)
                {
                    var knowledgeAreaList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description
                    });

                    return Json(new { success = true, lista = knowledgeAreaList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar área de conhecimento." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        [HttpPost]
		public JsonResult Save(KnowledgeArea entity)
		{
            try
            {
                if (entity.Id > 0)
                {
                    entity = knowledgeAreaBusiness.Update(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
                else
                {
                    entity = knowledgeAreaBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
                }
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} área de conhecimento.", entity.Id > 0 ? "alterar" : "salvar");

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }
       
		[HttpPost]
		public JsonResult Delete(int Id)
		{
            KnowledgeArea entity = new KnowledgeArea();

            try
            {
                entity = knowledgeAreaBusiness.Delete(Id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir a área de conhecimento.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}