using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IFileServer;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;
using GestaoEscolar.Entities.DTO;
using GestaoEscolar.IBusiness;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace GestaoAvaliacao.Business
{
    public class TestBusiness : ITestBusiness
    {
        private readonly ITestRepository testRepository;
        private readonly IFileBusiness fileBusiness;
        private readonly IParameterBusiness parameterBusiness;
        private readonly IBookletBusiness bookletBusiness;
        private readonly IFileRepository fileRepository;
        private readonly ITestPerformanceLevelRepository testPerformanceLevelRepository;
        private readonly IItemLevelRepository itemLevelRepository;
        private readonly IPerformanceLevelRepository performanceLevelRepository;
        private readonly IBlockRepository blockRepository;
        private readonly IStorage storage;
        private readonly ITUR_TurmaBusiness turmaBusiness;
        private readonly ISYS_UnidadeAdministrativaBusiness unidadeAdministrativaBusiness;
        private readonly IESC_EscolaBusiness escolaBusiness;
        private readonly ITestTypeDeficiencyRepository testTypeDeficiencyRepository;
        private readonly INumberItemsAplicationTaiRepository numberItemsAplicationTaiRepository;
        private readonly INumberItemTestTaiRepository numberItemTestTaiRepository;
        private readonly ITestTaiCurriculumGradeRepository testTaiCurriculumGradeRepository;


        public TestBusiness(ITestRepository testRepository, IFileBusiness fileBusiness, IBookletBusiness bookletBusiness, IFileRepository fileRepository,
            ITestPerformanceLevelRepository testPerformanceLevelRepository, IItemLevelRepository itemLevelRepository, IPerformanceLevelRepository performanceLevelRepository,
            IBlockRepository blockRepository, IParameterBusiness parameterBusiness, IStorage storage, ITUR_TurmaBusiness turmaBusiness, ISYS_UnidadeAdministrativaBusiness unidadeAdministrativaBusiness,
            IESC_EscolaBusiness escolaBusiness, ITestTypeDeficiencyRepository testTypeDeficiencyRepository, INumberItemsAplicationTaiRepository numberItemsAplicationTaiRepository,
            INumberItemTestTaiRepository numberItemTestTaiRepository, ITestTaiCurriculumGradeRepository testTaiCurriculumGradeRepository)
        {
            this.testRepository = testRepository;
            this.fileRepository = fileRepository;
            this.fileBusiness = fileBusiness;
            this.parameterBusiness = parameterBusiness;
            this.bookletBusiness = bookletBusiness;
            this.testPerformanceLevelRepository = testPerformanceLevelRepository;
            this.itemLevelRepository = itemLevelRepository;
            this.performanceLevelRepository = performanceLevelRepository;
            this.blockRepository = blockRepository;
            this.storage = storage;
            this.turmaBusiness = turmaBusiness;
            this.unidadeAdministrativaBusiness = unidadeAdministrativaBusiness;
            this.escolaBusiness = escolaBusiness;
            this.testTypeDeficiencyRepository = testTypeDeficiencyRepository;
            this.numberItemsAplicationTaiRepository = numberItemsAplicationTaiRepository;
            this.numberItemTestTaiRepository = numberItemTestTaiRepository;
            this.testTaiCurriculumGradeRepository = testTaiCurriculumGradeRepository;
        }

        #region Custom

        public Validate CanEdit(long TestId, Guid UsuId, EnumSYS_Visao vision)
        {
            Validate valid = new Util.Validate();
            var cadastred = testRepository.GetObjectWithTestType(TestId);

            if (!((cadastred.UsuId == UsuId) || (vision == EnumSYS_Visao.Administracao && cadastred.TestType.Global)))
                valid.Message = "Apenas o proprietário da prova pode alterá-la";

            if (!string.IsNullOrEmpty(valid.Message))
                valid.IsValid = false;
            else
                valid.IsValid = true;

            return valid;
        }
        private Validate Validate(Test entity, ValidateAction action, Validate valid, bool isAdmin, Guid? UsuId = null)
        {
            valid.Message = null;
            int qtdeMaxItems = 100;

            if (action == ValidateAction.Save)
            {
                if (string.IsNullOrEmpty(entity.Description) || entity.ApplicationEndDate == null || entity.ApplicationStartDate == null
                || entity.CorrectionEndDate == null || entity.CorrectionStartDate == null
                || entity.TestType == null || (entity.TestType != null && entity.TestType.Id <= 0)
                || (entity.Discipline == null && !entity.Multidiscipline) || (!entity.Multidiscipline && entity.Discipline != null && entity.Discipline.Id <= 0)
                || entity.TestCurriculumGrades == null || (entity.TestCurriculumGrades != null && entity.TestCurriculumGrades.Count <= 0)
                || entity.TestTime == null || (entity.TestTime != null && entity.TestTime.Id <= 0))
                    valid.Message = "Não foram preenchidos todos os campos obrigatórios.";

                int totalItems = entity.TestItemLevels != null ? entity.TestItemLevels.Sum(i => i.Value) : 0;
                if (entity.NumberItem > qtdeMaxItems || totalItems > qtdeMaxItems)
                    valid.Message = string.Format("A quantidade de itens deve ser menor ou igual a {0}.", qtdeMaxItems);
            }

            if (entity.Bib)
            {
                if (entity.NumberBlock <= 0)
                    valid.Message = "A quantidade de cadernos deve ser maior ou igual a 1.";

                var maxBlock = parameterBusiness.GetByKey(EnumParameterKey.TEST_MAX_BLOCK.GetDescription());

                if (maxBlock != null)
                {
                    if (entity.NumberBlock > int.Parse(maxBlock.Value))
                        valid.Message = string.Format("A quantidade de cadernos deve ser menor ou igual a {0}.", int.Parse(maxBlock.Value));
                }

                if (entity.BlockChain.GetValueOrDefault())
                {
                    if (entity.BlockChainNumber.GetValueOrDefault() <= 0)
                        valid.Message = "A quantidade de blocos deve ser maior ou igual a 1.";

                    if (entity.BlockChainItems.GetValueOrDefault() <= 0)
                        valid.Message = "A quantidade de itens por bloco deve ser maior ou igual a 1.";

                    if (entity.BlockChainForBlock.GetValueOrDefault() <= 0)
                        valid.Message = "A quantidade de blocos por caderno deve ser maior ou igual a 1.";
                }
            }

            if (action == ValidateAction.Update)
            {
                var cadastred = testRepository.GetObjectWithTestType(entity.Id);

                if (entity.TestTime == null || (entity.TestTime != null && entity.TestTime.Id <= 0))
                    valid.Message = "O tempo de prova deve ser informado.";

                int totalItems = entity.TestItemLevels != null ? entity.TestItemLevels.Sum(i => i.Value) : 0;
                if (entity.NumberItem > qtdeMaxItems || totalItems > qtdeMaxItems)
                    valid.Message = string.Format("A quantidade de itens deve ser menor ou igual a {0}.", qtdeMaxItems);
                if (!((cadastred.UsuId == UsuId) || (isAdmin && cadastred.TestType.Global)))
                    valid.Message = "Apenas o proprietário da prova pode alterá-la";
            }

            if (entity.TestContexts.Any())
            {
                foreach (var testContext in entity.TestContexts)
                {
                    var textoSemHtml = UtilRegex.RemoverTagsHtml(testContext.Text);
                    if (textoSemHtml.Length > 500)
                    {
                        valid.Message = $"O Contexto '{testContext.Title}' está com o texto maior que 500 caracteres.";
                    }
                }
            }


            if (!string.IsNullOrEmpty(valid.Message))
            {
                valid.IsValid = false;

                if (valid.Code <= 0)
                    valid.Code = 400;

                valid.Type = ValidateType.alert.ToString();
            }
            else
                valid.IsValid = true;

            return valid;
        }

        private EnumTestSituation ValidateTestSituation(Test entity)
        {
            var cadastred = testRepository.GetObject(entity.Id);

            int totalItems = entity.TestItemLevels != null ? entity.TestItemLevels.Sum(i => i.Value) : 0;
            if (entity.NumberItem > 0)
                totalItems = (int)entity.NumberItem;

            IEnumerable<TestItemLevel> TestItemLevels = testRepository.GetTestItems(cadastred.Id);
            int totalItemsCadastred = TestItemLevels != null ? TestItemLevels.Sum(i => i.Value) : 0;
            if (cadastred.NumberItem > 0)
                totalItemsCadastred = (int)cadastred.NumberItem;

            var disciplineChanged = entity.Multidiscipline != cadastred.Multidiscipline
                || (!entity.Multidiscipline && entity.Discipline.Id != cadastred.Discipline_Id);

            if (entity.TestType.Id != cadastred.TestType_Id || disciplineChanged
                || !entity.Description.Equals(cadastred.Description) || !entity.FrequencyApplication.Equals(cadastred.FrequencyApplication)
                || !totalItems.Equals(totalItemsCadastred))
                entity.TestSituation = EnumTestSituation.Pending;
            else
                entity.TestSituation = cadastred.TestSituation;

            return entity.TestSituation;
        }

        #endregion

        #region Read

        public Test GetObject(long Id)
        {
            return testRepository.GetObject(Id);
        }

        public Test GetTestBy_Id(long Id)
        {
            return testRepository.GetTestBy_Id(Id);
        }

        public Test GetObjectWithTestType(long Id)
        {
            return testRepository.GetObjectWithTestType(Id);
        }

        public Test GetObjectWithTestTypeItemType(long Id)
        {
            return testRepository.GetObjectWithTestTypeItemType(Id);
        }

        public List<Test> GetByTestType(long Id)
        {
            return testRepository.GetByTestType(Id);
        }

        public IEnumerable<BlockItem> GetItemsByTest(long test_id, Guid UsuId, ref Pager pager)
        {
            return blockRepository.GetItemsByTestId(test_id, UsuId, ref pager);
        }

        public async Task<IEnumerable<BlockItem>> GetItemsByTestAsync(long test_id, Guid UsuId, int page, int pageItens)
            => await blockRepository.GetItemsByTestIdAsync(test_id, UsuId, page, pageItens);

        public IEnumerable<BlockItem> GetPendingRevokeItems(ref Pager pager, string ItemCode, DateTime? StartDate, DateTime? EndDate, EnumSituation? Situation)
        {
            return blockRepository.GetPendingRevokeItems(ref pager, ItemCode, StartDate, EndDate, Situation);
        }

        public IEnumerable<TestResult> _SearchTests(TestFilter filter, ref Pager pager)
        {
            switch (filter.vis_id)
            {
                case EnumSYS_Visao.Administracao:
                    return testRepository._SearchTests(filter, ref pager);
                case EnumSYS_Visao.Gestao:
                case EnumSYS_Visao.UnidadeAdministrativa:
                case EnumSYS_Visao.Individual:
                    return testRepository._SearchTestsUser(filter, ref pager);
                default:
                    return null;
            }
        }

        public ReportCorrectionTestResult GetInfoReportCorrection(long test_id)
        {
            return FormatReturnReportCorrection(test_id);
        }
        public ReportCorrectionTestResult GetInfoReportCorrection(long test_id, Guid ent_id, Guid uad_id)
        {
            GestaoEscolar.Entities.SYS_UnidadeAdministrativa dre = unidadeAdministrativaBusiness.GetByUad_Id(uad_id);
            ReportCorrectionTestResult reportCorrectionTestResult = ValidateReportCorrection<GestaoEscolar.Entities.SYS_UnidadeAdministrativa>(dre);
            if (reportCorrectionTestResult.Validate.IsValid)
            {
                reportCorrectionTestResult = FormatReturnReportCorrection(test_id, dre);
            }
            else
                reportCorrectionTestResult.Validate.Message = "Unidade administrativa não encontrada";
            return reportCorrectionTestResult;


        }
        public ReportCorrectionTestResult GetInfoReportCorrection(long test_id, Guid ent_id, Guid uad_id, long esc_id)
        {

            GestaoEscolar.Entities.ESC_Escola escola = escolaBusiness.GetWithAdministrativeUnity(ent_id, esc_id);
            GestaoEscolar.Entities.SYS_UnidadeAdministrativa dre = new GestaoEscolar.Entities.SYS_UnidadeAdministrativa();

            ReportCorrectionTestResult reportCorrectionTestResult = ValidateReportCorrection<GestaoEscolar.Entities.ESC_Escola>(escola);
            if (reportCorrectionTestResult.Validate.IsValid)
            {
                dre.uad_id = escola.SYS_UnidadeAdministrativa.uad_id;
                dre.uad_nome = escola.SYS_UnidadeAdministrativa.uad_nome;
                reportCorrectionTestResult = FormatReturnReportCorrection(test_id, dre, escola);
            }
            else
                reportCorrectionTestResult.Validate.Message = "Unidade administrativa não encontrada";

            return reportCorrectionTestResult;
        }
        public ReportCorrectionTestResult GetInfoReportCorrection(long test_id, Guid ent_id, Guid uad_id, long esc_id, long tur_id)
        {
            GestaoEscolar.Entities.SYS_UnidadeAdministrativa dre = new GestaoEscolar.Entities.SYS_UnidadeAdministrativa();
            GestaoEscolar.Entities.ESC_Escola escola = escolaBusiness.GetWithAdministrativeUnity(ent_id, esc_id);

            ReportCorrectionTestResult reportCorrectionTestResult = ValidateReportCorrection<GestaoEscolar.Entities.ESC_Escola>(escola);
            if (reportCorrectionTestResult.Validate.IsValid)
            {

                dre.uad_id = escola.SYS_UnidadeAdministrativa.uad_id;
                dre.uad_nome = escola.SYS_UnidadeAdministrativa.uad_nome;
                TUR_TurmaDTO turma = turmaBusiness.GetWithTurnoAndModality(tur_id);
                reportCorrectionTestResult = ValidateReportCorrection<TUR_TurmaDTO>(turma);
                if (reportCorrectionTestResult.Validate.IsValid)
                {

                    reportCorrectionTestResult = FormatReturnReportCorrection(test_id, dre, escola, turma);
                }
                else
                    reportCorrectionTestResult.Validate.Message = "Turma não encontrada";
            }
            else
                reportCorrectionTestResult.Validate.Message = "Escola não encontrada";

            return reportCorrectionTestResult;
        }
        private ReportCorrectionTestResult ValidateReportCorrection<T>(T entity)
        {
            ReportCorrectionTestResult reportCorrectionTestResult = new ReportCorrectionTestResult();
            if (entity == null)
            {
                reportCorrectionTestResult.Validate.IsValid = false;
            }
            return reportCorrectionTestResult;
        }
        private ReportCorrectionTestResult FormatReturnReportCorrection(long test_id = 0, GestaoEscolar.Entities.SYS_UnidadeAdministrativa dre = null,
            GestaoEscolar.Entities.ESC_Escola escola = null, TUR_TurmaDTO turma = null)
        {
            ReportCorrectionTestResult reportCorrectionTestResult = new ReportCorrectionTestResult();
            if (test_id > 0)
            {
                Test test = testRepository.GetObject(test_id);
                if (test != null)
                {
                    reportCorrectionTestResult.Test_Id = test.Id;
                    reportCorrectionTestResult.Description = test.Description;
                    reportCorrectionTestResult.ApplicationStartDate = test.ApplicationStartDate;
                    reportCorrectionTestResult.ApplicationEndDate = test.ApplicationEndDate;
                    reportCorrectionTestResult.CorrectionStartDate = test.CorrectionStartDate;
                    reportCorrectionTestResult.CorrectionEndDate = test.CorrectionEndDate;
                    reportCorrectionTestResult.ProcessedCorrectionDate = test.ProcessedCorrectionDate;
                }
                else
                {
                    reportCorrectionTestResult.Validate.IsValid = false;
                    reportCorrectionTestResult.Validate.Message = "Prova não encontrada";
                }
            }
            if (dre != null)
            {

                reportCorrectionTestResult.uad_id = dre.uad_id;
                reportCorrectionTestResult.uad_nome = dre.uad_nome;
            }
            if (escola != null)
            {
                reportCorrectionTestResult.esc_id = escola.esc_id;
                reportCorrectionTestResult.esc_nome = escola.esc_nome;
            }
            if (turma != null)
            {
                reportCorrectionTestResult.tur_id = turma.tur_id;
                reportCorrectionTestResult.tur_descricao = string.Format("{0} - {1} - {2}", turma.tur_codigo, turma.ACA_TipoTurno.ttn_nome, turma.ACA_TipoModalidadeEnsino.tme_nome);
            }

            return reportCorrectionTestResult;
        }

        public IEnumerable<TestResult> GetTestByDate(TestFilter filter)
        {
            IEnumerable<TestResult> tests = testRepository.GetTestByDate(filter);
            return tests;
        }

        public IEnumerable<TestResult> GetTestByDateWithGroup(TestFilter filter)
        {
            IEnumerable<TestResult> tests = testRepository.GetTestByDateWithGroup(filter);
            return tests;
        }

        public Test GetTestById(long Id)
        {
            Test test = testRepository.GetTestById(Id);
            if (test.Discipline == null && test.Multidiscipline)
            {
                test.Discipline = new Discipline();
                test.Discipline.Description = "Multidisciplinar";
            }
            if (test != null)
                test.TestSituation = TestSituation(test);
            if (test != null && test.TestTai)
                test.NumberItemsAplicationTai = numberItemsAplicationTaiRepository.GetByTestId(test.Id);
            return test;
        }

        public AdherenceTest GetObjectToAdherence(long Id)
        {
            return testRepository.GetObjectToAdherence(Id);
        }

        public EnumTestSituation TestSituation(Test entity)
        {
            if (entity.TestSituation.Equals(EnumTestSituation.Registered))
            {
                if (DateTime.Now.Date < entity.ApplicationStartDate.Date)
                    return EnumTestSituation.Registered;
                if ((DateTime.Now.Date >= entity.ApplicationStartDate.Date) && (DateTime.Now.Date <= entity.CorrectionEndDate.Date))
                    return EnumTestSituation.Processing;
                if (DateTime.Now.Date > entity.CorrectionEndDate.Date)
                    return EnumTestSituation.Applied;
            }
            return EnumTestSituation.Pending;
        }

        public List<AnswerSheetBatchItems> GetTestAnswers(long Id)
        {
            return testRepository.GetTestAnswers(Id);
        }

        public KeyValuePair<long, long> GetTestItem(long Id, int ItemOrder, int AlternativeOrder)
        {
            return testRepository.GetTestItem(Id, ItemOrder, AlternativeOrder);
        }

        public IEnumerable<AnswerSheetStudentInformation> GetTeamStudents(int SchoolId, long SectionId, long StudentId, long test_id, bool allAdhered)
        {
            return testRepository.GetTeamStudents(SchoolId, SectionId, StudentId, test_id, allAdhered);
        }

        public IEnumerable<AnswerSheetBatch> GetTestAutomaticCorrectionSituation(long testId, long schoolId)
        {
            return testRepository.GetTestAutomaticCorrectionSituation(testId, schoolId);
        }
        public IEnumerable<Test> GetByTypeCurriculumGrade(int typeCurriculumGrade)
        {
            return testRepository.GetByTypeCurriculumGrade(typeCurriculumGrade);
        }
        public IEnumerable<Test> TestByUser(TestFilter filter)
        {
            return testRepository.TestByUser(filter);
        }

        public IEnumerable<Test> GetInCorrection()
        {
            return testRepository.GetInCorrection();
        }

        public IEnumerable<CorrectionStudentGrid> GetByTestSection(long test_id, long tur_id)
        {
            return testRepository.GetByTestSection(test_id, tur_id);
        }

        public IEnumerable<Test> GetTestFinishedCorrection(bool allTests)
        {
            return testRepository.GetTestFinishedCorrection(allTests);
        }

        public string GetTestFeedbackHtml(Test entity)
        {
            List<AnswerSheetBatchItems> answers = GetTestAnswers(entity.Id);

            StringBuilder html = new StringBuilder("<div class='pdf-content preview'><div class='test-feedback'><label class='section-title'>Gabarito da prova</label>");
            html.AppendLine("<div class='testTitleSection'><h2 class='prova-description'>");
            html.AppendLine(entity.Description);
            html.AppendLine("</h2></div>");
            html.AppendLine("<br clear='all' /><div class='answerFrame'><div class='table-pdf'>");
            html.AppendLine("<div class='div-columns'><table>");
            html.AppendLine("<tbody><tr><th>CÓDIGO DO ITEM</th><th>ITEM</th><th>GABARITO</th></tr>");
            foreach (AnswerSheetBatchItems item in answers.Where(a => a.Correct).OrderBy(a => a.Order))
            {
                html.AppendFormat("<tr><td>{0}</td><td>{1}</td><td><div class='number'>{2}</div></td></tr>", item.ItemCode, ++item.Order, StringHelper.RemoveSpecialCharactersWithRegex(item.Numeration, ""));
            }
            html.AppendLine("</tbody></table></div></div></div></div></div>");

            return html.ToString();
        }

        public IEnumerable<DisciplineItem> GetDisciplineItemByTestId(long test_id)
        {
            return testRepository.GetDisciplineItemByTestId(test_id);
        }

        public IEnumerable<TestResult> GetTestsBySubGroup(long id)
        {
            return testRepository.GetTestsBySubGroup(id);
        }

        public IEnumerable<TestResult> GetTestsBySubGroupTcpId(long id, long tcp_id)
        {
            return testRepository.GetTestsBySubGroupTcpId(id, tcp_id);
        }

        public Task<List<ElectronicTestDTO>> SearchEletronicTests() => testRepository.SearchEletronicTests();

        public async Task<List<ElectronicTestDTO>> SearchEletronicTestsByPesId(Guid pes_id)
        {
            var tests = await testRepository.SearchEletronicTestsByPesId(pes_id);
            tests = await FilterTestsTargetToStudentsWithDeficiencies(pes_id, tests);
            return tests;
        }
        public async Task<List<ElectronicTestDTO>> GetTestsByPesId(Guid pes_id)
        {
            var tests = await testRepository.GetTestsByPesId(pes_id);
            tests = await FilterTestsTargetToStudentsWithDeficiencies(pes_id, tests);
            return tests;
        }

        public async Task<ElectronicTestDTO> GetElectronicTestByPesIdAndTestId(Guid pes_id, long testId)
            => await testRepository.GetElectronicTestByPesIdAndTestId(pes_id, testId);

        private async Task<List<ElectronicTestDTO>> FilterTestsTargetToStudentsWithDeficiencies(Guid pes_id, List<ElectronicTestDTO> tests)
        {
            var testsTargetToStudentsWithDeficiencies = tests.Where(x => x.TargetToStudentsWithDeficiencies).ToList();
            if (!testsTargetToStudentsWithDeficiencies.Any()) return tests;

            var studentDeficiencies = await testRepository.GetStudentDeficiencies(pes_id);
            if (!studentDeficiencies?.Any() ?? true)
                return tests.Except(testsTargetToStudentsWithDeficiencies).ToList();

            foreach (var test in testsTargetToStudentsWithDeficiencies)
            {
                var testTypeDeficiencies = testTypeDeficiencyRepository.Get(test.TestTypeId);
                if (!studentDeficiencies.Any(x => testTypeDeficiencies.Any(y => y.DeficiencyId == x)))
                    tests.RemoveAll(x => x.Id == test.Id);
            }

            return tests;
        }

        public async Task<Test> SearchInfoTestAsync(long test_id) => await testRepository.SearchInfoTestAsync(test_id);

        public bool ExistsAdherenceByAluIdTestId(long alu_id, long test_id) => testRepository.ExistsAdherenceByAluIdTestId(alu_id, test_id);

        #endregion

        #region Write

        public Test Save(Test entity, Guid usu_id, bool isAdmin)
        {
            DateTime dateNow = DateTime.Now;

            entity.CreateDate = dateNow;
            entity.UpdateDate = dateNow;

            entity.Validate = Validate(entity, ValidateAction.Save, entity.Validate, isAdmin);
            if (entity.Validate.IsValid)
            {
                if (entity.NumberItem == 0)
                {
                    foreach (var item in entity.TestItemLevels)
                    {
                        entity.NumberItem += item.Value;
                    }
                }

                entity = testRepository.Save(entity, usu_id);

                if (entity.TestTai)
                {

                    var itemTestTai = new NumberItemTestTai(entity.Id, entity.NumberItemsAplicationTai.Id, entity.AdvanceWithoutAnswering, entity.BackToPreviousItem);
                    numberItemTestTaiRepository.Save(itemTestTai);
                }

                entity.Validate.Type = ValidateType.Save.ToString();
                entity.Validate.Message = "Prova salva com sucesso.";
            }

            return entity;
        }

        public Test Update(long Id, Test entity, Guid UsuId, bool isAdmin)
        {
            entity.Validate = Validate(entity, ValidateAction.Update, entity.Validate, isAdmin, UsuId);

            if (entity.Validate.IsValid)
            {
                if (entity.NumberItem == 0)
                {
                    foreach (var item in entity.TestItemLevels)
                    {
                        entity.NumberItem += item.Value;
                    }
                }

                if (entity.TestTai)
                {
                    if (entity.NumberItemsAplicationTai != null)
                    {
                        var itemTestTai = new NumberItemTestTai(entity.Id, entity.NumberItemsAplicationTai.Id, entity.AdvanceWithoutAnswering, entity.BackToPreviousItem);
                        numberItemTestTaiRepository.DeleteSaveByTestId(itemTestTai);
                    }
                    else
                    {
                        throw new Exception("É preciso informar uma opção de quantidade de amostras para prova com aplicação TAI.");
                    }
                }
                else if (entity.NumberItemsAplicationTai != null)
                    numberItemTestTaiRepository.DeleteByTestId(entity.Id);

                entity.TestSituation = ValidateTestSituation(entity);

                var test = testRepository.Update(Id, entity);

                entity.BlockChains.AddRange(test.BlockChains);
                entity.Blocks.AddRange(test.Blocks);
                entity.RemoveBlockChain = test.RemoveBlockChain;
                entity.RemoveBlockChainBlock = test.RemoveBlockChainBlock;
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Prova alterada com sucesso.";
            }

            return entity;
        }

        public Test FinallyTest(long Id, Guid UsuId, bool isAdmin)
        {
            Test entity = new Test { Id = Id };

            entity.Validate = this.Validate(entity, ValidateAction.Update, entity.Validate, isAdmin, UsuId);

            if (entity.Validate.IsValid)
            {
                var files = fileBusiness.GetFilesByParent(Id, EnumFileType.Test);
                var bookletList = bookletBusiness.GetAllByTest(Id).ToList();

                if (files != null && (files.FirstOrDefault(p => p.Path == null) == null) && (files.Count() > 0 && bookletList.Count > 0) && (files.Count() == bookletList.Count))
                {
                    entity = testRepository.UpdateSituation(Id, EnumTestSituation.Registered);
                    entity.TestSituation = TestSituation(entity);

                    entity.Validate.Type = ValidateType.Update.ToString();
                    entity.Validate.Message = "Situação da prova alterada com sucesso.";
                }
                else
                {
                    entity.Validate.IsValid = false;
                    entity.Validate.Code = 400;
                    entity.Validate.Type = ValidateType.alert.ToString();
                    entity.Validate.Message = "Não foi possível finalizar pois ainda existe(m) prova(s) não gerada(s).";
                }
            }

            return entity;
        }

        public Test Delete(long Id)
        {
            Test entity = new Test { Id = Id };
            testRepository.Delete(Id);
            fileRepository.DeleteByParentId(Id, EnumFileType.Test);
            testPerformanceLevelRepository.DeleteByTestId(Id);
            entity.Validate.Type = ValidateType.Delete.ToString();
            entity.Validate.Message = "Prova excluída com sucesso.";

            return entity;
        }

        public void SwitchAllAdhrered(Test test)
        {
            testRepository.SwitchAllAdhrered(test);
        }

        public Test Update2(long Id, Test entity, Guid UsuId, bool isAdmin)
        {
            // verifica se alterou alguma coisa relativa ao conteúdo da prova, para deixar no status pendente novamente
            bool alterouConteudoProva = false;

            entity.Validate = Validate(entity, ValidateAction.Update, entity.Validate, isAdmin, UsuId);

            DateTime dateNow = DateTime.Now;
            Test oldEntity = testRepository.GetTestById(Id);

            if (!GestaoAvaliacao.Util.Compare.ValidateEqualsInt(entity.TestType.Id, oldEntity.TestType.Id)
                || !GestaoAvaliacao.Util.Compare.ValidateEqualsString(entity.Description, oldEntity.Description)
                || !GestaoAvaliacao.Util.Compare.ValidateEqualsInt(entity.Discipline.Id, oldEntity.Discipline.Id)
                || !GestaoAvaliacao.Util.Compare.ValidateEqualsInt(entity.FrequencyApplication, oldEntity.FrequencyApplication)
                || !GestaoAvaliacao.Util.Compare.ValidateEqualsInt((long)entity.NumberItem, (long)oldEntity.NumberItem))
            {
                alterouConteudoProva = true;
            }

            // get para pegar o objeto antigo e validações na business e retirar do repositório
            oldEntity.ApplicationEndDate = entity.ApplicationEndDate;
            oldEntity.ApplicationStartDate = entity.ApplicationStartDate;
            oldEntity.Bib = entity.Bib;
            oldEntity.FrequencyApplication = entity.FrequencyApplication;
            oldEntity.CorrectionEndDate = entity.CorrectionEndDate;
            oldEntity.CorrectionStartDate = entity.CorrectionStartDate;

            oldEntity.NumberBlock = entity.NumberBlock;
            oldEntity.NumberItem = entity.NumberItem;
            oldEntity.NumberItemsBlock = entity.NumberItemsBlock;
            oldEntity.KnowledgeAreaBlock = entity.KnowledgeAreaBlock;
            oldEntity.ElectronicTest = entity.ElectronicTest;

            oldEntity.Description = entity.Description;
            oldEntity.UpdateDate = DateTime.Now;

            oldEntity.Discipline = null;
            oldEntity.Discipline_Id = entity.Discipline.Id;

            if (entity.FormatType != null)
            {
                oldEntity.FormatType = null;
                oldEntity.FormatType_Id = entity.FormatType.Id;
            }

            oldEntity.TestType = null;
            oldEntity.TestType_Id = entity.TestType.Id;

            oldEntity.BlockChain = entity.BlockChain;
            oldEntity.BlockChainNumber = entity.BlockChainNumber;
            oldEntity.BlockChainItems = entity.BlockChainItems;
            oldEntity.BlockChainForBlock = entity.BlockChainForBlock;

            #region testCurriculumGrades

            if (oldEntity.TestCurriculumGrades.Count != entity.TestCurriculumGrades.Count)
                alterouConteudoProva = true;

            List<TestCurriculumGrade> testCurriculumGrades = new List<TestCurriculumGrade>();

            foreach (TestCurriculumGrade testCurriculumGrade in oldEntity.TestCurriculumGrades.Where(p => p.State != Convert.ToByte(EnumState.excluido)))
            {
                if (!entity.TestCurriculumGrades.Any(p => p.TypeCurriculumGradeId == testCurriculumGrade.TypeCurriculumGradeId))
                {
                    testCurriculumGrade.State = Convert.ToByte(EnumState.excluido);
                    testCurriculumGrade.UpdateDate = dateNow;
                    testCurriculumGrades.Add(testCurriculumGrade);
                }
                else
                {
                    testCurriculumGrade.UpdateDate = dateNow;
                    testCurriculumGrades.Add(testCurriculumGrade);
                }
                entity.TestCurriculumGrades.RemoveAll(p => p.TypeCurriculumGradeId == testCurriculumGrade.TypeCurriculumGradeId);
            }

            testCurriculumGrades.AddRange(entity.TestCurriculumGrades);

            if (testCurriculumGrades != null && testCurriculumGrades.Count > 0)
                oldEntity.TestCurriculumGrades.AddRange(testCurriculumGrades);

            #endregion

            #region testItemLevels

            List<TestItemLevel> itemlevels = new List<TestItemLevel>();

            foreach (TestItemLevel testitemlevel in oldEntity.TestItemLevels.Where(p => p.State != Convert.ToByte(EnumState.excluido)))
            {
                if (!entity.TestItemLevels.Any(p => p.ItemLevel.Id == testitemlevel.ItemLevel.Id))
                {
                    testitemlevel.State = Convert.ToByte(EnumState.excluido);
                    testitemlevel.UpdateDate = dateNow;
                    itemlevels.Add(testitemlevel);
                }
                else
                {
                    testitemlevel.Value = entity.TestItemLevels.FirstOrDefault(p => p.ItemLevel.Id == testitemlevel.ItemLevel.Id).Value;
                    testitemlevel.PercentValue = entity.TestItemLevels.FirstOrDefault(p => p.ItemLevel.Id == testitemlevel.ItemLevel.Id).PercentValue;
                    testitemlevel.UpdateDate = dateNow;
                    itemlevels.Add(testitemlevel);
                }
                entity.TestItemLevels.RemoveAll(p => p.ItemLevel.Id == testitemlevel.ItemLevel.Id);
            }

            foreach (var t in entity.TestItemLevels)
            {
                // validar
                t.ItemLevel = itemLevelRepository.Get(t.ItemLevel.Id);
            }

            itemlevels.AddRange(entity.TestItemLevels);

            if (itemlevels != null && itemlevels.Count > 0)
                oldEntity.TestItemLevels.AddRange(itemlevels);

            #endregion

            #region testPerformanceLevels

            List<TestPerformanceLevel> performancelevels = new List<TestPerformanceLevel>();

            foreach (TestPerformanceLevel testperformancelevel in oldEntity.TestPerformanceLevels.Where(p => p.State != Convert.ToByte(EnumState.excluido)))
            {
                if (!entity.TestPerformanceLevels.Any(p => p.PerformanceLevel.Id == testperformancelevel.PerformanceLevel.Id))
                {
                    testperformancelevel.State = Convert.ToByte(EnumState.excluido);
                    testperformancelevel.UpdateDate = dateNow;
                    performancelevels.Add(testperformancelevel);
                }
                else
                {
                    testperformancelevel.Value1 = entity.TestPerformanceLevels.FirstOrDefault(p => p.PerformanceLevel.Id == testperformancelevel.PerformanceLevel.Id).Value1;
                    testperformancelevel.Value2 = entity.TestPerformanceLevels.FirstOrDefault(p => p.PerformanceLevel.Id == testperformancelevel.PerformanceLevel.Id).Value2;
                    testperformancelevel.UpdateDate = dateNow;
                    performancelevels.Add(testperformancelevel);
                }
                entity.TestPerformanceLevels.RemoveAll(p => p.PerformanceLevel.Id == testperformancelevel.PerformanceLevel.Id);
            }

            foreach (var t in entity.TestPerformanceLevels)
            {
                // validar
                t.PerformanceLevel = performanceLevelRepository.Get(t.PerformanceLevel.Id);
            }

            performancelevels.AddRange(entity.TestPerformanceLevels);

            if (performancelevels != null && performancelevels.Count > 0)
                oldEntity.TestPerformanceLevels.AddRange(performancelevels);

            #endregion

            if (alterouConteudoProva)
            {
                oldEntity.TestSituation = EnumTestSituation.Pending;
            }

            if (entity.Validate.IsValid)
            {
                testRepository.Update(oldEntity);
                entity.Validate.Type = ValidateType.Update.ToString();
                entity.Validate.Message = "Prova alterada com sucesso.";
            }

            return entity;
        }

        public TestShowVideoAudioFilesDto GetTestShowVideoAudioFiles(long testId) => testRepository.GetTestShowVideoAudioFiles(testId);

        public GenerateTestDTO GenerateTest(long Id, bool sheet, bool publicFeedback, bool CDNMathJax, string separator, SYS_Usuario Usuario, SYS_Grupo Grupo, string UrlSite, string VirtualDirectory, string PhysicalDirectory)
        {
            Booklet entity = bookletBusiness.GetTestBooklet(Id);
            GenerateTestDTO ret = new GenerateTestDTO();

            if (entity != null)
            {
                var valid = CanEdit(entity.Test.Id, Usuario.usu_id, (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), Grupo.vis_id.ToString()));

                if (valid.IsValid)
                {
                    PDFFilter filter = new PDFFilter
                    {
                        Booklet = entity,
                        Test = entity.Test,
                        FontSize = 20,
                        UrlSite = UrlSite,
                        ContentType = "application/pdf",
                        VirtualDirectory = VirtualDirectory,
                        PhysicalDirectory = PhysicalDirectory,
                        GenerateType = EnumGenerateType.Test,
                        FileType = EnumFileType.Test,
                        CDNMathJax = CDNMathJax
                    };

                    EntityFile file = bookletBusiness.SavePdfTest(filter, Usuario.ent_id);

                    if (file.Validate.IsValid)
                    {
                        #region Folha de resposta

                        EntityFile answerSheetFile = null;
                        if (sheet)
                        {
                            filter.GenerateType = EnumGenerateType.AnswerSheet;
                            filter.FileType = EnumFileType.AnswerSheetStudentNumber;
                            filter.OwnerId = filter.Booklet.Id;
                            filter.ParentOwnerId = filter.Booklet.Test.Id;

                            answerSheetFile = bookletBusiness.SavePdfTest(filter, Usuario.ent_id);
                            file.Validate.IsValid = answerSheetFile.Validate.IsValid;
                        }
                        else
                        {
                            IEnumerable<EntityFile> filesAnswerSheet = fileBusiness.GetFilesByParent(entity.Test.Id, EnumFileType.AnswerSheetStudentNumber);
                            foreach (EntityFile fileSheet in filesAnswerSheet)
                            {
                                EntityFile f = fileBusiness.LogicalDelete(fileSheet.Id, Usuario.usu_id, Grupo.vis_id);
                                string path = string.Concat(PhysicalDirectory, new Uri(fileSheet.Path).AbsolutePath.Replace("/", "\\"));
                                if (f.Validate.IsValid && !string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                                    System.IO.File.Delete(path);
                            }
                        }

                        #endregion

                        #region Gabarito

                        List<AnswerSheetBatchItems> answers = GetTestAnswers(entity.Test.Id);

                        StringBuilder stringBuilder = new StringBuilder(entity.Test.Description);
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine(string.Format("CÓDIGO DO ITEM{0} ITEM{0} GABARITO{0}", separator));
                        foreach (AnswerSheetBatchItems item in answers.Where(a => a.Correct).OrderBy(a => a.Order))
                        {
                            stringBuilder.Append(item.ItemCode + separator);
                            stringBuilder.Append(++item.Order + separator);
                            stringBuilder.AppendLine(StringHelper.RemoveSpecialCharactersWithRegex(item.Numeration, "") + separator);
                        }

                        byte[] buffer = System.Text.Encoding.Default.GetBytes(stringBuilder.ToString());
                        string originalName = string.Format("Gabarito_Prova_{0}.csv", entity.Test.Id);
                        string name = string.Format("{0}.csv", Guid.NewGuid());
                        string contentType = MimeType.CSV.GetDescription();
                        EntityFile filedb = fileBusiness.GetFilesByOwner(entity.Id, entity.Test.Id, EnumFileType.TestFeedback).FirstOrDefault();
                        if (filedb != null)
                        {
                            if (!string.IsNullOrEmpty(filedb.OriginalName))
                                originalName = filedb.OriginalName;

                            name = filedb.Name;
                        }

                        EntityFile feedbackFile = storage.Save(buffer, name, contentType, EnumFileType.TestFeedback.GetDescription(), filter.VirtualDirectory, filter.PhysicalDirectory, out feedbackFile);
                        if (feedbackFile.Validate.IsValid)
                        {
                            feedbackFile.ContentType = contentType;
                            feedbackFile.OriginalName = StringHelper.Normalize(originalName);
                            feedbackFile.OwnerId = entity.Id;
                            feedbackFile.ParentOwnerId = entity.Test.Id;

                            if (filedb != null)
                            {
                                fileBusiness.Update(filedb.Id, feedbackFile);
                            }
                            else
                            {
                                feedbackFile.OwnerType = (byte)EnumFileType.TestFeedback;
                                fileBusiness.Save(feedbackFile);
                            }
                        }

                        testRepository.UpdateTestFeedback(entity.Test.Id, publicFeedback);

                        #endregion

                        if (file.Validate.IsValid)
                        {
                            ret.Validate = file.Validate;
                            ret.File = file != null ? new TestFileDTO
                            {
                                Id = file.Id,
                                Name = !string.IsNullOrEmpty(file.OriginalName) ? file.OriginalName : file.Name,
                                Path = file.Path,
                                GenerationData = !file.UpdateDate.Equals(file.CreateDate) ? file.UpdateDate.ToString("dd/MM/yyyy") : file.CreateDate.ToString("dd/MM/yyyy")
                            } : null;
                            ret.FileAnswerSheet = answerSheetFile != null ? new TestFileDTO
                            {
                                Id = answerSheetFile.Id,
                                Name = !string.IsNullOrEmpty(answerSheetFile.OriginalName) ? answerSheetFile.OriginalName : answerSheetFile.Name,
                                Path = answerSheetFile.Path,
                                GenerationData = !answerSheetFile.UpdateDate.Equals(answerSheetFile.CreateDate) ? answerSheetFile.UpdateDate.ToString("dd/MM/yyyy") : answerSheetFile.CreateDate.ToString("dd/MM/yyyy")
                            } : null;
                            ret.FileFeedback = feedbackFile != null ? new TestFileDTO
                            {
                                Id = feedbackFile.Id,
                                Name = !string.IsNullOrEmpty(feedbackFile.OriginalName) ? feedbackFile.OriginalName : feedbackFile.Name,
                                Path = feedbackFile.Path,
                                GenerationData = !feedbackFile.UpdateDate.Equals(feedbackFile.CreateDate) ? feedbackFile.UpdateDate.ToString("dd/MM/yyyy") : feedbackFile.CreateDate.ToString("dd/MM/yyyy")
                            } : null;
                        }
                    }
                }
                else
                {
                    ret.Validate.IsValid = false;
                    ret.Validate.Type = ValidateType.alert.ToString();
                    ret.Validate.Message = valid.Message;
                }
            }
            else
            {
                ret.Validate.IsValid = false;
                ret.Validate.Type = ValidateType.alert.ToString();
                ret.Validate.Message = "Não existe o caderno selecionado.";
            }

            return ret;
        }

        public GenerateTestDTO ExportTestDoc(long Id, PDFFilter filter, SYS_Usuario Usuario, SYS_Grupo Grupo)
        {
            Booklet booklet = bookletBusiness.GetTestBooklet(Id);
            GenerateTestDTO ret = new GenerateTestDTO();
            if (booklet != null)
            {
                var valid = CanEdit(booklet.Test.Id, Usuario.usu_id, (EnumSYS_Visao)Enum.Parse(typeof(EnumSYS_Visao), Grupo.vis_id.ToString()));

                if (valid.IsValid)
                {
                    string url = string.Format("http://{0}/TestContent?id={1}&EntityId={2}&generateType={3}&fileType={4}&preview={5}", filter.UrlSite, booklet.Id, Usuario.ent_id, (byte)filter.GenerateType, (byte)filter.FileType, filter.PDFPreview);
                    using (WebClient client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        ret.Html = client.DownloadString(url);
                    }
                    ret.Html = ret.Html.Replace(@"<div><input class=""form-control"" type=""text"" #school#=""""></div>", "______________________________________________________________");
                    ret.Html = ret.Html.Replace(@"<div><input class=""form-control"" type=""text"" #studentname#=""""></div>", "______________________________________________________________");
                    ret.Html = ret.Html.Replace(@"<div><input class=""form-control"" type=""text"" #teachername#=""""></div>", "______________________________________________________________");
                    ret.Html = ret.Html.Replace(@"<div><input class=""form-control"" type=""text"" #classname#=""""></div>", "______________________________________________________________");
                    ret.Html = ret.Html.Replace(@"<div><input class=""form-control"" type=""text"" #studentnumber#=""""></div>", "______________________________________________________________");
                    ret.Html = ret.Html.Replace(@"<div><input class=""form-control"" type=""text"" #date#=""""></div>", "______________________________________________________________");
                    ret.Html = ret.Html.Replace(@"<div class='line-separation'></div>", "___________________________________________________________________");
                    ret.File = new TestFileDTO();
                    ret.File.Name = booklet.Test.Description;
                }
                else
                {
                    ret.Validate.IsValid = false;
                    ret.Validate.Type = ValidateType.alert.ToString();
                    ret.Validate.Message = valid.Message;
                }
            }
            else
            {
                ret.Validate.IsValid = false;
                ret.Validate.Type = ValidateType.alert.ToString();
                ret.Validate.Message = "Não existe o caderno selecionado.";
            }
            return ret;
        }

        public void UpdateTestProcessedCorrection(long Id, bool processedCorrection)
        {
            testRepository.UpdateTestProcessedCorrection(Id, processedCorrection);
        }

        public void UpdateTestVisible(long Id, bool visible)
        {
            testRepository.UpdateTestVisible(Id, visible);
        }

        public void ChangeOrderTestUp(long Id, long order)
        {
            Test test = testRepository.GetTestBy_Id(Id);
            Test testAcima = testRepository.SelectOrderTestUp(order);

            long ordem = test.Order;
            long ordemAcima = testAcima.Order;

            test.Order = ordemAcima;
            testAcima.Order = ordem;

            testRepository.Update(test);
            testRepository.Update(testAcima);
        }

        public void ChangeOrderTestDown(long Id, long order)
        {
            Test test = testRepository.GetTestBy_Id(Id);
            Test testAbaixo = testRepository.SelectOrderTestDown(order);

            long ordem = test.Order;
            long ordemAbaixo = testAbaixo.Order;

            test.Order = ordemAbaixo;
            testAbaixo.Order = ordem;

            testRepository.Update(test);
            testRepository.Update(testAbaixo);
        }

        public void ChangeOrder(long idOrigem, long idDestino)
        {
            Test testOrigem = testRepository.GetTestBy_Id(idOrigem);
            Test testDestino = testRepository.GetTestBy_Id(idDestino);

            long ordemOrigem = testOrigem.Order;
            long ordemDestino = testDestino.Order;

            testOrigem.Order = ordemDestino;
            testDestino.Order = ordemOrigem;

            testRepository.Update(testOrigem);
            testRepository.Update(testDestino);
        }

        public void TestTaiCurriculumGradeSave(List<TestTaiCurriculumGrade> listEntity)
        {
            try
            {
                var testId = listEntity.FirstOrDefault().TestId;

                if (testId <= 0)
                    throw new Exception("TestId é obrigatório");

                var listTestTaiCurriculumGrade = testTaiCurriculumGradeRepository.GetListByTestId(testId);

                if (listTestTaiCurriculumGrade == null || !listTestTaiCurriculumGrade.Any())
                {
                    foreach (var entity in listEntity)
                    {
                        testTaiCurriculumGradeRepository.Save(entity);
                    }
                }
                else
                {

                    var testTaiCurriculumGradeNew = listEntity.Where(x => !listTestTaiCurriculumGrade.Any(entity => x.DisciplineId == entity.DisciplineId
                                                                                                                 && x.MatrixId == entity.MatrixId
                                                                                                                 && x.TypeCurriculumGradeId == entity.TypeCurriculumGradeId));

                    var testTaiCurriculumGradeExists = listTestTaiCurriculumGrade.Where(x => listEntity.Any(entity => x.DisciplineId == entity.DisciplineId
                                                                                                                   && x.MatrixId == entity.MatrixId
                                                                                                                   && x.TypeCurriculumGradeId == entity.TypeCurriculumGradeId));

                    var testTaiCurriculumGradeNotExists = listTestTaiCurriculumGrade.Where(x => !listEntity.Any(entity => x.DisciplineId == entity.DisciplineId
                                                                                                                      && x.MatrixId == entity.MatrixId
                                                                                                                      && x.TypeCurriculumGradeId == entity.TypeCurriculumGradeId));
                    //Inserir
                    foreach (var entity in testTaiCurriculumGradeNew)
                    {
                        testTaiCurriculumGradeRepository.Save(entity);
                    }

                    //Alterar
                    foreach (var entity in testTaiCurriculumGradeExists)
                    {
                        var testTaiCurriculumGrade = listEntity.FirstOrDefault(x => x.DisciplineId == entity.DisciplineId
                                                                                    && x.MatrixId == entity.MatrixId
                                                                                    && x.TypeCurriculumGradeId == entity.TypeCurriculumGradeId);
                        if (entity.Percentage != testTaiCurriculumGrade.Percentage)
                        {
                            entity.Percentage = testTaiCurriculumGrade.Percentage;
                            entity.UpdateDate = DateTime.Now;
                            testTaiCurriculumGradeRepository.Update(entity);
                        }
                    }

                    //Excluir
                    foreach (var entity in testTaiCurriculumGradeNotExists)
                    {
                        entity.State = 3;
                        entity.UpdateDate = DateTime.Now;
                        testTaiCurriculumGradeRepository.Update(entity);
                    }
                    
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<TestTaiCurriculumGrade> GetListTestTaiCurriculumGrade(long testId)
        {
            if (testId <= 0)
                throw new Exception("O parametro testId é obrigatório e não pode ser 0");

            return testTaiCurriculumGradeRepository.GetListByTestId(testId);
        }

    }

    #endregion
}

