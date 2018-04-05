using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers.Integration
{
    [Authorize]
	public class CourseController : Controller
	{
		private readonly IACA_CursoBusiness courseBusiness;
        private readonly IACA_TipoModalidadeEnsinoBusiness modalityBusiness;

        public CourseController(IACA_CursoBusiness courseBusiness, IACA_TipoModalidadeEnsinoBusiness modalityBusiness)
		{
			this.courseBusiness = courseBusiness;
            this.modalityBusiness = modalityBusiness;
		}

		public ActionResult Index()
		{
			return View();
		}

		#region Read

        [HttpGet]
		public JsonResult SearchCourses()
		{
			try
			{
				IEnumerable<ACA_Curso> lista = courseBusiness.Load(SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var courseList = lista.Select(x => new
					{
						Id = x.cur_id,
						Description = x.cur_nome,
					});

					return Json(new { success = true, lista = courseList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Curso não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar curso." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult SearchCoursesByLevel(int typeLevelEducation)
		{
			try
			{
				IEnumerable<ACA_Curso> lista = courseBusiness.LoadByTipoNivelEnsino(SessionFacade.UsuarioLogado.Usuario.ent_id, typeLevelEducation);

				if (lista != null && lista.Count() > 0)
				{
					var courseList = lista.Select(x => new
					{
						Id = x.cur_id,
						Description = x.cur_nome,
					});

					return Json(new { success = true, lista = courseList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Curso não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar curso." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult SearchCoursesByLevelModality(int typeLevelEducation, int modality)
		{
			try
			{
				IEnumerable<ACA_Curso> lista = courseBusiness.LoadByNivelEnsinoModality(SessionFacade.UsuarioLogado.Usuario.ent_id, typeLevelEducation, modality);

				if (lista != null && lista.Count() > 0)
				{
					var courseList = lista.Select(x => new
					{
						Id = x.cur_id,
						Description = x.cur_nome,
                        Modality = modalityBusiness.GetCustom(x.tme_id),
                    });

					return Json(new { success = true, lista = courseList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Curso não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar curso." }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion
	}
}