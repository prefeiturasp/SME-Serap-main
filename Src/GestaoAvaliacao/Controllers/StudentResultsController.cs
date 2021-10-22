using GestaoAvaliacao.Dtos.StudentTestAccoplishments;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.DTO.StudentsTestSent;
using GestaoAvaliacao.Entities.DTO.Tests;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using Newtonsoft.Json;
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
        private readonly ITestBusiness _testBusiness;
        private readonly IStudentCorrectionBusiness _studentCorrectionBusiness;

        public StudentResultsController(IStudentTestAccoplishmentBusiness studentTestAccoplishmentBusiness, ITestBusiness testBusiness,
            IStudentCorrectionBusiness studentCorrectionBusiness)
        {
            _studentTestAccoplishmentBusiness = studentTestAccoplishmentBusiness;
            _testBusiness = testBusiness;
            _studentCorrectionBusiness = studentCorrectionBusiness;
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
                //var textoJson = System.IO.File.ReadAllText(Server.MapPath("/") + "file.json");
                //var dados = JsonConvert.DeserializeObject<StudentTestTimeResultDto>(textoJson);

                //return Json(new { success = true, dados }, JsonRequestBehavior.AllowGet);

                var dados = new StudentTestTimeResultDto();
                var electronicTests = await _testBusiness.SearchEletronicTestsByPesId(SessionFacade.UsuarioLogado.Usuario.pes_id);
                if (electronicTests is null || electronicTests.Count() == 0)
                {
                    return Json(new { success = true, dados }, JsonRequestBehavior.AllowGet);
                }

                dados = await _studentTestAccoplishmentBusiness.GetStudenteResultAsync(electronicTests);
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