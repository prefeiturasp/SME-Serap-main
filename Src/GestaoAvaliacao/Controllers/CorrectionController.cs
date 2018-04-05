using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Controllers
{
	[Authorize]
	[AuthorizeModule]
	public class CorrectionController : Controller
	{
		private readonly IESC_EscolaBusiness escolaBusiness;
		private readonly ITUR_TurmaBusiness turmaBusiness;
		private readonly ITestBusiness testBusiness;
		private readonly IItemTypeBusiness itemTypeBusiness;
		private readonly ICorrectionBusiness correctionBusiness;
		private readonly ICorrectionResultsBusiness correctionResultsBusiness;
		private readonly ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness;
		private readonly ITestPermissionBusiness testPermissionBusiness;

		public CorrectionController(IESC_EscolaBusiness escolaBusiness, ITUR_TurmaBusiness turmaBusiness,
			ITestBusiness testBusiness, IItemTypeBusiness itemTypeBusiness, ICorrectionBusiness correctionBusiness,
			ICorrectionResultsBusiness correctionResultsBusiness, ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness, ITestPermissionBusiness testPermissionBusiness)
		{
			this.escolaBusiness = escolaBusiness;
			this.turmaBusiness = turmaBusiness;
			this.testBusiness = testBusiness;
			this.itemTypeBusiness = itemTypeBusiness;
			this.correctionBusiness = correctionBusiness;
			this.correctionResultsBusiness = correctionResultsBusiness;
			this.testSectionStatusCorrectionBusiness = testSectionStatusCorrectionBusiness;
			this.testPermissionBusiness = testPermissionBusiness;
		}

		// GET: Correction Result
		public ActionResult Index(long test_id, long team_id)
		{
			return View();
		}

		// GET: Correction
		public ActionResult IndexForm(long test_id, long team_id)
		{
			return View();
		}

		[HttpGet]
		public JsonResult GetAuthorize(long test_id, long team_id, bool result)
		{
			try
			{
				var permission = testPermissionBusiness.GetByTest(test_id, SessionFacade.UsuarioLogado.Grupo.gru_id).FirstOrDefault();
				if (permission != null)
				{
					if ((result && !permission.ShowResult) || (!result && !permission.AllowAnswer))
					{
						return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Usuário não possui permissão para realizar essa ação." }, JsonRequestBehavior.AllowGet);
					}
				}

				var test = testBusiness.GetTestById(test_id);
				var turma = turmaBusiness.Get(team_id);

				TestSectionStatusCorrection testsection = new TestSectionStatusCorrection();
				if (result)
				{
					testsection = testSectionStatusCorrectionBusiness.Get(test_id, team_id);
				}

				if (test != null && turma != null)
				{
					var escola = escolaBusiness.Get(turma.esc_id);

					if (test.TestType.ItemType_Id.HasValue)
						test.TestType.ItemType = itemTypeBusiness.Get(test.TestType.ItemType_Id.Value);

					bool blockCorrection = result;
					if (test.TestType.Global)
					{
						if (!result && (EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id != EnumSYS_Visao.Administracao)
						{
							blockCorrection = true;
						}
						else if (result && (EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id == EnumSYS_Visao.Administracao && testsection != null && testsection.StatusCorrection == EnumStatusCorrection.Success)
						{
							blockCorrection = false;
						}
					}
					else
					{
						if (!result && test.UsuId != SessionFacade.UsuarioLogado.Usuario.usu_id)
						{
							blockCorrection = true;
						}
						else if (result && test.UsuId == SessionFacade.UsuarioLogado.Usuario.usu_id && testsection != null && testsection.StatusCorrection == EnumStatusCorrection.Success)
						{
							blockCorrection = false;
						}
					}

					bool blockAccess = false;
					if ((EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id == EnumSYS_Visao.Individual &&
						test.UsuId != SessionFacade.UsuarioLogado.Usuario.usu_id &&
						!turmaBusiness.ValidateTeacherSection(team_id, SessionFacade.UsuarioLogado.Usuario.pes_id))
					{
						blockAccess = true;
					}

					var dados = new
					{
						testOwner = test.UsuId.Equals(SessionFacade.UsuarioLogado.Usuario.usu_id),
						testName = test.Description,
						frequencyApplication = EnumHelper.GetFrenquencyApplication(test.FrequencyApplication, test.TestType.FrequencyApplication, true, true),
						testDiscipline = test.Discipline.Description,
						testId = test.Id,
						team = new { Id = turma.tur_id, Name = turma.tur_codigo, esc_id = turma.esc_id },
						schoolName = escola.esc_nome,
						numberAnswer = test.TestType.ItemType != null && test.TestType.ItemType.QuantityAlternative.HasValue ? test.TestType.ItemType.QuantityAlternative.Value : 0,
						blockCorrection = blockCorrection,
						blockAccess = blockAccess,
						InCorrection = Compare.ValidateBetweenShortDates(Convert.ToDateTime(test.CorrectionStartDate), Convert.ToDateTime(test.CorrectionEndDate)),
						answerSheetBlocked = test.TestType.ItemType == null || test.TestType.ItemType.QuantityAlternative == null,
						token = Util.JwtHelper.CreateToken(SessionFacade.UsuarioLogado.Usuario.usu_id.ToString(), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString(),
							SessionFacade.UsuarioLogado.Usuario.pes_id.ToString(), SessionFacade.UsuarioLogado.Usuario.ent_id.ToString(),
							string.Format("{0}_{1}", test.Id, team_id)),
						access_token = Util.JwtHelper.CreateToken(SessionFacade.UsuarioLogado.Usuario.usu_id.ToString(), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString(),
							SessionFacade.UsuarioLogado.Usuario.pes_id.ToString(), SessionFacade.UsuarioLogado.Usuario.ent_id.ToString(),
							string.Format("{0}_{1}", test.Id, team_id)),
						token_type = "Bearer",
						expires_in = double.Parse(ConfigurationManager.AppSettings["MinutesLifetimeToken"]) * 1000,
                        electronicTest = test.ElectronicTest
					};

					var visao = (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString());

					var token = Util.JwtHelper.CreateToken(SessionFacade.UsuarioLogado.Usuario.usu_id.ToString(), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString(),
							SessionFacade.UsuarioLogado.Usuario.pes_id.ToString(), SessionFacade.UsuarioLogado.Usuario.ent_id.ToString(),
							string.Format("{0}_{1}", test.Id, team_id));

					return Json(new { success = true, dados = dados, visao = visao, token = token }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não foi possível carregar os dados." }, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar dados." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public async Task<JsonResult> GetStudentBySection(long team_id, long test_id)
		{
			try
			{
				List<CorrectionStudentGrid> lista = await correctionBusiness.GetGridCorrection(team_id, test_id);

				if (lista != null && lista.Count() > 0)
				{
					var ret = lista.Select(i => new
					{
						alu_id = i.alu_id,
						alu_nome = i.alu_nome,
						AbsenceReason_id = i.AbsenceReason_id,
						mtu_numeroChamada = i.mtu_numeroChamada,
						blocked = i.blocked,
						TotalAnswers = i.QtdeItem,
						TotalAnswered = i.Correcteds,
						Items = i.Items,
						StudentAnswerKey = new
						{
							Warning = i.Items.Count(item => item.Null || item.StrikeThrough) > 0 && i.Automatic
						},
                        Status = EnumHelper.GetDescriptionFromEnumValue(i.status)
					});

					return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = true, lista = "" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os alunos da turma." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public JsonResult GetTestAveragesPercentagesByTest(long test_id, int? esc_id, Guid? dre_id, long? team_id, long? discipline_id)
		{
			try
			{
				var testAvgPercentages = correctionResultsBusiness.GetTestAveragesHitsAndPercentagesByTest(test_id, esc_id, dre_id, team_id, discipline_id);

				if (testAvgPercentages != null)
				{
					return Json(new { success = true, data = testAvgPercentages }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), data = "Não foi possível buscar as médias dessa avaliação" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar buscar as médias da avaliação." }, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpGet]
		public JsonResult GetAveragesByTest(long test_id, int? esc_id, Guid? dre_id, long? tcp_id, long? discipline_id)
		{
			try
			{
				var testAvgPercentages = correctionResultsBusiness.GetAveragesByTest(test_id, esc_id, dre_id, tcp_id, discipline_id, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);

				if (testAvgPercentages != null)
				{
					return Json(new { success = true, data = testAvgPercentages }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), data = "Não foi possível buscar as médias dessa avaliação" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar buscar as médias da avaliação." }, JsonRequestBehavior.AllowGet);
			}
		}
		[HttpGet]
		public JsonResult GetAveragesByTestSubGroup_Id(long subgroup_id, int? esc_id, Guid? dre_id, long? tcp_id, long? discipline_id)
		{
			try
			{
				var testAvgPercentages = correctionResultsBusiness.GetAveragesByTestSubGroup_Id(subgroup_id, esc_id, dre_id, tcp_id, discipline_id, SessionFacade.UsuarioLogado.Usuario, SessionFacade.UsuarioLogado.Grupo);

				if (testAvgPercentages != null)
				{
					return Json(new { success = true, data = testAvgPercentages }, JsonRequestBehavior.AllowGet);
				}
				else
					return Json(new { success = false, type = ValidateType.alert.ToString(), data = "Não foi possível buscar as médias dessa avaliação" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar buscar as médias da avaliação." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public async Task<JsonResult> GetStudentAnswer(long team_id, long test_id, long alu_id)
		{
			try
			{
				var lista = await correctionBusiness.GetStudentAnswer(test_id, team_id, alu_id, SessionFacade.UsuarioLogado.Usuario.ent_id);

				return Json(new { success = true, lista = lista }, JsonRequestBehavior.AllowGet);

			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os alunos da turma." }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public async Task<JsonResult> GetResultCorrectionGrid(long team_id, long test_id, long? discipline_id)
		{
			CorrectionResults entity;

			try
			{
				entity = await correctionBusiness.GetResultCorrectionGrid(team_id, test_id, discipline_id, SessionFacade.UsuarioLogado.Usuario.ent_id);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os alunos da turma." }, JsonRequestBehavior.AllowGet);
			}

			return Json(new { success = true, entity }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public async Task<JsonResult> UnblockCorrection(long team_id, long test_id)
		{
			var entity = new CorrectionResults();
			try
			{
				entity = await correctionResultsBusiness.UnblockCorrection(team_id, test_id, SessionFacade.UsuarioLogado.Usuario, (EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id);
			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao alterar a prova.";

				LogFacade.SaveError(ex);
			}
			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public async Task<JsonResult> FinalizeCorrection(long team_id, long test_id)
		{
			Adherence entity = new Adherence();
			try
			{
				entity = await correctionBusiness.FinalizeCorrection(team_id, test_id, SessionFacade.UsuarioLogado.Usuario, (EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id);

			}
			catch (Exception ex)
			{
				entity.Validate.IsValid = false;
				entity.Validate.Type = ValidateType.error.ToString();
				entity.Validate.Message = "Erro ao finalizar a correção.";

				LogFacade.SaveError(ex);
			}
			return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public async Task<ActionResult> GetResultExport(long test_id, long team_id, long? discipline_id)
		{
			EntityFile ret = new EntityFile();

			try
			{
				string separator = ";";
				Parameter param = ApplicationFacade.Parameters.FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()));
				if (param != null)
					separator = param.Value;

				ret = await correctionResultsBusiness.GetResultFile(SessionFacade.UsuarioLogado.Usuario.ent_id, test_id, team_id, discipline_id,
					separator, ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory);
			}
			catch (Exception ex)
			{
				LogFacade.SaveError(ex);
				ret.Validate.Type = ValidateType.error.ToString();
				ret.Validate.IsValid = false;
				ret.Validate.Message = "Erro ao obter o resultado de prova.";
			}

			return Json(new { success = ret.Validate.IsValid, type = ret.Validate.Type, message = ret.Validate.Message, file = ret }, JsonRequestBehavior.AllowGet);
		}
	}
}