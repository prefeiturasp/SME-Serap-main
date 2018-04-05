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
    public class ModelSkillLevelController : Controller
    {
        private readonly IModelSkillLevelBusiness modelSkillLevelBusiness;

        public ModelSkillLevelController(IModelSkillLevelBusiness modelSkillLevelBusiness)
        {
            this.modelSkillLevelBusiness = modelSkillLevelBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Read

        [HttpGet]
        public JsonResult Find(int Id)
        {
            ModelSkillLevel modelSkillLevel = modelSkillLevelBusiness.Get(Id);
            return Json(new { success = true, modelSkillLevel = modelSkillLevel }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FindByLevel(int level, int modelEvatuationMatrixId)
        {
            ModelSkillLevel modelSkillLevel = modelSkillLevelBusiness.GetByLevel(level, modelEvatuationMatrixId);
            return Json(new { success = true, modelSkillLevel = modelSkillLevel }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Load(long modelEvatuationMatrixId)
        {
            try
            {
                IEnumerable<ModelSkillLevel> lista = modelSkillLevelBusiness.Load(modelEvatuationMatrixId);

                if (lista != null && lista.Count() > 0)
                {
                    var modelSkillLevelList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Level = x.Level,
                        LastLevel = x.LastLevel,
                        ModelEvaluationMatrix = new
                            {
                                Id = x.ModelEvaluationMatrix.Id,
                                Description = x.ModelEvaluationMatrix.Description
                            }
                    });

                    return Json(new { success = true, lista = modelSkillLevelList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Modelo de Habilidades não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o Modelo de Habilidades pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
	}
}