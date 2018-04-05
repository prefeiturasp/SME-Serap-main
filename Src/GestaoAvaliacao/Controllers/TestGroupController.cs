using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestaoAvaliacao.Controllers
{
	[Authorize]
	[AuthorizeModule]
	public class TestGroupController : Controller
	{
		private readonly ITestGroupBusiness testGroupBusiness;
        private readonly ITestSubGroupBusiness testSubGroupBusiness;
		private readonly ITestCurriculumGradeBusiness testCurriculumGradeBusiness;

		public TestGroupController(ITestGroupBusiness testGroupBusiness, ITestCurriculumGradeBusiness testCurriculumGradeBusiness, ITestSubGroupBusiness testSubGroupBusiness)
		{
			this.testGroupBusiness = testGroupBusiness;
			this.testCurriculumGradeBusiness = testCurriculumGradeBusiness;
            this.testSubGroupBusiness = testSubGroupBusiness;
		}

		public ActionResult Form()
		{
			return View();
		}

		public ActionResult List()
		{
			return View();
		}

		#region Read

		[HttpGet]
		public JsonResult Find(int Id)
		{
			try
			{
				TestGroup testGroup = testGroupBusiness.Get(Id);
				return Json(new { success = true, typeLevelEducation = testGroup }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o grupo de prova pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetTestGroup(long Id)
		{
			try
			{
				TestGroup entity = testGroupBusiness.GetTestGroup(Id);

				if (entity != null)
				{
					var ret = new
					{
						Id = entity.Id,
						Description = entity.Description,
						State = entity.State,
						TestSubGroups = entity.TestSubGroups.Where(i => i.State == (Byte)EnumState.ativo).Select(a => new
						{
							Id = a.Id,
							Description = a.Description,
						})
					};

					return Json(new { success = true, modelList = ret }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Grupo de prova não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o grupo de prova pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult Load()
		{
			try
			{
				IEnumerable<TestGroup> lista = testGroupBusiness.Load(SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var testGroupList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
					});

					return Json(new { success = true, lista = testGroupList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Grupo de prova não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o grupo de prova pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult LoadByPermissionTest()
		{
			try
			{
				TestFilter filter = new TestFilter();
				var visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString());

				if (filter == null)
				{
					filter = new TestFilter();
					if (visao == EnumSYS_Visao.Administracao)
						filter.global = true;
					else if (visao != EnumSYS_Visao.Individual)
						filter.global = false;
				}

				filter.ent_id = SessionFacade.UsuarioLogado.Usuario.ent_id;
				filter.gru_id = SessionFacade.UsuarioLogado.Grupo.gru_id;
				filter.pes_id = SessionFacade.UsuarioLogado.Usuario.pes_id;
				filter.usuId = SessionFacade.UsuarioLogado.Usuario.usu_id;
				filter.vis_id = visao;
				IEnumerable<TestGroup> lista = testGroupBusiness.LoadByPermissionTest(filter);

				if (lista != null && lista.Count() > 0)
				{
					var testGroupList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
					});

					return Json(new { success = true, lista = testGroupList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Grupo de prova não encontrado." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o grupo de prova pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult LoadPaginate()
		{
			try
			{
				Pager pager = this.GetPager();

				IEnumerable<TestGroup> lista = testGroupBusiness.LoadPaginate(ref pager, SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var testGroupList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
						modelState = x.State,
						LevelCount = x.TestSubGroups.Where(c => c.State == (Byte)EnumState.ativo).Count()
					}).ToList();

					return Json(new { success = true, lista = testGroupList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o(s) grupo(s) de prova pesquisado(s)." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[Paginate]
		public JsonResult Search(String search = null, int levelQntd = 0)
		{
			try
			{
				Pager pager1 = this.GetPager();
				IEnumerable<TestGroup> lista = ((search != null && !string.IsNullOrEmpty(search)) || levelQntd != 0) ? testGroupBusiness.Search(ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id, search, levelQntd) : testGroupBusiness.LoadPaginate(ref pager1, SessionFacade.UsuarioLogado.Usuario.ent_id);

				if (lista != null && lista.Count() > 0)
				{
					var testGroupList = lista.Select(x => new
					{
						Id = x.Id,
						Description = x.Description,
						modelState = x.State,
						levelCount = x.TestSubGroups.Where(c => c.State == (Byte)EnumState.ativo).Count()
					}).ToList();

					return Json(new { success = true, lista = testGroupList }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o(s) grupo(s) de prova pesquisado(s)." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult LoadGroupsSubGroups()
		{
			try
			{
				IEnumerable<TestGroup> lstTestGroup = testGroupBusiness.LoadGroupsSubGroups(SessionFacade.UsuarioLogado.Usuario.ent_id);
				return Json(new { success = true, groupSubGroup = lstTestGroup }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o grupo de prova pesquisado." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult VerifyDeleteSubGroup(long Id)
		{
			try
			{
				bool permiteExcluir = testGroupBusiness.VerifyDeleteSubGroup(Id);
				return Json(new { success = true, permiteExcluir = permiteExcluir }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar excluir subgrupo." }, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpGet]
		public JsonResult GetDistinctCurricumGradeByTestSubGroup_Id(long TestSubGroup_Id)
		{
			try
			{
				var curriculumGrade = testCurriculumGradeBusiness.GetDistinctCurricumGradeByTestSubGroup_Id(TestSubGroup_Id);

				return Json(new { success = true, lista = curriculumGrade }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao obter ano(s) de aplicação da prova" }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion

		#region Write

		[HttpPost]
		public JsonResult Save(TestGroup entity)
		{
			try
			{
				if (entity.Id > 0)
				{
					entity = testGroupBusiness.Update(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
				}
				else
				{
					entity = testGroupBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.ent_id);
				}
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = string.Format("Erro ao {0} o grupo de prova.", entity.Id > 0 ? "alterar" : "salvar");

				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Delete(long Id)
		{
			TestGroup entity = new TestGroup();

			try
			{
				entity = testGroupBusiness.Delete(Id, SessionFacade.UsuarioLogado.Usuario.ent_id);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao tentar excluir o grupo de prova.";
				LogFacade.SaveError(ex);
			}

			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

        [HttpPost]
        public JsonResult ChangeOrder(long idOrigem, long idDestino)
        {
            TestSubGroup entity = new TestSubGroup();
            try
            {
                testSubGroupBusiness.ChangeOrder(idOrigem, idDestino);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao alterar a ordem do grupo.";

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, TestID = entity.Id }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}