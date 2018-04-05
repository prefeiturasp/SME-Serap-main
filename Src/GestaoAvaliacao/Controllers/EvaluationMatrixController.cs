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
	public class EvaluationMatrixController : Controller
	{
		private readonly IEvaluationMatrixBusiness evaluationMatrixBusiness;
		private readonly IACA_TipoNivelEnsinoBusiness levelEducationBusiness;

		public EvaluationMatrixController(IEvaluationMatrixBusiness evaluationMatrixBusiness, IACA_TipoNivelEnsinoBusiness levelEducationBusiness)
		{
			this.evaluationMatrixBusiness = evaluationMatrixBusiness;
			this.levelEducationBusiness = levelEducationBusiness;
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult IndexList()
		{
			return View("Index");
		}

		public ActionResult IndexForm()
		{
			return View("IndexForm");
		}

		#region Read

		[HttpGet]
		public JsonResult Find(int Id)
		{
			EvaluationMatrix evaluationMatrix = evaluationMatrixBusiness.Get(Id);
			return Json(new { success = true, itemLevel = evaluationMatrix }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Paginate]
		public JsonResult Load()
		{
			try
			{
				Pager pager = this.GetPager();
				IEnumerable<EvaluationMatrix> lista = evaluationMatrixBusiness.Load(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var evaluationMatrixList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
						Discipline = x.Description,
						Edition = x.Edition,
						State = x.State == (Byte)EnumState.ativo ? EnumExtensions.GetDescription(EnumState.ativo) : EnumExtensions.GetDescription(EnumState.inativo)
					});

					return Json(new { success = true, lista = evaluationMatrixList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Matriz de avaliação não encontrada." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a matriz de avaliação pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult Search(String search, String search1)
		{
			try
			{
				Pager pager1 = this.GetPager();
				IEnumerable<EvaluationMatrix> lista = search != null || search1 != null ? evaluationMatrixBusiness.Search(search, search1, ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id) : evaluationMatrixBusiness.Load(ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var evaluationMatrixList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
						DisciplineDescription = x.Discipline.Description,
						Edition = x.Edition,
						State = x.State == (Byte)EnumState.ativo ? EnumExtensions.GetDescription(EnumState.ativo) : EnumExtensions.GetDescription(EnumState.inativo)
					});

					return Json(new { success = true, lista = evaluationMatrixList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar a matriz de avaliação." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetByDiscipline(long Id)
		{
			try
			{
				IEnumerable<EvaluationMatrix> lista = evaluationMatrixBusiness.GetByDiscipline(Id);

				if (lista != null && lista.Count() > 0)
				{
					var evaluationMatrixList = lista.Select(m => new
					{
						m.Id,
						m.Description,
						ModelEvaluationMatrix = m.ModelEvaluationMatrix != null ? new
						{
							Id = m.ModelEvaluationMatrix.Id,
							Description = m.ModelEvaluationMatrix.Description
						} : null,
						Discipline = m.Discipline != null ? new
						{
							TypeLevelEducation = levelEducationBusiness.GetCustom(m.Discipline.TypeLevelEducationId)
						} : null,
					}).ToList();

					return Json(new { success = true, lista = evaluationMatrixList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Matriz de avaliação não encontrada." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar a matriz de avaliação." }, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpGet]
        public JsonResult GetByMatriz(long Id)
        {
            try
            {
                IEnumerable<EvaluationMatrix> lista = evaluationMatrixBusiness.GetByDiscipline(Id);

                if (lista != null && lista.Count() > 0)
                {
                    var evaluationMatrixList = lista.Select(m => new
                    {
                        m.Id,
                        m.Description,
                        ModelEvaluationMatrix = m.ModelEvaluationMatrix != null ? new
                        {
                            Id = m.ModelEvaluationMatrix.Id,
                            Description = m.ModelEvaluationMatrix.Description
                        } : null,
                        Discipline = m.Discipline != null ? new
                        {
                            TypeLevelEducation = levelEducationBusiness.GetCustom(m.Discipline.TypeLevelEducationId)
                        } : null,
                    }).ToList();

                    return Json(new { success = true, lista = evaluationMatrixList }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Matriz de avaliação não encontrada." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar a matriz de avaliação." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
		public JsonResult GetComboByDiscipline(long Id)
		{
			try
			{
				IEnumerable<EvaluationMatrix> lista = evaluationMatrixBusiness.GetComboByDiscipline(Id);

				if (lista != null && lista.Count() > 0)
				{
					var evaluationMatrixList = lista.Select(m => new
					{
						m.Id,
						m.Description
					}).ToList();

					return Json(new { success = true, lista = evaluationMatrixList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Matriz de avaliação não encontrada." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar a matriz de avaliação." }, JsonRequestBehavior.AllowGet);
			}
		}

		
		[HttpGet]
		public JsonResult LoadComboSituation()
		{
			try
			{
				List<State> lista = evaluationMatrixBusiness.LoadComboSituation();

				if (lista != null && lista.Count() > 0)
					return Json(new { success = true, lista = lista }, JsonRequestBehavior.AllowGet);
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Situação não encontrada." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a situação pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult LoadCombo()
		{
			try
			{
				IEnumerable<EvaluationMatrix> lista = evaluationMatrixBusiness.LoadCombo(SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var evaluationMatrixList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
						ModelEvaluationMatrix = new
						{
							Id = x.ModelEvaluationMatrix.Id,
							Description =
							x.ModelEvaluationMatrix.
							Description
						}
					});

					return Json(new { success = true, lista = evaluationMatrixList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Matriz de avaliação não encontrada." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a matriz de avaliação pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}


		public JsonResult LoadComboSimple(long? typeLevelEducation)
		{
			try
			{
				IEnumerable<EvaluationMatrix> lista;

					lista = evaluationMatrixBusiness.LoadComboSimple(SessionFacade.UsuarioLogado.Usuario.ent_id, typeLevelEducation);

				if (lista != null && lista.Count() > 0)
				{
					var evaluationMatrixList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description
					});


					return Json(new { success = true, lista = evaluationMatrixList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Matriz de avaliação não encontrada." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a matriz de avaliação pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult LoadUpdate(long evaluationMatrixId)
		{
			try
			{
				EvaluationMatrix lista = evaluationMatrixBusiness.LoadUpdate(evaluationMatrixId);

				if (lista != null)
				{
					var evaluationMatrixList = new
						{
							Id = lista.Id,
							Description = lista.Description,
							Edition = lista.Edition,
							Visualizar = evaluationMatrixBusiness.ExistsItemMatrix(lista.Id),

							ModelEvaluationMatrix = new
							{
								Id = lista.ModelEvaluationMatrix.Id,
								Description = lista.ModelEvaluationMatrix.Description
							},
							Discipline = new
							{
								Id = lista.Discipline.Id,
								Description = lista.Discipline.Description
							},
							TypeLevelEducation = levelEducationBusiness.GetCustom(lista.Discipline.TypeLevelEducationId),
							Situacao = new
							{
								Id = lista.State,
								Description = lista.State == (Byte)EnumState.ativo ? EnumExtensions.GetDescription(EnumState.ativo) : EnumExtensions.GetDescription(EnumState.inativo)
							},
							EvaluationMatrixCourse = lista.EvaluationMatrixCourse.Select(emc => new
							{
								Id = emc.Id,
								IdCourse = emc.CourseId
							})
						};
					return Json(new { success = true, lista = evaluationMatrixList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Matriz de avaliação não encontrada." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a matriz de avaliação pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion

		#region Write

		[HttpPost]
		public JsonResult Save(EvaluationMatrix entity)
		{
			try
			{
				if (entity.Id > 0)
				{
					entity = evaluationMatrixBusiness.Update(entity.Id, entity);
				}
				else
				{
					entity = evaluationMatrixBusiness.Save(entity);
				}
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = string.Format("Erro ao {0} a matriz de avaliação.", entity.Id > 0 ? "alterar" : "salvar");

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, Id = entity.Id, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Delete(int Id)
		{
			EvaluationMatrix entity = new EvaluationMatrix();

			try
			{
				entity = evaluationMatrixBusiness.Delete(Id);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao tentar excluir a matriz de avaliação.";
				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}