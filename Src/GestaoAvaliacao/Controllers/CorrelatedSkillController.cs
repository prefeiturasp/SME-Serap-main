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
    public class CorrelatedSkillController : Controller
    {
        private readonly ICorrelatedSkillBusiness correlatedSkillBusiness;

        public CorrelatedSkillController(ICorrelatedSkillBusiness correlatedSkillBusiness)
        {
            this.correlatedSkillBusiness = correlatedSkillBusiness;
        }

        public ActionResult Form()
        {
            return View();
        }

        #region Read

        [HttpGet]
        [Paginate]
        public JsonResult LoadList(long MatrizId)
        {
            try
            {
                Pager pager = this.GetPager();
                List<CorrelatedSkillByEvaluationMatrix> lista = correlatedSkillBusiness.LoadList(MatrizId, ref pager);
                if (lista != null && lista.Count() > 0)
                {
                    var correlatedSkillList = lista.Select(il => new
                        {
                            Id = il.Id,
                            Matriz1 = il.Matriz1,
                            Matriz2 = il.Matriz2,
                            UltimoNivel1 = il.UltimoNivel1,
                            UltimoNivel2 = il.UltimoNivel2
                        });

                    return Json(new { success = true, lista = correlatedSkillList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar correlação de habilidade pesquisada." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        [HttpPost]
        public JsonResult Save(CorrelatedSkill entity)
        {
            try
            {
                entity = correlatedSkillBusiness.Save(entity);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao salvar a correlação de habilidade.";

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(int Id)
        {
            CorrelatedSkill entity = new CorrelatedSkill();

            try
            {
                entity = correlatedSkillBusiness.Delete(Id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir a correlação de habilidade.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}