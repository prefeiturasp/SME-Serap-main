using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
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
	public class TestTypeCourseController : Controller
	{
		private readonly ITestTypeCourseBusiness testTypeCourseBusiness;
        private readonly IACA_CursoBusiness courseBusiness;
		private readonly ITestCurriculumGradeBusiness testCurriculumGradeBusiness;
        private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;
        private readonly IACA_TipoModalidadeEnsinoBusiness modalityBusiness;

        public TestTypeCourseController(ITestTypeCourseBusiness testTypeCourseBusiness, IACA_CursoBusiness courseBusiness, ITestCurriculumGradeBusiness testCurriculumGradeBusiness, IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness,
            IACA_TipoModalidadeEnsinoBusiness modalityBusiness)
		{
			this.testTypeCourseBusiness = testTypeCourseBusiness;
			this.courseBusiness = courseBusiness;
			this.testCurriculumGradeBusiness = testCurriculumGradeBusiness;
            this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
            this.modalityBusiness = modalityBusiness;
        }

		public ActionResult Index()
		{
			return View();
		}

		#region Read

		[HttpGet]
		public JsonResult Find(int Id)
		{
			TestTypeCourse testTypeCourse = testTypeCourseBusiness.Get(Id);
			return Json(new { success = true, testTypeCourse = testTypeCourse }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Paginate]
		public JsonResult Search(int testTypeId, int TypeLevelEducationId)
		{
			try
			{
				Pager pager1 = this.GetPager();
				IEnumerable<TestTypeCourse> lista = testTypeId > 0 ? testTypeCourseBusiness.Search(testTypeId, ref pager1) : testTypeCourseBusiness.Load(ref pager1);

				if (lista != null && lista.Count() > 0)
				{
					var courseList = lista.Select(x => new
					{
						Id = x.Id,
						CourseId = x.CourseId,
						TestTypeCourseCurriculumGrades = x.TestTypeCourseCurriculumGrades.Where(cg => cg.State == (Byte)EnumState.ativo).Select(cg => new
						{
							Id = cg.CurriculumGradeId,
							//Description = curriculumGradeBusiness.Get(cg.CurriculumGradeId).crp_descricao,
                            Description = tipoCurriculoPeriodoBusiness.GetDescription(cg.TypeCurriculumGradeId, 0, 0, 0),
							Ordem = cg.Ordem,
							Status = testCurriculumGradeBusiness.ExistsTestCurriculumGrade((x.TestTypeCourseCurriculumGrades.FirstOrDefault(cgc => cgc.TypeCurriculumGradeId == cg.TypeCurriculumGradeId).TypeCurriculumGradeId), testTypeId),
							IdBD = cg.Id
						}).OrderBy(a => a.Ordem).ToList(),
						Description = courseBusiness.Get(x.CourseId, SessionFacade.UsuarioLogado.Usuario.ent_id).cur_nome,
                        Modality = modalityBusiness.GetCustom(x.ModalityId)
                    });

					return Json(new { success = true, lista = courseList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Curso não encontrado." }, JsonRequestBehavior.AllowGet);
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
		public JsonResult Save(TestTypeCourse entity, int TypeLevelEducationId, int ModalityId)
		{
			try
			{
				if (entity.Id > 0)
				{
					entity = testTypeCourseBusiness.Update(entity.Id, entity);
				}
				else
				{
					entity = testTypeCourseBusiness.Save(entity, TypeLevelEducationId, ModalityId);
				}
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = string.Format("Erro ao {0} o curso.", entity.Id > 0 ? "alterar" : "salvar");

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, testTypeID = entity.Id }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Delete(int Id, int testTypeId)
		{
			TestTypeCourse entity = new TestTypeCourse();

			try
			{
				entity = testTypeCourseBusiness.Delete(Id, testTypeId);
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