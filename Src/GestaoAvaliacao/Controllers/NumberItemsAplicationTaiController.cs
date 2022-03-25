using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Models;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.Util.Extensions;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{

    [Authorize]
    [AuthorizeModule]
    public class NumberItemsAplicationTaiController : Controller
    {

        private readonly INumberItemsAplicationTaiBusiness numberItemsAplicationTaiBusiness;

        public NumberItemsAplicationTaiController(INumberItemsAplicationTaiBusiness numberItemsAplicationTaiBusiness)
        {
            this.numberItemsAplicationTaiBusiness = numberItemsAplicationTaiBusiness;
        }

        [HttpGet]
        public JsonResult LoadAll()
        {
            try
            {
                var ret = numberItemsAplicationTaiBusiness.GetAll();
                return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar listar as opções de itens para prova TAI." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetByTestId(long testId)
        {
            try
            {
                var ret = numberItemsAplicationTaiBusiness.GetByTestId(testId);
                return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar obter a opção de itens da prova TAI." }, JsonRequestBehavior.AllowGet);
            }
        }

    }

}
