using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.IBusiness;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class ReportStudentPerformanceController : Controller
    {
        private readonly IReportStudentPerformanceBusiness _reportStudentPerformanceBusiness;
        private readonly ITUR_TurmaBusiness _turmaBusiness;
        private readonly ITestBusiness _testBusiness;

        public ReportStudentPerformanceController(IReportStudentPerformanceBusiness reportStudentPerformanceBusiness,
                                                  ITUR_TurmaBusiness turmaBusiness,
                                                  ITestBusiness testBusiness)
        {
            _reportStudentPerformanceBusiness = reportStudentPerformanceBusiness;
            _turmaBusiness = turmaBusiness;
            _testBusiness = testBusiness;
        }

        public ActionResult Index(int? esc_id, long? team_id, long? test_id, long? alu_id, Guid? dre_id)
        {
            if (esc_id.HasValue && team_id.HasValue && test_id.HasValue && alu_id.HasValue && dre_id.HasValue)
            {
                if (VerifyTeacherPermission((long)test_id, (long)team_id))
                    return View();
                else
                    return RedirectToAction("index", "home");
            }
            else
                return RedirectToAction("index", "home");
        }

        /// <summary>
        /// Verifica se o professor tem permissão para acessar a tela
        /// </summary>
        /// <param name="test_id">Id da prova</param>
        /// <param name="team_id">Id da turma</param>
        private bool VerifyTeacherPermission(long test_id, long team_id)
        {
            bool isValid = true;

            var test = _testBusiness.GetTestById(test_id);

            if (test != null)
            {
                if ((EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id == EnumSYS_Visao.Individual &&
                    test.UsuId != SessionFacade.UsuarioLogado.Usuario.usu_id &&
                    !_turmaBusiness.ValidateTeacherSection(team_id, SessionFacade.UsuarioLogado.Usuario.pes_id))
                {
                    isValid = false;
                }
            }
            else
                isValid = false;

            return isValid;
        }

        /// <summary>
        /// Busca informações de identificação do alu e relacionadas ao desempenho do aluno na prova
        /// </summary>
        /// <param name="test_id">Id da prova</param>
        /// <param name="alu_id">Id do aluno</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <returns>DTO com todas as informações do aluno</returns>
        [HttpGet]
        public async Task<JsonResult> GetStudentInformation(long test_id, long alu_id, Guid dre_id)
        {
            try
            {
                var studentInformation = await _reportStudentPerformanceBusiness.GetStudentInformation(test_id, alu_id, dre_id);

                if (studentInformation != null)
                {
                    return Json(new { success = true, data = studentInformation }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), data = "Não foi possível buscar os dados do aluno." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar buscar os dados do aluno." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Busca as informações das unidades(SME, DRE, escola, turma) em relação ao desempenho na prova 
        /// </summary>
        /// <param name="test_id">Id da prova</param>
        /// <param name="esc_id">Id da escola</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="team_id">Id da turma</param>
        /// <returns>DTO com as informações das unidades(SME, DRE, escola, turma) em relação ao desempenho na prova</returns>
        [HttpGet]
        public async Task<JsonResult> GetUnitsInformation(long test_id, int esc_id, Guid dre_id, long team_id)
        {
            try
            {
                var unitsInformation = await _reportStudentPerformanceBusiness.GetUnitsInformation(test_id, esc_id, dre_id, team_id);

                if (unitsInformation != null)
                {
                    return Json(new { success = true, data = unitsInformation }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), data = "Não foi possível buscar os dados das unidades." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar buscar os dados das unidades." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Exporta o relatório para CSV
        /// </summary>
        /// <param name="test_id">Id da prova</param>
        /// <param name="alu_id">Id do aluno</param>
        /// <param name="dre_id">Id da DRE</param>
        /// <param name="esc_id">Id da escola</param>
        /// <param name="team_id">Id da turma</param>
        /// <returns>Dados para download do CSV</returns>
        [HttpGet]
        public async Task<JsonResult> ExportReport(int test_id, long alu_id, Guid dre_id, int esc_id, long team_id)
        {
            EntityFile ret = new EntityFile();

            try
            {
                string separator = ApplicationFacade
                                    .Parameters
                                    .FirstOrDefault(p => p.Key.Equals(EnumParameterKey.CHAR_SEP_CSV.GetDescription()))
                                    .Value ?? ";";

                ret = await _reportStudentPerformanceBusiness.ExportReport(test_id, alu_id, dre_id, esc_id, team_id, separator,
                                                                  ApplicationFacade.VirtualDirectory, ApplicationFacade.PhysicalDirectory, (EnumSYS_Visao)SessionFacade.UsuarioLogado.Grupo.vis_id);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                ret.Validate.Type = ValidateType.error.ToString();
                ret.Validate.IsValid = false;
                ret.Validate.Message = "Erro ao obter os dados.";
            }

            return Json(new { success = ret.Validate.IsValid, type = ret.Validate.Type, message = ret.Validate.Message, file = ret }, JsonRequestBehavior.AllowGet);
        }
    }
}