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
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class StudentResultsGraphicsController : Controller
    {
        private readonly IESC_EscolaBusiness escolaBusiness;
        private readonly ITUR_TurmaBusiness turmaBusiness;
        private readonly ITestBusiness testBusiness;
        private readonly IItemTypeBusiness itemTypeBusiness;
        private readonly ICorrectionBusiness correctionBusiness;
        private readonly ICorrectionResultsBusiness correctionResultsBusiness;
        private readonly ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness;
        private readonly ITestPermissionBusiness testPermissionBusiness;

        public StudentResultsGraphicsController(IESC_EscolaBusiness escolaBusiness, ITUR_TurmaBusiness turmaBusiness,
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

        [HttpGet]
        public async Task<JsonResult> GetDataTest(long test_id)
        {
            try
            {
                var permission = await testPermissionBusiness.GetPermissionByTest(test_id, SessionFacade.UsuarioLogado.Grupo.gru_id);
                var permissao = permission.FirstOrDefault();
                if (permissao != null)
                {
                    if ((!permissao.ShowResult) || (!permissao.AllowAnswer))
                    {
                        return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Usuário não possui permissão para realizar essa ação." }, JsonRequestBehavior.AllowGet);
                    }
                }

                var electronicTests = await testBusiness.GetElectronicTestByPesIdAndTestId(SessionFacade.UsuarioLogado.Usuario.pes_id, test_id);

                if (electronicTests != null)
                {
                    var dados = new
                    {
                        testName = electronicTests.Description,
                        frequencyApplication = electronicTests.FrequencyApplicationText,
                        testDiscipline = electronicTests.Disciplina,
                        testId = electronicTests.Id,
                        schoolName = electronicTests.esc_nome,
                    };

                    return Json(new { success = true, dados}, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetPercentualDeAcerto(long test_id)
        {
            try
            {
                var percentualDeAcerto = new PercentualDeAcertoDTO
                {
                    PercentualDeAcertoAluno = 20,
                    PercentualDeAcertoTurma = 30,
                    PercentualDeAcertoDRE = 40,
                    PercentualDeAcertoSME = 50,
                };

                return Json(new { success = true, percentualDeAcerto }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar os alunos da turma." }, JsonRequestBehavior.AllowGet);
            }

        }
    }

    public class PercentualDeAcertoDTO 
    {
        public int PercentualDeAcertoAluno { get; set; }
        public int PercentualDeAcertoTurma { get; set; }
        public int PercentualDeAcertoDRE { get; set; }
        public int PercentualDeAcertoSME { get; set; }
    }
}