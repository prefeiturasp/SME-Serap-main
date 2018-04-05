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
    public class FormatTypeController : Controller
    {
        private readonly IFormatTypeBusiness formatTypeBusiness;

        public FormatTypeController(IFormatTypeBusiness formatTypeBusiness)
        {
            this.formatTypeBusiness = formatTypeBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Read

        [HttpGet]
        public JsonResult Find(int Id)
        {
            FormatType formatType = formatTypeBusiness.Get(Id);
            return Json(new { success = true, formatType = formatType }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Load()
        {
            try
            {
                IEnumerable<FormatType> lista = formatTypeBusiness.Load();

                if (lista != null && lista.Count() > 0)
                {
                    var formatTypeList = lista.Select(x => new
                    {
                        Id = x.Id,
                        Description = x.Description,
                    });

                    return Json(new { success = true, lista = formatTypeList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Formato não encontrado." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar formato pesquisada." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}