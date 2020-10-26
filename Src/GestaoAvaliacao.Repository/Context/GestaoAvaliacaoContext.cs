using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map;
using GestaoAvaliacao.Repository.Migrations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GestaoAvaliacao.Repository.Context
{
	public class GestaoAvaliacaoContext : DbContext
	{
		public GestaoAvaliacaoContext()
			: base(Connection.GetConnectionString("GestaoAvaliacao"))
		{
			this.Configuration.ProxyCreationEnabled = false;
			this.Configuration.LazyLoadingEnabled = false;
		}

		public GestaoAvaliacaoContext(string connectionString) : base(connectionString) { }

		static GestaoAvaliacaoContext()
		{
			#if DEBUG
				Database.SetInitializer(new MigrateDatabaseToLatestVersion<GestaoAvaliacaoContext, Configuration>());
			#endif
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Configurations.Add(new AbsenceReasonMap());
			modelBuilder.Configurations.Add(new AlternativeMap());
			modelBuilder.Configurations.Add(new BaseTextMap());
			modelBuilder.Configurations.Add(new CorrelatedSkillMap());
			modelBuilder.Configurations.Add(new DisciplineMap());
			modelBuilder.Configurations.Add(new EvaluationMatrixCourseMap());
			modelBuilder.Configurations.Add(new EvaluationMatrixCourseCurriculumGradeMap());
			modelBuilder.Configurations.Add(new EvaluationMatrixMap());
			modelBuilder.Configurations.Add(new ItemLevelMap());
			modelBuilder.Configurations.Add(new ItemMap());
			modelBuilder.Configurations.Add(new ItemSkillMap());
			modelBuilder.Configurations.Add(new ItemTypeMap());
			modelBuilder.Configurations.Add(new ModelEvaluationMatrixMap());
			modelBuilder.Configurations.Add(new ModelSkillLevelMap());
			modelBuilder.Configurations.Add(new ParameterMap());
			modelBuilder.Configurations.Add(new SkillMap());
			modelBuilder.Configurations.Add(new TestTypeCourseMap());
			modelBuilder.Configurations.Add(new TestTypeCourseCurriculumGradeMap());
			modelBuilder.Configurations.Add(new TestTypeItemLevelMap());
			modelBuilder.Configurations.Add(new TestTypeDeficiencyMap());
			modelBuilder.Configurations.Add(new FormatTypeMap());
			modelBuilder.Configurations.Add(new TestTypeMap());
			modelBuilder.Configurations.Add(new ItemSituationMap());
			modelBuilder.Configurations.Add(new FileMap());
			modelBuilder.Configurations.Add(new ItemCurriculumGradeMap());
			modelBuilder.Configurations.Add(new PerformanceLevelMap());
			modelBuilder.Configurations.Add(new ParameterPageMap());
			modelBuilder.Configurations.Add(new ParameterCategoryMap());
			modelBuilder.Configurations.Add(new ParameterTypeMap());
			modelBuilder.Configurations.Add(new TestMap());
			modelBuilder.Configurations.Add(new TestPerformanceLevelMap());
			modelBuilder.Configurations.Add(new TestCurriculumGradeMap());
			modelBuilder.Configurations.Add(new TestItemLevelMap());
            modelBuilder.Configurations.Add(new TestPermissionMap());
            modelBuilder.Configurations.Add(new BookletMap());
			modelBuilder.Configurations.Add(new BlockMap());
			modelBuilder.Configurations.Add(new CognitiveCompetenceMap());
			modelBuilder.Configurations.Add(new ModelTestMap());
			modelBuilder.Configurations.Add(new BlockItemMap());
			modelBuilder.Configurations.Add(new TestFilesMap());
			modelBuilder.Configurations.Add(new AdherenceMap());
			modelBuilder.Configurations.Add(new StudentTestAbsenceReasonMap());
			modelBuilder.Configurations.Add(new TestSectionStatusCorrectionMap());
			modelBuilder.Configurations.Add(new AnswerSheetBatchMap());
			modelBuilder.Configurations.Add(new AnswerSheetBatchFilesMap());
			modelBuilder.Configurations.Add(new AnswerSheetBatchLogMap());
			modelBuilder.Configurations.Add(new AnswerSheetLotMap());
			modelBuilder.Configurations.Add(new RequestRevokeMap());
			modelBuilder.Configurations.Add(new ExportAnalysisMap());
			modelBuilder.Configurations.Add(new AnswerSheetBatchQueueMap());
            modelBuilder.Configurations.Add(new TestGroupMap());
            modelBuilder.Configurations.Add(new TestSubGroupMap());
            modelBuilder.Configurations.Add(new KnowledgeAreaMap());
            modelBuilder.Configurations.Add(new KnowledgeAreaDisciplineMap());
            modelBuilder.Configurations.Add(new SubjectMap());
            modelBuilder.Configurations.Add(new SubSubjectMap());
            modelBuilder.Configurations.Add(new PageConfigurationMap());
            modelBuilder.Configurations.Add(new AdministrativeUnitTypeMap());
            modelBuilder.Configurations.Add(new ItemFileMap());
            modelBuilder.Configurations.Add(new BlockKnowledgeAreaMap());
            modelBuilder.Configurations.Add(new ItemAudioMap());

            modelBuilder.Entity<Subject>()
                .HasMany<Discipline>(s => s.Disciplines)
                .WithMany(c => c.Subjects)
                .Map(cs =>
                    {
                        cs.MapLeftKey("Subject_Id");
                        cs.MapRightKey("Discipline_Id");
                        cs.ToTable("SubjectDiscipline");
                    }
               );

            modelBuilder.Entity<Subject>()
                .HasMany<KnowledgeArea>(s => s.KnowledgeAreas)
                .WithMany(c => c.Subjects)
                .Map(cs =>
                {
                    cs.MapLeftKey("Subject_Id");
                    cs.MapRightKey("KnowledgeArea_Id");
                    cs.ToTable("SubjectKnowledgeArea");
                }
               );
        }

		public DbSet<AbsenceReason> AbsenceReason { get; set; }
		public DbSet<Alternative> Alternative { get; set; }
		public DbSet<BaseText> BaseText { get; set; }
		public DbSet<CorrelatedSkill> CorrelatedSkill { get; set; }
		public DbSet<Discipline> Discipline { get; set; }
		public DbSet<EvaluationMatrixCourse> EvaluationMatrixCourse { get; set; }
		public DbSet<EvaluationMatrixCourseCurriculumGrade> EvaluationMatrixCourseCurriculumGrade { get; set; }
		public DbSet<EvaluationMatrix> EvaluationMatrix { get; set; }
		public DbSet<ItemLevel> ItemLevel { get; set; }
		public DbSet<Item> Item { get; set; }
		public DbSet<ItemSkill> ItemSkill { get; set; }
		public DbSet<ItemType> ItemType { get; set; }
		public DbSet<ModelEvaluationMatrix> ModelEvaluationMatrix { get; set; }
		public DbSet<ModelSkillLevel> ModelSkillLevel { get; set; }
		public DbSet<Parameter> Parameter { get; set; }
		public DbSet<ParameterPage> ParameterPage { get; set; }
		public DbSet<ParameterCategory> ParameterCategory { get; set; }
		public DbSet<ParameterType> ParameterType { get; set; }
		public DbSet<Skill> Skill { get; set; }
        public DbSet<StudentTestSession> StudentTestSessions { get; set; }
        public DbSet<TestTypeCourse> TestTypeCourse { get; set; }
		public DbSet<TestTypeCourseCurriculumGrade> TestTypeCourseCurriculumGrade { get; set; }
		public DbSet<TestTypeDeficiency> TestTypeDeficiencies { get; set; }
		public DbSet<TestTypeItemLevel> TestTypeItemLevel { get; set; }
		public DbSet<FormatType> FormatType { get; set; }
		public DbSet<TestType> TestType { get; set; }
		public DbSet<ItemSituation> ItemSituation { get; set; }
		public DbSet<File> File { get; set; }
		public DbSet<ItemCurriculumGrade> ItemCurriculumGrade { get; set; }
		public DbSet<PerformanceLevel> PerformanceLevel { get; set; }
		public DbSet<Test> Test { get; set; }
		public DbSet<TestPerformanceLevel> TestPerformanceLevel { get; set; }
		public DbSet<TestCurriculumGrade> TestCurriculumGrade { get; set; }
		public DbSet<TestItemLevel> TestItemLevel { get; set; }
        public DbSet<TestPermission> TestPermission { get; set; }
        public DbSet<Booklet> Booklet { get; set; }
		public DbSet<Block> Block { get; set; }
		public DbSet<CognitiveCompetence> CognitiveCompetence { get; set; }
		public DbSet<ModelTest> ModelTest { get; set; }
		public DbSet<BlockItem> BlockItem { get; set; }
		public DbSet<TestFiles> TestFiles { get; set; }
		public DbSet<Adherence> Adherence { get; set; }
		public DbSet<StudentTestAbsenceReason> StudentTestAbsenceReason { get; set; }
		public DbSet<TestSectionStatusCorrection> TestSectionStatusCorrection { get; set; }
		public DbSet<AnswerSheetBatch> AnswerSheetBatch { get; set; }
		public DbSet<AnswerSheetBatchFiles> AnswerSheetBatchFiles { get; set; }
		public DbSet<AnswerSheetBatchLog> AnswerSheetBatchLog { get; set; }
		public DbSet<AnswerSheetLot> AnswerSheetLot { get; set; }
		public DbSet<RequestRevoke> RequestRevoke { get; set; }
		public DbSet<ExportAnalysis> ExportAnalysis { get; set; }
		public DbSet<AnswerSheetBatchQueue> AnswerSheetBatchQueue { get; set; }
        public DbSet<TestGroup> TestGroup { get; set; }
        public DbSet<TestSubGroup> TestSubGroup { get; set; }
        public DbSet<KnowledgeArea> KnowledgeArea { get; set; }
        public DbSet<KnowledgeAreaDiscipline> KnowledgeAreaDiscipline { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<SubSubject> SubSubject { get; set; }
        public DbSet<PageConfiguration> PageConfiguration { get; set; }
        public DbSet<AdministrativeUnitType> AdministrativeUnitType { get; set; }
        public DbSet<ItemFile> ItemFile { get; set; }
        public DbSet<ItemAudio> ItemAudio { get; set; }
    }
}
