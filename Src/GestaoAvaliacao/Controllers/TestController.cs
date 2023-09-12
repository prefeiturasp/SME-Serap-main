using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.Models;
using GestaoAvaliacao.Util;
using GestaoAvaliacao.Util.Extensions;
using GestaoAvaliacao.WebProject.Facade;
using GestaoEscolar.Entities;
using GestaoEscolar.IBusiness;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static IdentityModel.Client.OAuth2Constants;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Controllers
{
    [Authorize]
    [AuthorizeModule]
    public class TestController : Controller
    {
        private readonly ITestBusiness testBusiness;
        private readonly ITestFilesBusiness testFilesBusiness;
        private readonly IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness;
        private readonly IBlockBusiness blockBusiness;
        private readonly IFileBusiness fileBusiness;
        private readonly ICorrectionBusiness correctionBusiness;
        private readonly IRequestRevokeBusiness requestRevokeBusiness;
        private readonly IExportAnalysisBusiness exportAnalysisBusiness;
        private readonly IESC_EscolaBusiness escolaBusiness;
        private readonly ITestCurriculumGradeBusiness testCurriculumGradeBusiness;
        private readonly ITestPermissionBusiness testPermissionBusiness;
        private readonly ITestContextBusiness testContextBusiness;
        private readonly IBlockChainBusiness blockChainBusiness;
        private readonly IBlockChainBlockBusiness blockChainBlockBusiness;

        public TestController(ITestBusiness testBusiness, ITestFilesBusiness testFilesBusiness, IACA_TipoCurriculoPeriodoBusiness tipoCurriculoPeriodoBusiness,
            IBlockBusiness blockBusiness, IFileBusiness fileBusiness, ICorrectionBusiness correctionBusiness, IRequestRevokeBusiness requestRevokeBusiness,
            IExportAnalysisBusiness exportAnalysisBusiness, IESC_EscolaBusiness escolaBusiness, ITestCurriculumGradeBusiness testCurriculumGradeBusiness,
            ITestPermissionBusiness testPermissionBusiness, ITestContextBusiness testContextBusiness, IBlockChainBusiness blockChainBusiness,
            IBlockChainBlockBusiness blockChainBlockBusiness)
        {
            this.testBusiness = testBusiness;
            this.testFilesBusiness = testFilesBusiness;
            this.tipoCurriculoPeriodoBusiness = tipoCurriculoPeriodoBusiness;
            this.blockBusiness = blockBusiness;
            this.fileBusiness = fileBusiness;
            this.correctionBusiness = correctionBusiness;
            this.requestRevokeBusiness = requestRevokeBusiness;
            this.exportAnalysisBusiness = exportAnalysisBusiness;
            this.escolaBusiness = escolaBusiness;
            this.testCurriculumGradeBusiness = testCurriculumGradeBusiness;
            this.testPermissionBusiness = testPermissionBusiness;
            this.testContextBusiness = testContextBusiness;
            this.blockChainBusiness = blockChainBusiness;
            this.blockChainBlockBusiness = blockChainBlockBusiness;
        }

        public ActionResult Index() => View();

        [ActionAuthorizeAttribute(Permission.CreateOrUpdate)]
        public ActionResult IndexForm(long Id = -1)
        {
            return View(new Test { Id = Id });
        }

        public ActionResult IndexImport()
        {
            return View();
        }

        public ActionResult IndexReport()
        {
            return View();
        }

        public ActionResult IndexRevoke()
        {
            return View();
        }

        public ActionResult IndexRequestRevoke()
        {
            return View();
        }

        public ActionResult IndexAdministrate()
        {
            return View();
        }

        public ActionResult IndexStudentResponses()
        {
            return View();
        }

        public ActionResult IndexPermission()
        {
            return View();
        }

        public ActionResult IndexFilterGroupTest(long test_id)
        {
            try
            {
                var entity = testBusiness.GetTestById(test_id);
                ViewBag.GroupFilter = new
                {
                    TestGroupId = entity.TestSubGroup != null ? entity.TestSubGroup.TestGroup.Id : (long?)null,
                    TestSubGroupId = entity.TestSubGroup != null ? entity.TestSubGroup.Id : (long?)null,
                    getGroup = false,
                };
                return View();
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return View();
            }
        }

        #region Read

        [HttpGet]
        public JsonResult GetAuthorize(long test_id, int esc_id = 0)
        {
            try
            {
                var test = testBusiness.GetObjectToAdherence(test_id);
                if (test != null)
                {
                    ESC_Escola escola = null;
                    if (esc_id > 0)
                    {
                        escola = escolaBusiness.Get(esc_id);
                    }

                    var dados = new
                    {
                        testOwner = test.UsuId.Equals(SessionFacade.UsuarioLogado.Usuario.usu_id),
                        testName = test.TestDescription,
                        frequencyApplication = test.FrequencyApplicationDescription,
                        testDiscipline = test.DisciplineDescription,
                        testId = test.Id,
                        global = test.Global,
                        answerSheetBlocked = test.AnswerSheetBlocked,
                        esc_id = esc_id,
                        dre_id = escola != null ? escola.uad_idSuperiorGestao : null
                    };

                    return Json(new { success = true, dados = dados }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não foi possível carregar os dados." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar dados." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetTestInfo(long test_id)
        {
            try
            {
                var test = testBusiness.GetObjectToAdherence(test_id);
                if (test != null)
                {
                    var dados = new
                    {
                        testDescription = test.TestDescription,
                        testDiscipline = test.DisciplineDescription
                    };

                    return Json(new { success = true, dados = dados }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Não foi possível carregar os dados." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao carregar dados." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetTestById(long Id)
        {
            try
            {
                var entity = testBusiness.GetTestById(Id);

                if (entity != null)
                {
                    var ret = new
                    {
                        Id = entity.Id,
                        Description = entity.Description,
                        TestType = new
                        {
                            Id = entity.TestType.Id,
                            Description = entity.TestType.Description,
                            Global = entity.TestType.Global
                        },
                        Discipline = entity.Discipline != null ? new { Id = entity.Discipline.Id, Description = entity.Discipline.Description } : null,
                        Bib = entity.Bib,
                        NumberItemsBlock = entity.NumberItemsBlock,
                        NumberBlock = entity.NumberBlock,
                        NumberItem = entity.NumberItem,
                        DownloadStartDate = entity.DownloadStartDate.HasValue ? entity.DownloadStartDate.GetValueOrDefault().ToString("yyyy/MM/dd") : "",
                        ApplicationStartDate = entity.ApplicationStartDate.ToString("yyyy/MM/dd"),
                        ApplicationEndDate = entity.ApplicationEndDate.ToString("yyyy/MM/dd"),
                        ApplicationActiveOrDone = entity.ApplicationActiveOrDone,
                        CorrectionStartDate = entity.CorrectionStartDate.ToString("yyyy/MM/dd"),
                        CorrectionEndDate = entity.CorrectionEndDate.ToString("yyyy/MM/dd"),
                        Password = entity.Password,
                        BlockItem = entity.Bib ? blockBusiness.CountItemTestBIB(Id) : blockBusiness.CountItemTest(Id),
                        FrequencyApplication = entity.FrequencyApplication,
                        FormatType = entity.FormatType != null ? new { Id = entity.FormatType.Id, Description = entity.FormatType.Description } : null,
                        TestCurriculumGrades = entity.TestCurriculumGrades.Where(i => i.State == (Byte)EnumState.ativo).Select(icg => new
                        {
                            Id = icg.TypeCurriculumGradeId,
                            Description = tipoCurriculoPeriodoBusiness.GetDescription(Convert.ToInt32(icg.TypeCurriculumGradeId), 0, 0, 0)
                        }).ToList(),
                        TestPerformanceLevels = entity.TestPerformanceLevels.Where(i => i.State == (Byte)EnumState.ativo).Select(icg => new
                        {
                            Id = icg.Id,
                            Value1 = icg.Value1,
                            Value2 = icg.Value2,
                            Description = icg.PerformanceLevel.Description,
                            PerformanceLevelId = icg.PerformanceLevel.Id
                        }).ToList(),
                        TestItemLevels = entity.TestItemLevels.Where(i => i.State == (Byte)EnumState.ativo).Select(icg => new
                        {
                            Id = icg.Id,
                            Value = icg.Value,
                            PercentValue = icg.PercentValue,
                            Description = icg.ItemLevel.Description,
                            IdItem = icg.ItemLevel.Id
                        }).ToList(),
                        TestContexts = entity.TestContexts.Where(i => i.State == (Byte)EnumState.ativo).Select(tc => new
                        {
                            id = tc.Id,
                            imagePosition = tc.ImagePosition,
                            imagePositionDescription = tc.ImagePosition.GetDescription(),
                            imagePath = tc.ImagePath,
                            text = tc.Text,
                            title = tc.Title
                        }).ToList(),
                        BlockChains = entity.BlockChains.Where(c => c.State == (byte)EnumState.ativo).Select(c => new
                        {
                            c.Id,
                            c.Description
                        }).ToList(),
                        Blocks = entity.Blocks.Where(c => c.State == (byte)EnumState.ativo).Select(c => new
                        {
                            c.Id,
                            c.Description
                        }).ToList(),
                        TestSituation = entity.TestSituation,
                        PublicFeedback = entity.PublicFeedback,
                        Multidiscipline = entity.Multidiscipline,
                        KnowledgeAreaBlock = entity.KnowledgeAreaBlock,
                        ElectronicTest = entity.ElectronicTest,
                        ShowOnSerapEstudantes = entity.ShowOnSerapEstudantes,
                        ShowTestContext = entity.ShowTestContext,
                        NumberSynchronizedResponseItems = entity.NumberSynchronizedResponseItems,
                        entity.ShowVideoFiles,
                        entity.ShowAudioFiles,
                        entity.ShowJustificate,
                        TestSubGroup = entity.TestSubGroup != null ? new { Id = entity.TestSubGroup.Id, Description = entity.TestSubGroup.Description } : null,
                        TempoDeProva = new { entity.TestTime.Id, entity.TestTime.Description },
                        ShowTestTAI = entity.TestTai,
                        ProvaComProficiencia = entity.ProvaComProficiencia,
                        ApresentarResultados = entity.ApresentarResultados,
                        ApresentarResultadosPorItem = entity.ApresentarResultadosPorItem,
                        NumberItemsAplicationTai = entity.NumberItemsAplicationTai != null ? new { entity.NumberItemsAplicationTai.Id, entity.NumberItemsAplicationTai.Name, entity.NumberItemsAplicationTai.Value, entity.NumberItemsAplicationTai.AdvanceWithoutAnswering, entity.NumberItemsAplicationTai.BackToPreviousItem } : null,
                        AdvanceWithoutAnswering = entity.NumberItemsAplicationTai != null ? entity.NumberItemsAplicationTai.AdvanceWithoutAnswering : false,
                        BackToPreviousItem = entity.NumberItemsAplicationTai != null ? entity.NumberItemsAplicationTai.BackToPreviousItem : false,
                        entity.BlockChain,
                        BlockChainNumber = entity.BlockChainNumber.GetValueOrDefault(),
                        BlockChainItems = entity.BlockChainItems.GetValueOrDefault(),
                        BlockChainForBlock = entity.BlockChainForBlock.GetValueOrDefault()
                    };

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Prova não encontrada." }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar prova pesquisada." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CheckExistsAdherenceByTestId(long Id)
        {
            try
            {
                var existeAdesao = testBusiness.ExistsAdherenceByTestId(Id);
                return Json(new { success = true, existeAdesao }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao verificar a adesão da prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetStatusCorrectionList()
        {
            try
            {
                var ret = Enum.GetValues(typeof(EnumStatusCorrection)).Cast<EnumStatusCorrection>().Select(v => new
                {
                    Id = (int)v,
                    Description = EnumHelper.GetDescriptionFromEnumValue(v)
                }).ToList();

                if (ret != null)
                {
                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Opções de situação da correção não encontradas." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar opções de situação da correção." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetInfoTestReport(long Test_id)
        {
            try
            {
                var lista = testBusiness.GetInfoReportCorrection(Test_id);

                if (lista.Validate.IsValid)
                {
                    return Json(new { success = true, lista = lista }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = lista.Validate.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao obter informações do cabeçalho do relatório" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetInfoTestCurriculumGrade(long Test_id)
        {
            try
            {
                var curriculoPeriodo = testCurriculumGradeBusiness.GetTestCurriculumGradeCrpDescricao(Test_id);

                return Json(new { success = true, curriculoPeriodo = curriculoPeriodo }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao obter informações do cabeçalho do relatório" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCurriculumGradeByTestId(long Test_Id)
        {
            try
            {
                var curriculumGrade = testCurriculumGradeBusiness.GetCurricumGradeByTest_Id(Test_Id);

                return Json(new { success = true, lista = curriculumGrade }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao obter ano(s) de aplicação da prova" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetInfoUadReport(long Test_id, Guid uad_id)
        {
            try
            {
                var lista = testBusiness.GetInfoReportCorrection(Test_id, SessionFacade.UsuarioLogado.Usuario.ent_id, uad_id);

                if (lista.Validate.IsValid)
                {
                    return Json(new { success = true, lista = lista }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = lista.Validate.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao obter informações do cabeçalho do relatório" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetInfoEscReport(long Test_id, Guid uad_id, long esc_id)
        {
            try
            {
                var lista = testBusiness.GetInfoReportCorrection(Test_id, SessionFacade.UsuarioLogado.Usuario.ent_id, uad_id, esc_id);

                if (lista.Validate.IsValid)
                {
                    return Json(new { success = true, lista = lista }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = lista.Validate.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao obter informações do cabeçalho do relatório" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetInfoTurReport(long Test_id, Guid uad_id, long esc_id, long tur_id)
        {
            try
            {
                var lista = testBusiness.GetInfoReportCorrection(Test_id, SessionFacade.UsuarioLogado.Usuario.ent_id, uad_id, esc_id, tur_id);

                if (lista.Validate.IsValid)
                {
                    return Json(new { success = true, lista = lista }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = lista.Validate.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao obter informações do cabeçalho do relatório" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult SearchTests(TestFilter filter)
        {
            try
            {
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
                Pager pager = this.GetPager();
                var lista = testBusiness._SearchTests(filter, ref pager);

                if (lista != null)
                {
                    if (filter.getGroup)
                    {
                        var ret = lista.GroupBy(p => new
                        {
                            p.TestGroupId,
                            p.TestSubGroupId,
                            OrderSubGroup = p.TestGroupId == 0 ? -1 : p.OrderSubGroup,
                            p.TestGroupName,
                            p.TestSubGroupName,
                            p.TestGroupCreateDate
                        }).Select(grp => new
                        {
                            TestGroupId = grp.Key.TestGroupId,
                            TestSubGroupId = grp.Key.TestSubGroupId,
                            OrderSubGroup = grp.Key.OrderSubGroup,
                            TestGroupName = grp.Key.TestGroupName,
                            TestSubGroupName = grp.Key.TestSubGroupName,
                            TestGroupCreateDate = grp.Key.TestGroupCreateDate,
                            TestCount = grp.Count()
                        });

                        switch (filter.ordenacao)
                        {
                            case 1:
                                ret = ret.OrderByDescending(p => p.TestGroupCreateDate);
                                break;
                            case 2:
                                ret = ret.OrderBy(p => p.TestGroupName).ThenBy(q => q.TestSubGroupName);
                                break;
                            case 3:
                                ret = ret.OrderByDescending(p => p.OrderSubGroup);
                                break;
                        }

                        return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var ret = lista.Select(entity => new
                        {
                            Test = entity,
                            ItemTypeUndefined = entity.ItemType_Id == null,
                            TestOwner = entity.UsuId.Equals(SessionFacade.UsuarioLogado.Usuario.usu_id) || ((EnumSYS_Visao.Administracao == filter.vis_id && entity.Global))
                        }).ToList();

                        return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Provas não encontradas." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar itens pesquisados." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetTestByDate(DateTime DateStart, DateTime DateEnd)
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
                filter.ApplicationStartDate = DateStart;
                filter.CorrectionEndDate = DateEnd;
                filter.ent_id = SessionFacade.UsuarioLogado.Usuario.ent_id;
                filter.gru_id = SessionFacade.UsuarioLogado.Grupo.gru_id;
                filter.pes_id = SessionFacade.UsuarioLogado.Usuario.pes_id;
                filter.usuId = SessionFacade.UsuarioLogado.Usuario.usu_id;
                filter.vis_id = visao;

                var lista = testBusiness.GetTestByDate(filter);

                if (lista != null)
                {
                    return Json(new { success = true, lista = lista }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Provas não encontradas." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar itens pesquisados." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetSectionAdministrate(long test_id, int esc_id, int ttn_id, string dre_id, int crp_ordem, string statusCorrection)
        {
            try
            {
                Pager pager = this.GetPager();

                StudentResponseFilter filter = new StudentResponseFilter
                {
                    Test_Id = test_id,
                    School_Id = esc_id,
                    ttn_id = ttn_id,
                    uad_id = string.IsNullOrEmpty(dre_id) ? Guid.Empty : new Guid(dre_id),
                    crp_ordem = crp_ordem,
                    pes_id = SessionFacade.UsuarioLogado.Usuario.pes_id,
                    usu_id = SessionFacade.UsuarioLogado.Usuario.usu_id,
                    vis_id = SessionFacade.UsuarioLogado.Grupo.vis_id,
                    sis_id = SessionFacade.UsuarioLogado.Grupo.sis_id,
                    StatusCorrection = statusCorrection
                };

                var lista = correctionBusiness.LoadOnlySelectedSectionPaginate(ref pager, filter);

                if (lista != null && lista.Count() > 0)
                {
                    IEnumerable<AnswerSheetBatch> batchInfo = testBusiness.GetTestAutomaticCorrectionSituation(test_id, esc_id);

                    ESC_Escola escola = null;
                    if (esc_id > 0)
                    {
                        escola = escolaBusiness.Get(esc_id);
                    }

                    var permission = testPermissionBusiness.GetByTest(test_id, SessionFacade.UsuarioLogado.Grupo.gru_id).FirstOrDefault();

                    var ret = lista.Select(i => new
                    {
                        tur_id = i.tur_id,
                        tur_codigo = i.tur_codigo,
                        ttn_nome = i.ttn_nome,
                        esc_id = i.esc_id,
                        esc_nome = i.esc_nome,
                        dre_id = i.dre_id,
                        dre_nome = i.uad_nome,
                        StatusCorrection = i.StatusCorrection,
                        FileAnswerSheet = new
                        {
                            Id = i.FileId,
                            Name = !string.IsNullOrEmpty(i.FileOriginalName) ? i.FileOriginalName : i.FileName,
                            Path = i.FilePath
                        }
                    });

                    var result = new
                    {
                        success = true,
                        lista = ret,
                        AllowAnswer = permission != null ? permission.AllowAnswer : true,
                        ShowResult = permission != null ? permission.ShowResult : true,
                        batchWarning = new
                        {
                            status = batchInfo != null && batchInfo.Any(i => i.Processing.Equals(EnumBatchProcessing.Failure)),
                            message = (batchInfo == null ? string.Empty : batchInfo != null && !batchInfo.Any(i => i.Processing.Equals(EnumBatchProcessing.Failure)) ? string.Empty : esc_id > 0 ? string.Format("Atenção: Há falha no lote da escola {0}.", escola.esc_nome) : "Atenção: Há falha no lote da prova.")
                        }
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, type = ValidateType.alert.ToString(), message = "Nenhuma turma foi encontrada" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar prova pesquisada." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCurriculumGradeSimple(int esc_id)
        {
            try
            {
                var curriculoPeriodo = this.tipoCurriculoPeriodoBusiness.GetSimple(esc_id);

                var retorno = curriculoPeriodo.Select(e => new DropDownReturnModel
                {
                    Id = string.Format("{0}_{1}", e.tne_id, e.tcp_ordem),
                    Description = string.Format("{0} - {1}", e.tcp_descricao, e.ACA_TipoNivelEnsino.tne_nome),
                    CurriculumTypeId = e.tcp_id
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
        [Paginate]
        public JsonResult GetItems(string ItemCode, int? ItemOrder, long TestId)
        {
            try
            {
                Pager pager1 = this.GetPager();

                var blockItems = testBusiness.GetItemsByTest(TestId, SessionFacade.UsuarioLogado.Usuario.usu_id, ref pager1);

                if (!string.IsNullOrEmpty(ItemCode))
                {
                    blockItems = blockItems.Where(i => i.Item.ItemCode.CompareWithTrim(ItemCode));
                    pager1.SetTotalPages(1);
                }
                if (ItemOrder != null)
                {
                    blockItems = blockItems.Where(i => i.Order == ItemOrder);
                    pager1.SetTotalPages(1);
                }

                var retorno = blockItems.Select(bi => new
                {
                    Item_Id = bi.Item.Id,
                    ItemCode = bi.Item.ItemCode,
                    ItemVersion = bi.Item.ItemVersion,
                    ItemOrder = bi.Order,
                    BaseTextDescription = bi.Item.BaseText != null ? bi.Item.BaseText.Description : "",
                    Statement = bi.Item.Statement != null ? bi.Item.Statement : "",
                    Revoked = bi.RequestRevokes != null ? bi.RequestRevokes.First().Situation : EnumSituation.NotRevoked,
                    ItemSituation = bi.RequestRevokes != null ? bi.RequestRevokes.First().Situation : EnumSituation.NotRevoked,
                    RequestRevoke_Id = bi.RequestRevokes != null ? bi.RequestRevokes.First().Id : 0,
                    BlockItem_Id = bi.Id,
                    Justification = bi.RequestRevokes != null ? bi.RequestRevokes.First().Justification : ""
                }).ToList();

                return Json(new { sucess = true, lista = retorno }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar items da prova" }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetPendingRevokeItems(string ItemCode, DateTime? StartDate, DateTime? EndDate, EnumSituation? Situation)
        {
            Pager pager1 = this.GetPager();

            var blockItems = testBusiness.GetPendingRevokeItems(ref pager1, ItemCode, StartDate, EndDate, Situation);

            var retorno = blockItems.Select(bi => new
            {
                BlockItem_Id = bi.Id,
                Item_Id = bi.Item.Id,
                ItemCode = bi.Item.ItemCode,
                ItemVersion = bi.Item.ItemVersion,
                ItemOrder = bi.Order,
                ItemRevoked = bi.Item.Revoked,
                ItemLastVersion = bi.Item.LastVersion,
                BaseTextDescription = bi.Item.BaseText != null ? bi.Item.BaseText.Description : "",
                Statement = bi.Item.Statement != null ? bi.Item.Statement : "",
                Date = bi.RequestRevokes.First().CreateDate,
                Test_Id = bi.RequestRevokes.First().Test.Id,
                Test_Description = bi.RequestRevokes.First().Test.Description,
                RequestRevokes = bi.RequestRevokes.Count(),
                Situation = bi.RequestRevokes.First().Situation,
                LabelSituation = EnumExtensions.GetDescription(bi.RequestRevokes.First().Situation)
            }).OrderBy(x => x.Date).ToList();

            return Json(new { sucess = true, lista = retorno }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetRequestRevokes(int blockItem_Id)
        {
            var requestRevokes = requestRevokeBusiness.GetRequestRevoke(blockItem_Id);

            var retorno = requestRevokes.Select(rr => new
            {
                Date = rr.UpdateDate.ToShortDateString(),
                Requester_Name = rr.pes_nome != null ? rr.pes_nome : "Este usuário não possui uma pessoa associada",
                Requester_Email = rr.usu_email,
                Requester_Id = rr.UsuId,
                Justification = rr.Justification

            }).ToList();

            return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Paginate]
        public JsonResult GetTestsPermissions(long test_id, Guid? gru_id)
        {
            var retorno = new List<TestPermission>();
            if (SessionFacade.UsuarioLogado.Grupo.vis_id == (int)EnumSYS_Visao.Administracao)
            {
                retorno = testPermissionBusiness.GetByTest(test_id, gru_id).ToList();
            }

            return Json(new { success = true, lista = retorno }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetTestsBySubGroup(long Id)
        {
            try
            {
                IEnumerable<TestResult> tests = testBusiness.GetTestsBySubGroup(Id);
                return Json(new { success = true, lista = tests }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar o grupo de prova pesquisado." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public void DownloadFile(long Id)
        {
            bool redirect = false;
            try
            {
                EntityFile file = fileBusiness.Get(Id);
                if (file != null)
                {
                    string filePath = new Uri(file.Path).AbsolutePath.Replace("Files/", string.Empty);
                    string physicalPath = string.Concat(ApplicationFacade.PhysicalDirectory, filePath.Replace("/", "\\"));
                    string decodedUrl = HttpUtility.UrlDecode(physicalPath);

                    if (System.IO.File.Exists(decodedUrl))
                    {
                        FileStream fs = System.IO.File.Open(decodedUrl, FileMode.Open);
                        byte[] btFile = new byte[fs.Length];
                        fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
                        fs.Close();

                        Response.Clear();
                        Response.AddHeader("Content-disposition", "attachment; filename=" + file.OriginalName);
                        Response.ContentType = file.ContentType;
                        Response.BinaryWrite(btFile);
                        Response.End();
                        redirect = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
            }

            if (!redirect && Request.UrlReferrer != null && !string.IsNullOrEmpty(Request.UrlReferrer.PathAndQuery))
                Response.Redirect(Request.UrlReferrer.PathAndQuery, false);
        }

        [HttpGet]
        public async Task<JsonResult> ObterDadosAmostraProvaTai(long provaId, long matrizId,
            int tipoCurriculoGradeId)
        {
            var dadosProvaTai = await testBusiness.ObterDadosProvaTai(provaId);

            if (dadosProvaTai == null)
            {
                return Json(
                    new
                    {
                        success = false,
                        type = ValidateType.error.ToString(),
                        message = $"Os dados da amostra não foram cadastrados para a prova {provaId}."
                    }, JsonRequestBehavior.AllowGet);
            }

            var numeroItensAmostraTai = dadosProvaTai.NumeroItensAmostra;

            var numeroItensAmostraMatrizAnoTai = (await testBusiness
                    .ObterItensAmostraTai(new[] { matrizId }, new[] { tipoCurriculoGradeId }))
                .Take(numeroItensAmostraTai).Count();

            var porcentagemMaxima = numeroItensAmostraMatrizAnoTai * 100 / numeroItensAmostraTai;

            var dados = new
            {
                porcentagemMaximaMatrizAno = porcentagemMaxima,
                numeroItensAmostraMatrizAno = numeroItensAmostraMatrizAnoTai,
                labelInfoPorcentagemMaximaMatrizAno = $"Valor % máximo: {porcentagemMaxima}."
            };

            return Json(new { success = true, dados }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> ObterDadosAmostraProvaTaiPorProvaId(long provaId)
        {
            var dadosProvaTai = await testBusiness.ObterDadosProvaTai(provaId);

            if (dadosProvaTai == null)
            {
                return Json(
                    new
                    {
                        success = false,
                        type = ValidateType.error.ToString(),
                        message = $"Os dados da amostra não foram cadastrados para a prova {provaId}."
                    }, JsonRequestBehavior.AllowGet);
            }

            var list = await testBusiness.GetListTestTaiCurriculumGradeByTestId(provaId);

            if (list == null || !list.Any())
            {
                return Json(
                    new
                    {
                        success = false,
                        type = ValidateType.error.ToString(),
                        message = $"Os dados não foram cadastrados para a prova {provaId}."
                    }, JsonRequestBehavior.AllowGet);
            }

            var numeroItensAmostraTai = dadosProvaTai.NumeroItensAmostra;
            var matrizesIds = list.Select(c => c.MatrixId).Distinct().ToArray();
            var tiposCurriculosGradesIds = list.Select(c => int.Parse(c.TypeCurriculumGradeId.ToString())).Distinct().ToArray();

            var itensAmostraMatrizAnoTai = await testBusiness.ObterItensAmostraTai(matrizesIds, tiposCurriculosGradesIds);

            var itensAgrupadosAmostraMatrizAnoTai = itensAmostraMatrizAnoTai
                .GroupBy(c => new {c.MatrizId, c.TipoCurriculoGradeId });

            var dados = new List<object>();

            foreach (var item in itensAgrupadosAmostraMatrizAnoTai)
            {
                var numeroItensAmostraMatrizAnoTai = item.Take(numeroItensAmostraTai).Count();
                var porcentagemMaxima = numeroItensAmostraMatrizAnoTai * 100 / numeroItensAmostraTai;

                dados.Add(new
                {
                    item.Key.MatrizId,
                    item.Key.TipoCurriculoGradeId,
                    porcentagemMaximaMatrizAno = porcentagemMaxima,
                    numeroItensAmostraMatrizAno = item.Count(),
                    labelInfoPorcentagemMaximaMatrizAno = $"Valor % máximo: {porcentagemMaxima}."
                });
            }

            return Json(new { success = true, dados }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Write

        [HttpPost]
        public JsonResult Save(Test entity)
        {
            try
            {
                foreach (var testContext in entity.TestContexts)
                {
                    var position = ObterPosicionamento(testContext.ImagePositionDescription);

                    testContext.ImagePosition = position;
                }

                if (entity.Id > 0)
                {
                    entity = testBusiness.Update(entity.Id, entity, SessionFacade.UsuarioLogado.Usuario.usu_id,
                            (EnumSYS_Visao.Administracao == (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                                SessionFacade.UsuarioLogado.Grupo.vis_id.ToString())));

                    if (entity.TestContexts.Any())
                    {
                        testContextBusiness.DeleteByTestId(entity.Id);

                        foreach (var testContext in entity.TestContexts)
                        {
                            var position = ObterPosicionamento(testContext.ImagePositionDescription);

                            testContext.ImagePosition = position;
                            testContext.Test_Id = entity.Id;
                            testContextBusiness.Save(testContext);
                        }
                    }

                    if (entity.RemoveBlockChain && entity.BlockChains.Any())
                        blockChainBusiness.DeleteByTestId(entity.Id);

                    if (entity.RemoveBlockChainBlock && entity.Blocks.Any())
                    {
                        foreach (var block in entity.Blocks)
                            blockChainBlockBusiness.DeleteByBlockId(block.Id);
                    }
                }
                else
                {
                    entity = testBusiness.Save(entity, SessionFacade.UsuarioLogado.Usuario.usu_id, (EnumSYS_Visao.Administracao == (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                            SessionFacade.UsuarioLogado.Grupo.vis_id.ToString())));
                }

                if (entity.Validate.IsValid)
                {
                    entity.TestSituation = testBusiness.TestSituation(entity);
                }
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} a prova.", entity.Id > 0 ? "alterar" : "salvar");

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, TestID = entity.Id, TestSituation = entity.TestSituation, entity.ApplicationActiveOrDone }, JsonRequestBehavior.AllowGet);
        }

        private EnumPosition ObterPosicionamento(string imagePositionDescription)
        {
            if (EnumPosition.Center.GetDescription() == imagePositionDescription)
                return EnumPosition.Center;

            if (EnumPosition.Right.GetDescription() == imagePositionDescription)
                return EnumPosition.Right;

            return EnumPosition.Left;
        }

        [HttpPost]
        public JsonResult FinallyTest(long Id)
        {
            Test entity = new Test();

            try
            {
                entity = testBusiness.FinallyTest(Id, SessionFacade.UsuarioLogado.Usuario.usu_id,
                    (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()) == EnumSYS_Visao.Administracao);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar alterar a situação da prova.";

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, TestSituation = entity.TestSituation }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(long Id)
        {
            Test entity = new Test();

            try
            {
                entity = testBusiness.Delete(Id);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao tentar excluir a prova.";
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChangeTestVisible(long Id, bool Visible)
        {
            Test entity = new Test();

            try
            {
                testBusiness.UpdateTestVisible(Id, Visible);
                entity.Validate.IsValid = true;
                entity.Validate.Message = string.Format("Prova {0} com sucesso.", Visible ? "reexibida" : "ocultada");
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao tentar {0} a prova.", Visible ? "reexibir" : "ocultar");
                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SavePermission(long test_id, List<TestPermission> permissions)
        {
            TestPermission entity = new TestPermission();
            try
            {
                entity.Test_Id = test_id;
                testPermissionBusiness.Save(entity, permissions);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao salvar as permissões";

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, TestID = entity.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeOrderTestUp(long Id, long order)
        {
            Test entity = new Test();
            try
            {
                testBusiness.ChangeOrderTestUp(Id, order);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao alterar a ordem da prova.";

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, TestID = entity.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeOrderTestDown(long Id, long order)
        {
            Test entity = new Test();
            try
            {
                testBusiness.ChangeOrderTestDown(Id, order);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao alterar a ordem da prova.";

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, TestID = entity.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeOrder(long idOrigem, long idDestino)
        {
            Test entity = new Test();
            try
            {
                testBusiness.ChangeOrder(idOrigem, idDestino);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao alterar a ordem da prova.";

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, TestID = entity.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TestTaiCurriculumGradeSave(List<TestTaiCurriculumGrade> listEntity)
        {
            try
            {

                testBusiness.TestTaiCurriculumGradeSave(listEntity);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                throw ex;
            }

            //GetListTestTaiCurriculumGrade
        }

        [HttpGet]
        public async Task<JsonResult> GetListTestTaiCurriculumGrade(long testId)
        {
            try
            {
                var list = await testBusiness.GetListTestTaiCurriculumGradeByTestId(testId);

                if (list != null && list.Any())
                    return Json(new { success = true, lista = list }, JsonRequestBehavior.AllowGet);

                return Json(new { success = true, lista = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, lista = "" }, JsonRequestBehavior.AllowGet);
                throw ex;
            }
        }


        #endregion

        #region Test Files

        #region Read

        [HttpGet]
        [Paginate]
        public JsonResult SearchTestFiles(FileFilter filter)
        {
            try
            {
                #region Filters

                if (filter == null)
                    filter = new FileFilter();

                Pager pager = this.GetPager();
                filter.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;
                filter.CoreVisionId = SessionFacade.UsuarioLogado.Grupo.vis_id;
                filter.CoreSystemId = Constants.IdSistema;

                #endregion

                IEnumerable<EntityFile> files = null;
                Test test = null;
                if (filter.OwnerId > 0)
                {
                    test = testBusiness.GetTestById(filter.OwnerId);
                    files = fileBusiness.SearchUploadedFiles(ref pager, filter);
                }

                if (files != null && test != null)
                {
                    var ret = files.Select(entity => new
                    {
                        Id = entity.Id,
                        Path = entity.Path,
                        OriginalName = entity.OriginalName,
                        CreateDate = entity.CreateDate.ToShortDateString(),
                        TestLinks = entity.TestFiles,
                        OwnerId = test.Id,
                        OwnerName = test.Description,
                        Checked = testFilesBusiness.GetChecked(entity.Id, test.Id),
                        AllLinks = fileBusiness.GetTestNames(entity.Id),
                        AllFiles = fileBusiness.GetAllFiles(filter)
                    }).ToList();

                    return Json(new { success = true, lista = ret, pageSize = pager.PageSize }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, lista = "" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar arquivos pesquisados." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CheckFilesExists(long Id)
        {
            bool success = false;

            try
            {
                IEnumerable<EntityFile> files = testFilesBusiness.GetFiles(Id, EnumFileType.AnswerSheetStudentNumber);
                success = fileBusiness.CheckFilesExists(files.Select(f => f.Id), ApplicationFacade.PhysicalDirectory);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
            }

            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void DownloadZipFiles(long Id)
        {
            string completePath = string.Empty;
            bool redirect = false;

            try
            {
                IEnumerable<EntityFile> files = testFilesBusiness.GetFiles(Id, EnumFileType.AnswerSheetStudentNumber);
                if (files != null)
                {
                    IEnumerable<ZipFileInfo> fileNames = files.Select(f => new ZipFileInfo
                    {
                        Path = string.Concat(ApplicationFacade.PhysicalDirectory, new Uri(f.Path).AbsolutePath.Replace("Files/", string.Empty).Replace("/", "\\")),
                        Name = !string.IsNullOrEmpty(f.OriginalName) ? f.OriginalName : f.Name
                    });

                    var filenNotExists = fileNames.Where(i => !System.IO.File.Exists(HttpUtility.UrlDecode(i.Path)));
                    if (filenNotExists != null && filenNotExists.Any())
                    {
                        redirect = true;
                    }
                    else
                    {
                        Test test = testBusiness.GetTestById(Id);

                        string displayName = String.Format("ArquivosProva{0}_{1}_{2}_{3}.zip", test != null ? "_" + test.Description : string.Empty, Id, DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HHmmss"));
                        displayName = Regex.Replace(displayName, @"[^\w\.@-]", "_");

                        string zipName = Guid.NewGuid() + ".zip";

                        EntityFile file = fileBusiness.SaveZip(zipName, "Zip", fileNames, ApplicationFacade.PhysicalDirectory);
                        if (file.Validate.IsValid)
                        {
                            completePath = Path.Combine(file.Path, zipName);

                            System.IO.FileStream fs = System.IO.File.Open(completePath, System.IO.FileMode.Open);
                            byte[] btFile = new byte[fs.Length];
                            fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
                            fs.Close();

                            Response.Clear();
                            Response.AddHeader("Content-disposition", "attachment; filename=" + displayName);
                            Response.ContentType = "application/octet-stream";
                            Response.BinaryWrite(btFile);
                            Response.End();
                        }
                        else
                            redirect = true;
                    }
                }
            }
            catch (Exception ex)
            {
                redirect = true;
                LogFacade.SaveError(ex);
            }

            if (!string.IsNullOrEmpty(completePath) && System.IO.File.Exists(completePath))
                System.IO.File.Delete(completePath);

            if (redirect)
                Response.Redirect("/Test", false);
        }

        [HttpGet]
        public JsonResult GetTestFiles(long Id)
        {
            try
            {
                IEnumerable<EntityFile> files = testFilesBusiness.GetFiles(Id, EnumFileType.AnswerSheetStudentNumber);

                if (files != null)
                {
                    var ret = files.Select(entity => new
                    {
                        Id = entity.Id,
                        Name = !string.IsNullOrEmpty(entity.OriginalName) ? entity.OriginalName : entity.Name,
                        Path = entity.Path,
                        AllowLink = !entity.ContentType.Equals(MimeType.CSV.GetDescription())
                    }).OrderBy(entity => entity.Name);

                    return Json(new { success = true, lista = ret }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, lista = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar arquivos da prova." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Write

        [HttpPost]
        public JsonResult SaveTestFiles(long Id, List<TestFiles> files)
        {
            TestFiles entity = new TestFiles();
            IEnumerable<TestFiles> entities = null;
            try
            {
                if (files == null)
                    files = new List<TestFiles>();

                entity.Test_Id = Id;
                entity.UserId = SessionFacade.UsuarioLogado.Usuario.usu_id;

                entities = testFilesBusiness.Update(entity, files, SessionFacade.UsuarioLogado.Usuario.usu_id,
                    (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()));

                if (entities != null && entities.Count() > 0)
                {
                    var saved = entities.ElementAt(0);
                    if (saved.Validate.IsValid)
                    {
                        entity.Validate.IsValid = true;
                        entity.Validate.Message = "Vinculo(s) alterado com sucesso.";
                    }
                    else
                    {
                        entity.Validate.IsValid = false;
                        entity.Validate.Message = saved.Validate.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = "Erro ao vincular arquivo(s) à prova.";

                LogFacade.SaveError(ex);
            }

            return Json(new { success = entity.Validate.IsValid, type = entity.Validate.Type, message = entity.Validate.Message, TestLinks = entities }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Export Analysis

        [Paginate]
        [HttpGet]
        public JsonResult ExportAnalysisSearch(ExportAnalysisFilter filter)
        {
            try
            {
                Pager pager = this.GetPager();
                var lista = exportAnalysisBusiness.Search(ref pager, filter);
                return Json(new { success = true, lista = lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar prova pesquisada." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SolicitExport(long TestId)
        {
            var entity = new ExportAnalysis() { StateExecution = EnumServiceState.Pending, Test_Id = TestId };
            try
            {
                entity = exportAnalysisBusiness.SolicitExport(entity.Test_Id);
                return Json(new
                {
                    success = true,
                    type = entity.Validate.Type,
                    message = entity.Validate.Message != null ? entity.Validate.Message : "Solicitação realizada com sucesso."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                entity.Validate.IsValid = false;
                entity.Validate.Type = ValidateType.error.ToString();
                entity.Validate.Message = string.Format("Erro ao {0} a prova.", entity.Id > 0 ? "alterar" : "salvar");

                LogFacade.SaveError(ex);
                return Json(new { success = false, type = ValidateType.error.ToString(), message = "Erro ao tentar encontrar prova pesquisada." }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult ImportarArquivoCsvBlocos(HttpPostedFileBase file, int testId)
        {
            var b = new BinaryReader(file.InputStream);
            var binData = b.ReadBytes(file.ContentLength);
            var result = System.Text.Encoding.UTF8.GetString(binData);

            var splitRowResult = result.Substring(0, StringHelper.PositionOfNewLine(result)).Trim()
                .Replace("\"", string.Empty).Split(';');

            if (string.IsNullOrEmpty(splitRowResult.ToString()))
                return Json(new { success = false, message = "Erro ao importar blocos." }, JsonRequestBehavior.AllowGet);

            b.BaseStream.Position = 0;

            try
            {
                testBusiness.ImportarCvsBlocos(file, testId, SessionFacade.UsuarioLogado.Usuario.usu_id,
                    (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                        SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()), out var retorno);


                return Json(new { success = true, retorno, message = "Importação dos blocos realizadas com sucesso!." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, retorno = "", message = "Erro ao importar resultados." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ImportarArquivoCsvCadernos(HttpPostedFileBase file, int testId)
        {
            var b = new BinaryReader(file.InputStream);
            var binData = b.ReadBytes(file.ContentLength);
            var result = System.Text.Encoding.UTF8.GetString(binData);

            var splitRowResult = result.Substring(0, StringHelper.PositionOfNewLine(result)).Trim()
                .Replace("\"", string.Empty).Split(';');

            if (string.IsNullOrEmpty(splitRowResult.ToString()))
                return Json(new { success = false, message = "Erro ao importar cadernos." }, JsonRequestBehavior.AllowGet);

            b.BaseStream.Position = 0;

            try
            {
                testBusiness.ImportarCvsCadernos(file, testId, SessionFacade.UsuarioLogado.Usuario.usu_id,
                    (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao),
                        SessionFacade.UsuarioLogado.Grupo.vis_id.ToString()), out var retorno);                

                return Json(new { success = true, retorno, message = "Importação dos cadernos realizadas com sucesso!." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogFacade.SaveError(ex);
                return Json(new { success = false, retorno = "", message = "Erro ao importar resultados." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}