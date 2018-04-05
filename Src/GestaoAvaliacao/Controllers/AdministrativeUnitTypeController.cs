using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
	[Authorize]
	[AuthorizeModule]
	public class AdministrativeUnitTypeController : Controller
	{
        private readonly IAdministrativeUnitTypeBusiness administrativeUnitTypeBusiness;

        public AdministrativeUnitTypeController(IAdministrativeUnitTypeBusiness administrativeUnitTypeBusiness)
        {
            this.administrativeUnitTypeBusiness = administrativeUnitTypeBusiness;
        }

        public ActionResult Index()
		{
			return View();
		}

        #region Read

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                IEnumerable<AdministrativeUnitType> lista = administrativeUnitTypeBusiness.Get();
                if (lista != null)
                {
                    var ret = lista.Select(x => new
                    {
                        AdministrativeUnitTypeId = x.AdministrativeUnitTypeId,
                        Name = x.Name
                    });

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não foram encontrados tipos de unidades administrativas disponíveis." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar carregar os tipos de unidades administrativas disponíveis." }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public ActionResult GetAdministrativeUnitsTypes()
        {
            try
            {
                IEnumerable<AdministrativeUnitType> lista = administrativeUnitTypeBusiness.GetAdministrativeUnitsTypes();
                if (lista != null)
                {
                    var ret = lista.Select(x => new
                    {
                        AdministrativeUnitTypeId = x.AdministrativeUnitTypeId,
                        Name = x.Name
                    });

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não foram encontrados tipos de unidades administrativas selecionados." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar carregar os tipos de unidades administrativas selecionados." }, JsonRequestBehavior.AllowGet);

            }
        }

        #endregion

        #region Write

        [HttpPost]
        public JsonResult Save(IEnumerable<AdministrativeUnitType> unitTypes)
        {
            try
            {
                administrativeUnitTypeBusiness.Save(unitTypes);
                return Json(new { success = true, message = "Tipos de unidades administrativas salvos com sucesso." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao salvar os tipos de unidade administrativa." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}