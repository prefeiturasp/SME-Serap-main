using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Models;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.IBusiness;
using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
	[AuthorizeModule]
	public class AdherenceController : Controller
	{
		private readonly IESC_EscolaBusiness escolaBusiness;
		private readonly IACA_TipoTurnoBusiness tipoTurnoBusiness;
		private readonly ISYS_UnidadeAdministrativaBusiness unidadeAdministrativaBusiness;
		private readonly IAdherenceBusiness adherenceBusiness;
		private readonly ITestBusiness testBusiness;
		private readonly ITestCurriculumGradeBusiness testCurriculumGradeBusiness;

		public AdherenceController(IESC_EscolaBusiness escolaBusiness, IACA_TipoTurnoBusiness tipoTurnoBusiness,
			ISYS_UnidadeAdministrativaBusiness unidadeAdministrativaBusiness, IAdherenceBusiness adherenceBusiness, ITestBusiness testBusiness,
			ITestCurriculumGradeBusiness testCurriculumGradeBusiness)
		{
			this.escolaBusiness = escolaBusiness;
			this.tipoTurnoBusiness = tipoTurnoBusiness;
			this.unidadeAdministrativaBusiness = unidadeAdministrativaBusiness;
			this.adherenceBusiness = adherenceBusiness;
			this.testBusiness = testBusiness;
			this.testCurriculumGradeBusiness = testCurriculumGradeBusiness;
		}

		#region View

		public ActionResult Index(long test_id)
		{
			var test = testBusiness.GetObjectToAdherence(test_id);
			var visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString());

            ViewBag.testOwner = DateTime.Today <= test.ApplicationEndDate &&
                                (test.UsuId.Equals(SessionFacade.UsuarioLogado.Usuario.usu_id) ||
                                 (test.Global && visao == EnumSYS_Visao.Administracao));

			var dados = new
			{
				//Apenas será disponivel aderir se estiver no periodo de aplicação e o usuario logado for o dono da prova ou a prova for global e o usuário admin
                ViewBag.testOwner,
				testName = test.TestDescription,
                frequencyApplication = test.FrequencyApplicationDescription,
				testDiscipline = test.DisciplineDescription,
				testId = test.Id,
				testAllAdhered = test.AllAdhered,
				token = JwtHelper.CreateToken(SessionFacade.UsuarioLogado.Usuario.usu_id.ToString(), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString(),
					SessionFacade.UsuarioLogado.Usuario.pes_id.ToString(), SessionFacade.UsuarioLogado.Usuario.ent_id.ToString(), test.Id.ToString()),
				global = test.Global,
			};

			ViewBag.dados = JsonConvert.SerializeObject(dados);
			ViewBag.typeSelection = EnumExtensions.EnumToJson<GestaoAvaliacao.Entities.Enumerator.EnumAdherenceSelection>();
			ViewBag.typeEntity = EnumExtensions.EnumToJson<GestaoAvaliacao.Entities.Enumerator.EnumAdherenceEntity>();
			ViewBag.global = test.Global;
			ViewBag.visao = visao;

			return View();
		}

		#endregion

		#region Write

		[HttpPost]
		public JsonResult SwitchAllAdhrered(long test_id, bool allAdhered)
		{
			Adherence entity = new Adherence();
			try
			{
				entity = adherenceBusiness.SwitchAllAdhrered(SessionFacade.UsuarioLogado.Usuario.usu_id, test_id, allAdhered,
					(EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()));
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = string.Format("Erro ao {0} a adesão da prova.", entity.Id > 0 ? "alterar" : "salvar");

				LogFacade.SaveError(ex);
			}
			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult Save(AdherenceSaveModel model)
		{
			return Json(new { a = true });
		}

		#endregion

		#region Read

		[HttpGet]
		public JsonResult GetDRESimple(string dre_id)
		{
			try
			{
                var lista = unidadeAdministrativaBusiness.LoadDRESimple(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);

                if (lista != null && lista.Count() > 0)
                {
                    var ret = lista.Select(dre => new DropDownReturnModel()
                    {
                        Id = dre.uad_id.ToString(),
                        Description = dre.uad_nome,
                        Code = dre.uad_codigo
                    });

                    if (!string.IsNullOrEmpty(dre_id))
                        ret = ret.Where(r => r.Id.Equals(dre_id));

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json(new { success = true, lista = "" }, JsonRequestBehavior.AllowGet);
                }
            }
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar DREs pesquisadas." }, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpGet]
        public JsonResult GetAdheredDreSimple(long testId)
        {
            try
            {
                var lista = adherenceBusiness.GetAdheredDreSimple(testId, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo, 0, 0);

                if (lista != null && lista.Count() > 0)
                {
                    var ret = lista.Select(dre => new DropDownReturnModel()
                    {
                        Id = dre.EntityId.ToString(),
                        Description = dre.Description
                    });

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json(new { success = true, lista = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar DREs pesquisadas." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
		[Paginate]
		public JsonResult GetSchoolsGrid(string dre_id, int esc_id, int ttn_id, long test_id, int crp_ordem)
		{
			try
			{
				Pager pager = this.GetPager();
				var escolas = adherenceBusiness.LoadSchool(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo, ref pager,
					string.IsNullOrEmpty(dre_id) ? Guid.Empty : new Guid(dre_id), esc_id, ttn_id, test_id, crp_ordem);

				var retorno = escolas.Select(e => new
				{
					DRE = e.uad_nome,
					Id = e.esc_id.ToString(),
					Description = e.esc_nome,
					Status = e.TypeSelection.Value,
					Selected = e.TypeSelection.Value != Entities.Enumerator.EnumAdherenceSelection.NotSelected
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar escolas pesquisadas." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetSchoolsSimple(string dre_id)
		{
			try
			{
				var escolas = escolaBusiness.LoadSimple(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo,
					string.IsNullOrEmpty(dre_id) ? Guid.Empty : new Guid(dre_id));

				var retorno = escolas.Select(e => new DropDownReturnModel
				{
					Id = e.esc_id.ToString(),
					Description = e.esc_nome
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as escolas." }, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpGet]
        public JsonResult GetAdheredSchoolSimple(long testId, string dre_id)
        {
            try
            {
                var escolas = adherenceBusiness.GetAdheredSchoolSimple(testId, string.IsNullOrEmpty(dre_id) ? Guid.Empty : new Guid(dre_id), SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo, 0, 0);

                var retorno = escolas.Select(e => new DropDownReturnModel
                {
                    Id = e.EntityId.ToString(),
                    Description = e.Description
                });

                return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar as escolas." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
		public JsonResult GetCurriculumGradeSimple(int esc_id, long test_id)
		{
			try
			{
				var curriculoPeriodo = testCurriculumGradeBusiness.GetSimple(test_id, esc_id);

				var retorno = curriculoPeriodo.Select(e => new DropDownReturnModel
				{
					Id = e.crp_ordem.ToString(),
					Description = e.crp_descricao
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os anos do curso" }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetShiftSimple(int esc_id)
		{
			try
			{
				var escolas = tipoTurnoBusiness.Load(esc_id);

				var retorno = escolas.Select(e => new DropDownReturnModel
				{
					Id = e.ttn_id.ToString(),
					Description = e.ttn_nome
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os turnos." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetSectionGrid(int esc_id, int ttn_id, long test_id, int crp_ordem)
		{
			try
			{
				var escolas = adherenceBusiness.LoadSection(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo, esc_id, ttn_id, test_id, crp_ordem);

				var retorno = escolas.Select(e => new
				{
					Id = e.esc_id,
					Description = e.esc_nome,
					Status = e.TypeSelection.Value,
					Turno = e.uad_nome,
					Selected = e.TypeSelection.Value != Entities.Enumerator.EnumAdherenceSelection.NotSelected,
                    Open = false
				});

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar turmas pesquisadas." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetTotalSelected(long test_id)
		{
			try
			{
				int totalSchool, totalSelectedSchool, totalSelectedSection;

				adherenceBusiness.GetTotalSelected(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo,
					test_id, out totalSchool, out totalSelectedSchool, out totalSelectedSection);

				var retorno = new
				{
					totalSchool,
					totalSelectedSchool,
					totalSelectedSection

				};

				return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar prova pesquisada." }, JsonRequestBehavior.AllowGet);
			}
		}


		[HttpGet]
		[Paginate]
		public JsonResult GetSelectedSchool(long test_id, string dre_id, int esc_id, int ttn_id, int crp_ordem)
		{
			try
			{
				Pager pager = this.GetPager();

				var escolas = adherenceBusiness.LoadSelectedSchool(ref pager, string.IsNullOrEmpty(dre_id) ? Guid.Empty : new Guid(dre_id), esc_id, ttn_id, test_id, crp_ordem,
					SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);

				var retorno = escolas.Select(e => new
				{
					DRE = e.uad_nome,
					Id = e.esc_id,
					Description = e.esc_nome,
					Status = e.TypeSelection.Value,
					Selected = e.TypeSelection.Value != Entities.Enumerator.EnumAdherenceSelection.NotSelected
				});

				if (retorno.Count() > 0)
					return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
				else
					return Json(new { success = true, type = ValidateType.alert.ToString(), message = "Não foram encontradas escolas aderidas." }, JsonRequestBehavior.AllowGet);


			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar escolas pesquisadas." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetSelectedSection(long test_id, int esc_id, int ttn_id, int crp_ordem)
		{
			try
			{
				var escolas = adherenceBusiness.LoadSelectedSection(SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo, esc_id, ttn_id, test_id, crp_ordem);

				var retorno = escolas.Select(e => new
				{
					Id = e.esc_id,
					Description = e.esc_nome,
					Status = e.TypeSelection.Value,
					Turno = e.uad_nome,
					Selected = e.TypeSelection.Value
				});
				if (retorno.Count() > 0)
					return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
				else
					return Json(new { success = true, type = ValidateType.alert.ToString(), message = "Não foram encontradas turmas aderidas." }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar turmas pesquisadas." }, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpGet]
        public JsonResult GetStudentsGrid(long team_id, long test_id)
        {
            try
            {
                var alunos = adherenceBusiness.LoadStudent(team_id, test_id);

                var retorno = alunos.Select(a => new
                {
                    Id = a.alu_id,
                    Description = a.alu_nome,
                    Status = a.TypeSelection,
                    Selected = a.TypeSelection != null && a.TypeSelection.Value != Entities.Enumerator.EnumAdherenceSelection.NotSelected && a.TypeSelection.Value != Entities.Enumerator.EnumAdherenceSelection.Blocked,
                    Open = false,
					a.Alu_Matricula
                });

                return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os alunos da turma." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetSelectedStudents(long team_id, long test_id)
        {
            try
            {
                var alunos = adherenceBusiness.LoadSelectedStudent(team_id, test_id);

                var retorno = alunos.Select(a => new
                {
                    Id = a.alu_id,
                    Description = a.alu_nome +
                                  (a.TypeSelection != null && a.TypeSelection.Value == Entities.Enumerator.EnumAdherenceSelection.Blocked
                                      ? " (Bloqueado)"
                                      : ""),
                    Status = a.TypeSelection,
                    Selected = a.TypeSelection != null && a.TypeSelection.Value != Entities.Enumerator.EnumAdherenceSelection.NotSelected &&
                               a.TypeSelection.Value != Entities.Enumerator.EnumAdherenceSelection.Blocked,
                    a.Alu_Matricula
                }).ToList();

                return retorno.Any()
                    ? Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet)
                    : Json(
                        new
                        {
                            success = true,
                            type = ValidateType.alert.ToString(),
                            message = "Não foram encontrados alunos aderidos."
                        }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os alunos da turma." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}