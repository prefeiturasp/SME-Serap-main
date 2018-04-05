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
    public class TestTypeItemLevelController : Controller
    {
        private readonly ITestTypeItemLevelBusiness testTypeItemLevelBusiness;

        public TestTypeItemLevelController(ITestTypeItemLevelBusiness testTypeItemLevelBusiness)
        {
            this.testTypeItemLevelBusiness = testTypeItemLevelBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Read

        [HttpGet]
        public JsonResult Find(int Id)
        {
            TestTypeItemLevel testTypeItemLevel = testTypeItemLevelBusiness.Get(Id);
            return Json(new { success = true, testTypeItemLevel = testTypeItemLevel }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Load()
        {
            try
            {
                IEnumerable<TestTypeItemLevel> lista = testTypeItemLevelBusiness.Load();

                if (lista != null && lista.Count() > 0)
                {
                    var testTypeItemLevelList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Value,
                    });

                    return Json(new { success = true, lista = testTypeItemLevelList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Item não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar item pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        public JsonResult Save(TestTypeItemLevel entity)
        {
            try
            {
                if (entity.TestType.Id > 0)
                {
                    entity = testTypeItemLevelBusiness.Update(entity.TestType.Id, entity);
                }
                else
                {
                    entity = testTypeItemLevelBusiness.Save(entity);
                }
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} o item.", entity.Id > 0 ? "alterar" : "salvar");

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}