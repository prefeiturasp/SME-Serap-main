using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
	[AuthorizeModule]
	public class TestTypeCourseCurriculumGradeController : Controller
	{
		private readonly ITestTypeCourseCurriculumGradeBusiness testTypeCourseCurriculumGrade;

        public TestTypeCourseCurriculumGradeController(ITestTypeCourseCurriculumGradeBusiness testTypeCourseCurriculumGrade)
		{
			this.testTypeCourseCurriculumGrade = testTypeCourseCurriculumGrade;
		}

		public ActionResult Index()
		{
			return View();
		}

		#region Write

		[HttpPost]
		public JsonResult Save(List<TestTypeCourseCurriculumGrade> testTypeCourseCurriculumGrades, int testTypeId, int courseId, int typeLevelEducationId, int modalityId)
		{
			try
			{
				testTypeCourseCurriculumGrade.SaveList(testTypeCourseCurriculumGrades, testTypeId, courseId, typeLevelEducationId, modalityId, SessionFacade.UsuarioLogado.Usuario.ent_id);
				return Json(new { success = true, type = ValidateType.Save.ToString(), TestType = testTypeId }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao alterar o(s) ano(s)." }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion
	}
}