using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class StudentTestSessionController : Controller
    {
        private readonly IStudentTestSessionBusiness studentTestSessionBusiness;

        public StudentTestSessionController(IStudentTestSessionBusiness studentTestSessionBusiness)
        {
            this.studentTestSessionBusiness = studentTestSessionBusiness;
        }

        public ActionResult Index(long test_id, long team_id, long esc_id) => View();

        [HttpGet]
        public async Task<JsonResult> GetStudentsSession(long test_id, long team_id, long esc_id)
        {
            try
            {
                var students = await studentTestSessionBusiness.GetStudentTestSession(test_id, team_id);
                return Json(new { success = true, dados = students }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar dados." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}