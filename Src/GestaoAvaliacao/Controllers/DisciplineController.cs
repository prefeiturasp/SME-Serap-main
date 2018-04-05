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
	public class DisciplineController : Controller
	{
		private readonly IDisciplineBusiness disciplineBusiness;
		private readonly IACA_TipoNivelEnsinoBusiness levelEducationBusiness;

		public DisciplineController(IDisciplineBusiness disciplineBusiness, IACA_TipoNivelEnsinoBusiness levelEducationBusiness)
		{
			this.disciplineBusiness = disciplineBusiness;
			this.levelEducationBusiness = levelEducationBusiness;
		}

		public ActionResult List()
		{
			return View();
		}

		public ActionResult Form()
		{
			return View();
		}

		#region Read

		[HttpGet]
		public JsonResult Find(int Id)
		{
			Discipline discipline = disciplineBusiness.Get(Id);
			return Json(new { success = true, discipline = discipline }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Paginate]
		public JsonResult Load()
		{
			try
			{
				Pager pager = this.GetPager();
				IEnumerable<Discipline> lista = disciplineBusiness.Load(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var disciplineList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
						LevelEducationDescription = levelEducationBusiness.Get(x.TypeLevelEducationId).tne_nome,
					});

					return Json(new { success = true, lista = disciplineList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Disciplina não encontrada." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a disciplina pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult SearchAllDisciplines()
		{
			try
			{
				IEnumerable<Discipline> lista = disciplineBusiness.SearchAllDisciplines(SessionFacade.UsuarioLogado.Usuario.ent_id);
				if (lista != null && lista.Count() > 0)
				{
					var disciplineTypeList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
					});

					return Json(new { success = true, lista = disciplineTypeList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não existem disciplinas a serem exibidas." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a disciplina pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult SearchDisciplines(int typeLevelEducation)
		{
			try
			{
				IEnumerable<Discipline> lista = disciplineBusiness.SearchDisciplines(typeLevelEducation, SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var disciplineTypeList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
					});

					return Json(new { success = true, lista = disciplineTypeList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não existem disciplinas a serem exibidas." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a disciplina pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult Search(String search)
		{
			try
			{
				Pager pager1 = this.GetPager();
				IEnumerable<Discipline> lista = search != null ? disciplineBusiness.Search(search, ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id) : disciplineBusiness.Load(ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var disciplineList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
						LevelEducationDescription = levelEducationBusiness.Get(x.TypeLevelEducationId).tne_nome,
					});

					return Json(new { success = true, lista = disciplineList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao pesquisar disciplina." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult LoadCustom()
		{
			try
			{
				IEnumerable<Discipline> lista = disciplineBusiness.LoadCustom(SessionFacade.UsuarioLogado.Usuario.ent_id);
				if (lista != null && lista.Count() > 0)
				{
					var disciplineList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
					});

					return Json(new { success = true, lista = disciplineList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Disciplina não encontrada." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a disciplina pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult SearchDisciplinesSaves(int typeLevelEducation)
		{
			try
			{
				IEnumerable<Discipline> lista = disciplineBusiness.SearchDisciplinesSaves(typeLevelEducation, SessionFacade.UsuarioLogado.Usuario.ent_id);
				if (lista != null && lista.Count() > 0)
				{
					var disciplineTypeList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
					});

					return Json(new { success = true, lista = disciplineTypeList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não existem disciplinas a serem exibidas." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar a disciplina pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult LoadWithMatrix()
		{
			try
			{
				IEnumerable<Discipline> lista = disciplineBusiness.LoadComboHasMatrix(SessionFacade.UsuarioLogado.Usuario.ent_id);
				if (lista != null && lista.Count() > 0)
				{
					var disciplineList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
						Url = (x.Description.Substring(0, 3).ToUpper() == "MAT") ?
								ApplicationFacade.ProjectVirtualDirectory + "Assets/images/Matematica.png" :
								(x.Description.ToUpper().Contains("PORT") ? ApplicationFacade.ProjectVirtualDirectory + "Assets/images/Portugues.png" :
									(x.Description.ToUpper().Contains("CIÊNCIA") ? ApplicationFacade.ProjectVirtualDirectory + "Assets/images/Ciencia.png" : null))
					});

					return Json(new { success = true, lista = disciplineList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Componente curricular não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar o componente curricular." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult LoadComboHasMatrix()
		{
			try
			{
				IEnumerable<Discipline> lista = disciplineBusiness.LoadComboHasMatrix(SessionFacade.UsuarioLogado.Usuario.ent_id);
				if (lista != null && lista.Count() > 0)
				{
					var disciplineList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description
					});

					return Json(new { success = true, lista = disciplineList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Componente curricular não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar o componente curricular." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult LoadComboByTest(long Test_id)
		{
			try
			{
				IEnumerable<Discipline> lista = disciplineBusiness.LoadComboByTest(Test_id);
				if (lista != null && lista.Count() > 0)
				{
					var disciplineList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description
					});

					return Json(new { success = true, lista = disciplineList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Componente curricular não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar o componente curricular." }, JsonRequestBehavior.AllowGet);
			}
		}
		public JsonResult GetDisciplinesByTestSubGroup_Id(long TestSubGroup_Id)
		{
			try
			{
				IEnumerable<Discipline> lista = disciplineBusiness.GetDisciplinesByTestSubGroup_Id(TestSubGroup_Id);
				if (lista != null && lista.Count() > 0)
				{
					var disciplineList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description
					});

					return Json(new { success = true, lista = disciplineList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Componente curricular não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar o componente curricular." }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion

		#region Write

		[HttpPost]
		public JsonResult Save(Discipline entity)
		{
			try
			{
				entity = disciplineBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao salvar a disciplina.";

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult SaveRange(List<Discipline> disciplines)
		{
			try
			{
				disciplineBusiness.SaveRange(disciplines, SessionFacade.UsuarioLogado.Usuario.ent_id);
				return Json(new { success = true, message = "Componente(s) curricular(es) salvo(s) com sucesso." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao salvar componente(s) curricular(es)." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public JsonResult Delete(int Id)
		{
			Discipline entity = new Discipline();

			try
			{
				entity = disciplineBusiness.Delete(Id);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao tentar excluir a disciplina.";
				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}