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
        private readonly IACA_AlunoBusiness acaAlunoBusiness;

        public StudentResultsGraphicsController(IESC_EscolaBusiness escolaBusiness, ITUR_TurmaBusiness turmaBusiness,
            ITestBusiness testBusiness, IItemTypeBusiness itemTypeBusiness, ICorrectionBusiness correctionBusiness,
            ICorrectionResultsBusiness correctionResultsBusiness, ITestSectionStatusCorrectionBusiness testSectionStatusCorrectionBusiness, 
            ITestPermissionBusiness testPermissionBusiness, IACA_AlunoBusiness acaAlunoBusiness)
        {
            this.escolaBusiness = escolaBusiness;
            this.turmaBusiness = turmaBusiness;
            this.testBusiness = testBusiness;
            this.itemTypeBusiness = itemTypeBusiness;
            this.correctionBusiness = correctionBusiness;
            this.correctionResultsBusiness = correctionResultsBusiness;
            this.testSectionStatusCorrectionBusiness = testSectionStatusCorrectionBusiness;
            this.testPermissionBusiness = testPermissionBusiness;
            this.acaAlunoBusiness = acaAlunoBusiness;
        }

        // GET: Correction Result
        public ActionResult Index(int Ano, long TestId)
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetDataTest(long TestId, int Ano)
        {
            try
            {
                var permission = await testPermissionBusiness.GetPermissionByTest(TestId, SessionFacade.UsuarioLogado.Grupo.gru_id);
                var permissao = permission.FirstOrDefault();
                if (permissao != null)
                {
                    if ((!permissao.ShowResult) || (!permissao.AllowAnswer))
                    {
                        return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Usuário não possui permissão para realizar essa ação." }, JsonRequestBehavior.AllowGet);
                    }
                }

                var electronicTests = await testBusiness.GetElectronicTestByPesIdAndTestId(SessionFacade.UsuarioLogado.Usuario.pes_id, TestId);
                var tests = await testBusiness.GetTestsByPesId(SessionFacade.UsuarioLogado.Usuario.pes_id);
                var testsFiltereds = tests.Where(d => d.AnoDeAplicacaoDaProva == Ano).ToList();

                if (electronicTests != null)
                {
                    var dados = new
                    {
                        AluId = electronicTests.alu_id,
                        Ano = Ano,
                        AnosDeAplicacaoDaProva = tests.Select(s => s.AnoDeAplicacaoDaProva).Distinct().ToList(),
                        Tests = testsFiltereds,
                        testName = electronicTests.Description,
                        frequencyApplication = electronicTests.FrequencyApplicationText,
                        testDiscipline = electronicTests.Disciplina,
                        testId = electronicTests.Id,
                        schoolName = electronicTests.esc_nome,
                        Turma = electronicTests.Turma,
                        TurId = electronicTests.tur_id,
                        EscId = electronicTests.esc_id,
                        DreId = electronicTests.dre_id
                    };

                    return Json(new { success = true, dados }, JsonRequestBehavior.AllowGet);
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
        public async Task<JsonResult> GetTests(int Ano)
        {
            var dados = await testBusiness.GetTestsByPesId(SessionFacade.UsuarioLogado.Usuario.pes_id);
            dados = dados.Where(d => d.AnoDeAplicacaoDaProva == Ano).ToList();
            return Json(new { success = true, dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetPercentualDeAcerto(long TestId, int EscId, Guid DreId, int TurId)
        {
            try
            {
                //var percentualDeAcerto = new PercentualDeAcertoDTO
                //{
                //    PercentualDeAcertoAluno = 20,
                //    PercentualDeAcertoTurma = 30,
                //    PercentualDeAcertoDRE = 40,
                //    PercentualDeAcertoSME = 50,
                //};
                var testAvgPercentages = correctionResultsBusiness.
                    GetTestAveragesHitsAndPercentagesByTest(TestId, EscId, DreId, TurId, null);

                var aluno = acaAlunoBusiness.GetStudentByPesId(SessionFacade.UsuarioLogado.Usuario.pes_id);

                var peformanceAluno = await correctionResultsBusiness.GetStudentTestInformationByTestAndStudent(TestId, aluno.alu_id);

                var percentualDeAcerto = new PercentualDeAcertoDTO
                {
                    PercentualDeAcertoAluno = peformanceAluno?.Avg ?? 0,
                    PercentualDeAcertoTurma = testAvgPercentages?.AvgTeam ?? 0,
                    PercentualDeAcertoDRE = testAvgPercentages?.AvgDRE ?? 0,
                    PercentualDeAcertoSME = testAvgPercentages?.AvgSME ?? 0,
                };

                if (testAvgPercentages != null)
                {
                    return Json(new { success = true, percentualDeAcerto }, JsonRequestBehavior.AllowGet);
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
    }

    public class PercentualDeAcertoDTO
    {
        public double PercentualDeAcertoAluno { get; set; }
        public double PercentualDeAcertoTurma { get; set; }
        public double PercentualDeAcertoDRE { get; set; }
        public double PercentualDeAcertoSME { get; set; }
    }
}