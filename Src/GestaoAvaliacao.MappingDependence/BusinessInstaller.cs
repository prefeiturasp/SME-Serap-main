using AvaliaMais.FolhaRespostas.Application;
using AvaliaMais.FolhaRespostas.Application.Interfaces;
using AvaliaMais.FolhaRespostas.Data.MongoDB.Context;
using AvaliaMais.FolhaRespostas.Data.MongoDB.Repository;
using AvaliaMais.FolhaRespostas.Data.SQLServer.Repository;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial.Interfaces;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial.Services;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva.Interfaces;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva.Services;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FluentValidation;
using GestaoAvaliacao.Business;
using GestaoAvaliacao.Business.StudentsTestSent;
using GestaoAvaliacao.Business.StudentTestAccoplishments;
using GestaoAvaliacao.Business.StudentTestAccoplishments.Validators;
using GestaoAvaliacao.Entities.DTO.StudentTestAccoplishments;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IBusiness.StudentsTestSent;
using GestaoAvaliacao.Repository;
using GestaoAvaliacao.Repository.StudentTestAccoplishments;
using GestaoEscolar.Business;
using GestaoEscolar.IBusiness;
using Microsoft.AspNet.SignalR;

namespace GestaoAvaliacao.MappingDependence
{
    public class BusinessInstaller : IWindsorInstaller
    {
        public BusinessInstaller()
        {
            this.LifestylePerWebRequest = true;
        }

        public bool LifestylePerWebRequest { get; set; }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyContaining<AbsenceReasonBusiness>()
                                .BasedOn(typeof(IAbsenceReasonBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AlternativeBusiness>()
                                .BasedOn(typeof(IAlternativeBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AnswerSheetBusiness>()
                            .BasedOn(typeof(IAnswerSheetBusiness))
                            .WithService.AllInterfaces()
                            .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<BaseTextBusiness>()
                                .BasedOn(typeof(IBaseTextBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<CorrelatedSkillBusiness>()
                                .BasedOn(typeof(ICorrelatedSkillBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<EvaluationMatrixCourseBusiness>()
                                .BasedOn(typeof(IEvaluationMatrixCourseBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<DisciplineBusiness>()
                               .BasedOn(typeof(IDisciplineBusiness))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<EvaluationMatrixCourseCurriculumGradeBusiness>()
                              .BasedOn(typeof(IEvaluationMatrixCourseCurriculumGradeBusiness))
                              .WithService.AllInterfaces()
                              .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<EvaluationMatrixBusiness>()
                                .BasedOn(typeof(IEvaluationMatrixBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemBusiness>()
                                .BasedOn(typeof(IItemBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemLevelBusiness>()
                                .BasedOn(typeof(IItemLevelBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemSituationBusiness>()
                                .BasedOn(typeof(IItemSituationBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemSkillBusiness>()
                                .BasedOn(typeof(IItemSkillBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemTypeBusiness>()
                                .BasedOn(typeof(IItemTypeBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ModelEvaluationMatrixBusiness>()
                             .BasedOn(typeof(IModelEvaluationMatrixBusiness))
                             .WithService.AllInterfaces()
                             .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ModelSkillLevelBusiness>()
                     .BasedOn(typeof(IModelSkillLevelBusiness))
                     .WithService.AllInterfaces()
                     .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ParameterBusiness>()
                 .BasedOn(typeof(IParameterBusiness))
                 .WithService.AllInterfaces()
                 .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<SkillBusiness>()
                             .BasedOn(typeof(ISkillBusiness))
                             .WithService.AllInterfaces()
                             .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<SkillBusiness>()
                                .BasedOn(typeof(ISkillBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTypeBusiness>()
                                .BasedOn(typeof(ITestTypeBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTypeCourseCurriculumGradeBusiness>()
                                .BasedOn(typeof(ITestTypeCourseCurriculumGradeBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTypeDeficiencyBusiness>()
                                .BasedOn(typeof(ITestTypeDeficiencyBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTypeItemLevelBusiness>()
                           .BasedOn(typeof(ITestTypeItemLevelBusiness))
                           .WithService.AllInterfaces()
                           .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<FormatTypeBusiness>()
                           .BasedOn(typeof(IFormatTypeBusiness))
                           .WithService.AllInterfaces()
                            .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<FileBusiness>()
                           .BasedOn(typeof(IFileBusiness))
                           .WithService.AllInterfaces()
                           .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemCurriculumGradeBusiness>()
                        .BasedOn(typeof(IItemCurriculumGradeBusiness))
                        .WithService.AllInterfaces()
                        .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<PerformanceLevelBusiness>()
                        .BasedOn(typeof(IPerformanceLevelBusiness))
                        .WithService.AllInterfaces()
                        .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestBusiness>()
                        .BasedOn(typeof(ITestBusiness))
                        .WithService.AllInterfaces()
                        .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<BlockBusiness>()
                        .BasedOn(typeof(IBlockBusiness))
                        .WithService.AllInterfaces()
                        .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestPerformanceLevelBusiness>()
                        .BasedOn(typeof(ITestPerformanceLevelBusiness))
                        .WithService.AllInterfaces()
                        .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestCurriculumGradeBusiness>()
                        .BasedOn(typeof(ITestCurriculumGradeBusiness))
                        .WithService.AllInterfaces()
                        .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestPermissionBusiness>()
                        .BasedOn(typeof(ITestPermissionBusiness))
                        .WithService.AllInterfaces()
                        .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<BookletBusiness>()
                        .BasedOn(typeof(IBookletBusiness))
                        .WithService.AllInterfaces()
                        .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<CognitiveCompetenceBusiness>()
                                    .BasedOn(typeof(ICognitiveCompetenceBusiness))
                                    .WithService.AllInterfaces()
                                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ModelTestBusiness>()
                                    .BasedOn(typeof(IModelTestBusiness))
                                    .WithService.AllInterfaces()
                                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTypeCourseBusiness>()
                                .BasedOn(typeof(ITestTypeCourseBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestFilesBusiness>()
                                .BasedOn(typeof(ITestFilesBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AdherenceBusiness>()
                                .BasedOn(typeof(IAdherenceBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<CorrectionBusiness>()
                                .BasedOn(typeof(ICorrectionBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<StudentTestAbsenceReasonBusiness>()
                                .BasedOn(typeof(IStudentTestAbsenceReasonBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<StudentCorrectionBusiness>()
                                .BasedOn(typeof(IStudentCorrectionBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<SectionTestStatsBusiness>()
                                .BasedOn(typeof(ISectionTestStatsBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<CorrectionResultsBusiness>()
                                .BasedOn(typeof(ICorrectionResultsBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestSectionStatusCorrectionBusiness>()
                                .BasedOn(typeof(ITestSectionStatusCorrectionBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<SectionTestGenerateLotBusiness>()
                                .BasedOn(typeof(ISectionTestGenerateLotBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AnswerSheetBatchBusiness>()
                                .BasedOn(typeof(IAnswerSheetBatchBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AnswerSheetBatchFilesBusiness>()
                                .BasedOn(typeof(IAnswerSheetBatchFilesBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AnswerSheetBatchLogBusiness>()
                                .BasedOn(typeof(IAnswerSheetBatchLogBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AnswerSheetLotBusiness>()
                                .BasedOn(typeof(IAnswerSheetLotBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<RequestRevokeBusiness>()
                                .BasedOn(typeof(IRequestRevokeBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ExportAnalysisBusiness>()
                                .BasedOn(typeof(IExportAnalysisBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));
            container.Register(Classes.FromAssemblyContaining<PerformanceItemBusiness>()
                                .BasedOn(typeof(IPerformanceItemBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ReportTestPerformanceBusiness>()
                                .BasedOn(typeof(IReportTestPerformanceBusiness))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ReportItemPerformanceBusiness>()
                               .BasedOn(typeof(IReportItemPerformanceBusiness))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ProcessamentoProvaRepository>()
                    .BasedOn(typeof(IProcessamentoProvaRepository))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ProcessamentoProvaService>()
                    .BasedOn(typeof(IProcessamentoProvaService))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ProcessamentoInicialRepository>()
                    .BasedOn(typeof(IProcessamentoInicialRepository))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ProcessamentoInicialService>()
                    .BasedOn(typeof(IProcessamentoInicialService))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ProcessamentoAppService>()
                    .BasedOn(typeof(IProcessamentoAppService))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ProcessamentoAppServiceWrite>()
                    .BasedOn(typeof(IProcessamentoAppServiceWrite))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ExportacaoAppService>()
                    .BasedOn(typeof(IExportacaoAppService))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<MongoDbContext>()
                    .BasedOn(typeof(MongoDbContext))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AnswerSheetBatchQueueBusiness>()
                            .BasedOn(typeof(IAnswerSheetBatchQueueBusiness))
                            .WithService.AllInterfaces()
                            .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ServiceAnswerSheetInfoBusiness>()
                     .BasedOn(typeof(IServiceAnswerSheetInfoBusiness))
                     .WithService.AllInterfaces()
                     .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ReportStudentPerformanceBusiness>()
                     .BasedOn(typeof(IReportStudentPerformanceBusiness))
                     .WithService.AllInterfaces()
                     .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ReportItemChoiceBusiness>()
                    .BasedOn(typeof(IReportItemChoiceBusiness))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestGroupBusiness>()
                            .BasedOn(typeof(ITestGroupBusiness))
                            .WithService.AllInterfaces()
                            .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestSubGroupBusiness>()
                     .BasedOn(typeof(ITestSubGroupBusiness))
                     .WithService.AllInterfaces()
                     .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<KnowledgeAreaBusiness>()
                     .BasedOn(typeof(IKnowledgeAreaBusiness))
                     .WithService.AllInterfaces()
                     .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<KnowledgeAreaDisciplineBusiness>()
                     .BasedOn(typeof(IKnowledgeAreaDisciplineBusiness))
                     .WithService.AllInterfaces()
                     .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<SubjectBusiness>()
                            .BasedOn(typeof(ISubjectBusiness))
                            .WithService.AllInterfaces()
                            .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<SubSubjectBusiness>()
                     .BasedOn(typeof(ISubSubjectBusiness))
                     .WithService.AllInterfaces()
                     .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ResponseChangeLogBusiness>()
                     .BasedOn(typeof(IResponseChangeLogBusiness))
                     .WithService.AllInterfaces()
                     .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<PageConfigurationBusiness>()
                     .BasedOn(typeof(IPageConfigurationBusiness))
                     .WithService.AllInterfaces()
                     .SetLifestyle(LifestylePerWebRequest));
            container.Register(Classes.FromAssemblyContaining<GenerateHtmlBusiness>()
                                 .BasedOn(typeof(IGenerateHtmlBusiness))
                                 .WithService.AllInterfaces()
                                 .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AdministrativeUnitTypeBusiness>()
                     .BasedOn(typeof(IAdministrativeUnitTypeBusiness))
                     .WithService.AllInterfaces()
                     .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemFileBusiness>()
                    .BasedOn(typeof(IItemFileBusiness))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemAudioBusiness>()
                    .BasedOn(typeof(IItemAudioBusiness))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<StudentTestAccoplishmentBusiness>()
                    .BasedOn(typeof(IStudentTestAccoplishmentBusiness))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<StudentTestSentBusiness>()
                    .BasedOn(typeof(IStudentTestSentBusiness))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            #region GestaoEscolar

            container.Register(Classes.FromAssemblyContaining<ACA_CursoBusiness>()
                   .BasedOn(typeof(IACA_CursoBusiness))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_CurriculoPeriodoBusiness>()
                   .BasedOn(typeof(IACA_CurriculoPeriodoBusiness))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_TipoDisciplinaBusiness>()
                  .BasedOn(typeof(IACA_TipoDisciplinaBusiness))
                  .WithService.AllInterfaces()
                  .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_TipoNivelEnsinoBusiness>()
                     .BasedOn(typeof(IACA_TipoNivelEnsinoBusiness))
                     .WithService.AllInterfaces()
                     .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_TipoModalidadeEnsinoBusiness>()
                  .BasedOn(typeof(IACA_TipoModalidadeEnsinoBusiness))
                  .WithService.AllInterfaces()
                  .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ESC_EscolaBusiness>()
                  .BasedOn(typeof(IESC_EscolaBusiness))
                  .WithService.AllInterfaces()
                  .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_TipoTurnoBusiness>()
                  .BasedOn(typeof(IACA_TipoTurnoBusiness))
                  .WithService.AllInterfaces()
                  .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TUR_TurmaBusiness>()
                  .BasedOn(typeof(ITUR_TurmaBusiness))
                  .WithService.AllInterfaces()
                  .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<SYS_UnidadeAdministrativaBusiness>()
                  .BasedOn(typeof(ISYS_UnidadeAdministrativaBusiness))
                  .WithService.AllInterfaces()
                  .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_AlunoBusiness>()
                  .BasedOn(typeof(IACA_AlunoBusiness))
                  .WithService.AllInterfaces()
                  .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_TipoCurriculoPeriodoBusiness>()
                  .BasedOn(typeof(IACA_TipoCurriculoPeriodoBusiness))
                  .WithService.AllInterfaces()
                  .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TUR_TurmaTipoCurriculoPeriodoBusiness>()
                  .BasedOn(typeof(ITUR_TurmaTipoCurriculoPeriodoBusiness))
                  .WithService.AllInterfaces()
                  .SetLifestyle(LifestylePerWebRequest));

            #endregion GestaoEscolar

            InstallValidators(container);
        }

        public static IStudentTestAccoplishmentBusiness GetIStudentTestAccoplishmentBusinessForHub()
        {
            var repository = new StudentTestAccoplishmentRepository();
            var testRepository = new TestRepository();
            var startSessionValidator = new StartStudentTestSessionValidator();
            var endSessionValidator = new EndStudentTestSessionValidator();
            var endTestValidator = new EndStudentTestAccoplishmentValidator();
            return new StudentTestAccoplishmentBusiness(repository, testRepository, startSessionValidator, endSessionValidator, endTestValidator);
        }

        private void InstallValidators(IWindsorContainer container)
        {
            container.Register(Classes.FromAssemblyContaining<StartStudentTestSessionValidator>()
                                .BasedOn(typeof(IValidator<StartStudentTestSessionDto>))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<EndStudentTestSessionValidator>()
                                .BasedOn(typeof(IValidator<EndStudentTestSessionDto>))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<EndStudentTestAccoplishmentValidator>()
                                .BasedOn(typeof(IValidator<EndStudentTestAccoplishmentDto>))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));
        }
    }
}