using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.IRepository.StudentsTestSent;
using GestaoAvaliacao.MongoRepository;
using GestaoAvaliacao.Repository;
using GestaoAvaliacao.Repository.StudentsTestSent;
using GestaoAvaliacao.Repository.StudentTestAccoplishments;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository;
using System.Configuration;

namespace GestaoAvaliacao.MappingDependence
{
    public class RepositoriesInstaller : IWindsorInstaller
    {
        public RepositoriesInstaller()
        {
            this.LifestylePerWebRequest = true;
        }

        public bool LifestylePerWebRequest { get; set; }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyContaining<AbsenceReasonRepository>()
                                .BasedOn(typeof(IAbsenceReasonRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AlternativeRepository>()
                                .BasedOn(typeof(IAlternativeRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<BaseTextRepository>()
                                .BasedOn(typeof(IBaseTextRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<CorrelatedSkillRepository>()
                                .BasedOn(typeof(ICorrelatedSkillRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<EvaluationMatrixCourseRepository>()
                             .BasedOn(typeof(IEvaluationMatrixCourseRepository))
                             .WithService.AllInterfaces()
                             .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<DisciplineRepository>()
                              .BasedOn(typeof(IDisciplineRepository))
                              .WithService.AllInterfaces()
                              .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<EvaluationMatrixCourseCurriculumGradeRepository>()
                               .BasedOn(typeof(IEvaluationMatrixCourseCurriculumGradeRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<EvaluationMatrixRepository>()
                                .BasedOn(typeof(IEvaluationMatrixRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemRepository>()
                                .BasedOn(typeof(IItemRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemLevelRepository>()
                                .BasedOn(typeof(IItemLevelRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemSituationRepository>()
                                .BasedOn(typeof(IItemSituationRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemSkillRepository>()
                                .BasedOn(typeof(IItemSkillRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemTypeRepository>()
                                .BasedOn(typeof(IItemTypeRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ModelEvaluationMatrixRepository>()
                                .BasedOn(typeof(IModelEvaluationMatrixRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ModelSkillLevelRepository>()
                                .BasedOn(typeof(IModelSkillLevelRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ParameterRepository>()
                              .BasedOn(typeof(IParameterRepository))
                              .WithService.AllInterfaces()
                              .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<SkillRepository>()
                                .BasedOn(typeof(ISkillRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTypeRepository>()
                                .BasedOn(typeof(ITestTypeRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTimeRepository>()
                                .BasedOn(typeof(ITestTimeRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTypeDeficiencyRepository>()
                                .BasedOn(typeof(ITestTypeDeficiencyRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestFilesRepository>()
                                .BasedOn(typeof(ITestFilesRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTypeCourseCurriculumGradeRepository>()
                                .BasedOn(typeof(ITestTypeCourseCurriculumGradeRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTypeItemLevelRepository>()
                             .BasedOn(typeof(ITestTypeItemLevelRepository))
                             .WithService.AllInterfaces()
                             .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<FormatTypeRepository>()
                            .BasedOn(typeof(IFormatTypeRepository))
                            .WithService.AllInterfaces()
                           .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<FileRepository>()
                           .BasedOn(typeof(IFileRepository))
                           .WithService.AllInterfaces()
                           .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemCurriculumGradeRepository>()
                         .BasedOn(typeof(IItemCurriculumGradeRepository))
                         .WithService.AllInterfaces()
                         .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<PerformanceLevelRepository>()
                         .BasedOn(typeof(IPerformanceLevelRepository))
                         .WithService.AllInterfaces()
                         .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<StudentTestAccoplishmentRepository>()
                                .BasedOn(typeof(IStudentTestAccoplishmentRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<StudentTestSentRepository>()
                                .BasedOn(typeof(IStudentTestSentRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestRepository>()
                                .BasedOn(typeof(ITestRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestContextRepository>()
                                .BasedOn(typeof(ITestContextRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<BlockRepository>()
                                .BasedOn(typeof(IBlockRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestPerformanceLevelRepository>()
                                .BasedOn(typeof(ITestPerformanceLevelRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestCurriculumGradeRepository>()
                                .BasedOn(typeof(ITestCurriculumGradeRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestPermissionRepository>()
                                .BasedOn(typeof(ITestPermissionRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<BookletRepository>()
                                .BasedOn(typeof(IBookletRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<CognitiveCompetenceRepository>()
                                            .BasedOn(typeof(ICognitiveCompetenceRepository))
                                            .WithService.AllInterfaces()
                                            .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ModelTestRepository>()
                                            .BasedOn(typeof(IModelTestRepository))
                                            .WithService.AllInterfaces()
                                            .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTypeCourseRepository>()
                               .BasedOn(typeof(ITestTypeCourseRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AdherenceRepository>()
                               .BasedOn(typeof(IAdherenceRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<CorrectionRepository>()
                               .BasedOn(typeof(ICorrectionRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<StudentTestAbsenceReasonRepository>()
                               .BasedOn(typeof(IStudentTestAbsenceReasonRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestSectionStatusCorrectionRepository>()
                               .BasedOn(typeof(ITestSectionStatusCorrectionRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AnswerSheetBatchRepository>()
                               .BasedOn(typeof(IAnswerSheetBatchRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AnswerSheetBatchFilesRepository>()
                               .BasedOn(typeof(IAnswerSheetBatchFilesRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AnswerSheetBatchLogRepository>()
                               .BasedOn(typeof(IAnswerSheetBatchLogRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AnswerSheetLotRepository>()
                               .BasedOn(typeof(IAnswerSheetLotRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));
            container.Register(Classes.FromAssemblyContaining<RequestRevokeRepository>()
                               .BasedOn(typeof(IRequestRevokeRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ExportAnalysisRepository>()
                               .BasedOn(typeof(IExportAnalysisRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));
            container.Register(Classes.FromAssemblyContaining<PerformanceItemRepository>()
                   .BasedOn(typeof(IPerformanceItemRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AnswerSheetBatchQueueRepository>()
                   .BasedOn(typeof(IAnswerSheetBatchQueueRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ServiceAnswerSheetInfoRepository>()
                    .BasedOn(typeof(IServiceAnswerSheetInfoRepository))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ReportTestPerformanceRepository>()
                   .BasedOn(typeof(IReportTestPerformanceRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ReportItemPerformanceRepository>()
                   .BasedOn(typeof(IReportItemPerformanceRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestGroupRepository>()
                                .BasedOn(typeof(ITestGroupRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestSubGroupRepository>()
                                .BasedOn(typeof(ITestSubGroupRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<KnowledgeAreaRepository>()
                                .BasedOn(typeof(IKnowledgeAreaRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<KnowledgeAreaDisciplineRepository>()
                                .BasedOn(typeof(IKnowledgeAreaDisciplineRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<SubjectRepository>()
                                .BasedOn(typeof(ISubjectRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<SubSubjectRepository>()
                                .BasedOn(typeof(ISubSubjectRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ResponseChangeLogMongoRepository>()
                                .BasedOn(typeof(IResponseChangeLogMongoRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ResponseChangeLogRepository>()
                               .BasedOn(typeof(IResponseChangeLogRepository))
                               .WithService.AllInterfaces()
                               .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<PageConfigurationRepository>()
                                .BasedOn(typeof(IPageConfigurationRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<AdministrativeUnitTypeRepository>()
                                .BasedOn(typeof(IAdministrativeUnitTypeRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemFileRepository>()
                                .BasedOn(typeof(IItemFileRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ItemAudioRepository>()
                                .BasedOn(typeof(IItemAudioRepository))
                                .WithService.AllInterfaces()
                                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<NumberItemsAplicationTaiRepository>()
            .BasedOn(typeof(INumberItemsAplicationTaiRepository))
            .WithService.AllInterfaces()
            .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<NumberItemTestTaiRepository>()
                    .BasedOn(typeof(INumberItemTestTaiRepository))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTaiCurriculumGradeRepository>()
                .BasedOn(typeof(ITestTaiCurriculumGradeRepository))
                .WithService.AllInterfaces()
                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ResultadoPspRepository>()
                .BasedOn(typeof(IResultadoPspRepository))
                .WithService.AllInterfaces()
                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TipoResultadoPspRepository>()
                .BasedOn(typeof(ITipoResultadoPspRepository))
                .WithService.AllInterfaces()
                .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<BlockChainRepository>()
                .BasedOn(typeof(IBlockChainRepository))
                .WithService.AllInterfaces()
                .SetLifestyle(LifestylePerWebRequest));

            #region GestaoEscolar

            container.Register(Classes.FromAssemblyContaining<ACA_CursoRepository>()
                  .BasedOn(typeof(IACA_CursoRepository))
                  .WithService.AllInterfaces()
                  .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_CurriculoPeriodoRepository>()
            .BasedOn(typeof(IACA_CurriculoPeriodoRepository))
            .WithService.AllInterfaces()
            .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_TipoDisciplinaRepository>()
                   .BasedOn(typeof(IACA_TipoDisciplinaRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_TipoNivelEnsinoRepository>()
                    .BasedOn(typeof(IACA_TipoNivelEnsinoRepository))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_TipoModalidadeEnsinoRepository>()
                   .BasedOn(typeof(IACA_TipoModalidadeEnsinoRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ESC_EscolaRepository>()
                   .BasedOn(typeof(IESC_EscolaRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_TipoTurnoRepository>()
                   .BasedOn(typeof(IACA_TipoTurnoRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TUR_TurmaRepository>()
                   .BasedOn(typeof(ITUR_TurmaRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<SYS_UnidadeAdministrativaRepository>()
                   .BasedOn(typeof(ISYS_UnidadeAdministrativaRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_AlunoRepository>()
                   .BasedOn(typeof(IACA_AlunoRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<ACA_TipoCurriculoPeriodoRepository>()
                   .BasedOn(typeof(IACA_TipoCurriculoPeriodoRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TUR_TurmaTipoCurriculoPeriodoRepository>()
                  .BasedOn(typeof(ITUR_TurmaTipoCurriculoPeriodoRepository))
                  .WithService.AllInterfaces()
                  .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<StudentCorrectionRepository>()
                   .BasedOn(typeof(IStudentCorrectionRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TestTemplateRepository>()
                   .BasedOn(typeof(ITestTemplateRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));
            container.Register(Classes.FromAssemblyContaining<SectionTestStatsRepository>()
                   .BasedOn(typeof(ISectionTestStatsRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));
            container.Register(Classes.FromAssemblyContaining<CorrectionResultsRepository>()
                   .BasedOn(typeof(ICorrectionResultsRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));
            container.Register(Classes.FromAssemblyContaining<SectionTestGenerateLotRepository>()
                   .BasedOn(typeof(ISectionTestGenerateLotRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Classes.FromAssemblyContaining<TempCorrectionResultRepository>()
                   .BasedOn(typeof(ITempCorrectionResultRepository))
                   .WithService.AllInterfaces()
                   .SetLifestyle(LifestylePerWebRequest));

            container.Register(Component.For<IConnectionMultiplexerSME>()
                .ImplementedBy<ConnectionMultiplexerSME>()
                .DependsOn(Dependency.OnValue("host", ConfigurationManager.AppSettings["EndPointRedis"]))
                .LifestyleSingleton());

            container.Register(Classes.FromAssemblyContaining<RepositoryCache>()
                    .BasedOn(typeof(IRepositoryCache))
                    .WithService.AllInterfaces()
                    .SetLifestyle(LifestylePerWebRequest));

            #endregion
        }
    }
}