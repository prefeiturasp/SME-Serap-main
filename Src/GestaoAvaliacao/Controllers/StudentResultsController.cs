using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.DTO.StudentsTestSent;
using GestaoAvaliacao.Entities.DTO.Tests;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    public class StudentResultsController : Controller
    {
        private readonly IStudentTestAccoplishmentBusiness _studentTestAccoplishmentBusiness;

        public StudentResultsController(IStudentTestAccoplishmentBusiness studentTestAccoplishmentBusiness)
        {
            _studentTestAccoplishmentBusiness = studentTestAccoplishmentBusiness;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetResultadosDosEstudantes()
        {
            try
            {
                var dados = await _studentTestAccoplishmentBusiness.GetStudenteResultAsync(SessionFacade.UsuarioLogado.Usuario.pes_id);
                return Json(new { success = true, dados }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as provas." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}