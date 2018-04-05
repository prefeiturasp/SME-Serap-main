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
	public class EvaluationMatrixCourseController : Controller
	{
		private readonly IEvaluationMatrixCourseBusiness evaluationMatrixCourseBusiness;
		private readonly IACA_CursoBusiness courseBusiness;
		private readonly IItemCurriculumGradeBusiness itemCurriculumGradeBusiness;
        private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;
        private readonly IACA_TipoModalidadeEnsinoBusiness modalityBusiness;

        public EvaluationMatrixCourseController(IEvaluationMatrixCourseBusiness evaluationMatrixCourseBusiness, IACA_CursoBusiness courseBusiness, IItemCurriculumGradeBusiness itemCurriculumGradeBusiness, IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness,
            IACA_TipoModalidadeEnsinoBusiness modalityBusiness)
		{
			this.evaluationMatrixCourseBusiness = evaluationMatrixCourseBusiness;
			this.courseBusiness = courseBusiness;
			this.itemCurriculumGradeBusiness = itemCurriculumGradeBusiness;
            this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
            this.modalityBusiness = modalityBusiness;
		}

		public ActionResult Index()
		{
			return View();
		}

		#region Read

		[HttpGet]
		[Paginate]
		public JsonResult Search(int tipoNivelEnsino, int evaluationMatrixId)
		{
			try
			{
				Pager pager1 = this.GetPager();
				IEnumerable<EvaluationMatrixCourse> lista = evaluationMatrixId > 0 ? evaluationMatrixCourseBusiness.Search(evaluationMatrixId, ref pager1) : evaluationMatrixCourseBusiness.Load(ref pager1);

				if (lista != null && lista.Count() > 0)
				{
					var courseList = lista.Select(x => new
					{
						Id = x.Id,
						CourseId = x.CourseId,
						EvaluationMatrixCourseCurriculumGrades = (from z in x.EvaluationMatrixCourseCurriculumGrades
																  where z.State == 1
																  select new
																  {
																	  Id = z.CurriculumGradeId,
																	  Description = tipoCurriculoPeriodoBusiness.GetDescription(z.TypeCurriculumGradeId, 0, 0, 0),
																	  Ordem = z.Ordem,
																	  Status = itemCurriculumGradeBusiness.ExistsItemCurriculumGrade((x.EvaluationMatrixCourseCurriculumGrades.FirstOrDefault(cgc => cgc.TypeCurriculumGradeId == z.TypeCurriculumGradeId).TypeCurriculumGradeId), evaluationMatrixId),
																	  IdBD = z.Id
																  }).OrderBy(a => a.Ordem).ToList(),
						Description = courseBusiness.Get(x.CourseId, SessionFacade.UsuarioLogado.Usuario.ent_id).cur_nome,
						Modality = modalityBusiness.GetCustom(x.ModalityId),
						TypeLevelId = tipoNivelEnsino
					});

					return Json(new { success = true, lista = courseList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar o curso." }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion

		#region Write

		[HttpPost]
		public JsonResult Save(EvaluationMatrixCourse entity)
		{
			try
			{
				if (entity.Id > 0)
				{
					entity = evaluationMatrixCourseBusiness.Update(entity.Id, entity);
				}
				else
				{
					entity = evaluationMatrixCourseBusiness.Save(entity);
				}
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = string.Format("Erro ao {0} o curso.", entity.Id > 0 ? "alterar" : "salvar");

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Delete(int Id, int evaluationMatrixId)
		{
			EvaluationMatrixCourse entity = new EvaluationMatrixCourse();

			try
			{
				entity = evaluationMatrixCourseBusiness.Delete(Id, evaluationMatrixId);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao tentar excluir o curso.";
				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}