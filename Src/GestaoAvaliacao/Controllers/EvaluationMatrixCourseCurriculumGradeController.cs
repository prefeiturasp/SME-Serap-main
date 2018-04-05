using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
	[AuthorizeModule]
	public class EvaluationMatrixCourseCurriculumGradeController : Controller
	{
		private readonly IEvaluationMatrixCourseCurriculumGradeBusiness evaluationMatrixCourseCurriculumBusiness;
		private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;

        public EvaluationMatrixCourseCurriculumGradeController(IEvaluationMatrixCourseCurriculumGradeBusiness evaluationMatrixCourseCurriculumBusiness, IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness)
		{
			this.evaluationMatrixCourseCurriculumBusiness = evaluationMatrixCourseCurriculumBusiness;
			this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
		}

		public ActionResult Index()
		{
			return View();
		}

		#region Read

		[HttpGet]
		public JsonResult GetCurriculumGradesByMatrix(int evaluationMatrixId)
		{
			try
			{
				IEnumerable<ACA_TipoCurriculoPeriodo> listCurriculumGrades = tipoCurriculoPeriodoBusiness.GetAllTypeCurriculumGrades();
				List<EvaluationMatrixCourseCurriculumGrade> list = evaluationMatrixCourseCurriculumBusiness.GetCurriculumGradesByMatrix(evaluationMatrixId);

				if (list != null && list.Count > 0)
				{
					var query = list.Select(i => new
					{
						TypeCurriculumGradeId = i.TypeCurriculumGradeId,
						TypeCurriculumGrade = i.TypeCurriculumGradeId > 0 ? new
						{
                            Id = listCurriculumGrades.FirstOrDefault(a => a.tcp_id == i.TypeCurriculumGradeId).tcp_id,
                            Description = listCurriculumGrades.FirstOrDefault(a => a.tcp_id == i.TypeCurriculumGradeId).tcp_descricao,
                            Order = listCurriculumGrades.FirstOrDefault(a => a.tcp_id == i.TypeCurriculumGradeId).tcp_ordem
						} : null
					}).Where(x => x.TypeCurriculumGrade != null).OrderBy(x => x.TypeCurriculumGrade.Order).ToList();

					return Json(new { success = true, lista = query }, JsonRequestBehavior.AllowGet);
				}

				return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Os anos da matriz não foram encontrados." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os anos da matriz do item pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion

		#region Write

		[HttpPost]
		public JsonResult Save(List<EvaluationMatrixCourseCurriculumGrade> evaluationMatrixCourseCurriculumGrades, int evaluationMatrixId, int courseId, int typeLevelEducationId, int modalityId)
		{
			try
			{
				evaluationMatrixCourseCurriculumBusiness.SaveList(evaluationMatrixCourseCurriculumGrades, evaluationMatrixId, courseId, typeLevelEducationId, modalityId, SessionFacade.UsuarioLogado.Usuario.ent_id);
				return Json(new { success = true, type = ValidateType.Save.ToString(), EvaluationMatrix = evaluationMatrixId, message = "Anos(s) alterado(s) com sucesso." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao alterar o(s) anos(s)." }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion
	}
}