using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers.Integration
{
    [Authorize]
    public class LevelEducationController : Controller
    {
        private readonly IACA_TipoNivelEnsinoBusiness levelEducationBusiness;

        public LevelEducationController(IACA_TipoNivelEnsinoBusiness levelEducationBusiness)
        {
            this.levelEducationBusiness = levelEducationBusiness;         
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Read

        [HttpGet]
        public JsonResult Load()
        {
            try
            {
                IEnumerable<ACA_TipoNivelEnsino> lista = levelEducationBusiness.Load();

                if (lista != null && lista.Count() > 0)
                {
                    var levelEducationList = lista.Select(x => new
                    {
                        Id = x.tne_id,
                        Description = x.tne_nome,
                    });

                    return Json(new { success = true, lista = levelEducationList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Nível de ensino não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar nível de ensino." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
	}
}