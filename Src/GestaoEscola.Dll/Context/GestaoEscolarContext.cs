using GestaoEscolar.Entities;
using GestaoEscolar.Repository.Map;
using GestaoEscolar.Repository.Migrations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GestaoEscolar.Repository.Context
{
    public class GestaoEscolarContext : DbContext
	{
		public GestaoEscolarContext()
			: base(Connection.GetConnectionString("GestaoEscolar"))
		{
			this.Configuration.ProxyCreationEnabled = false;
			this.Configuration.LazyLoadingEnabled = false;
		}

		public GestaoEscolarContext(string connectionString) : base(connectionString) { }

		static GestaoEscolarContext()
		{
#if DEBUG
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<GestaoEscolarContext, Configuration>());
#endif
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
			modelBuilder.Conventions.Remove<ForeignKeyIndexConvention>();
			modelBuilder.Conventions.Remove<StoreGeneratedIdentityKeyConvention>();

			modelBuilder.Configurations.Add(new ACA_AlunoMap());
			modelBuilder.Configurations.Add(new ACA_CalendarioAnualMap());
			modelBuilder.Configurations.Add(new ACA_CurriculoMap());
			modelBuilder.Configurations.Add(new ACA_CurriculoDisciplinaMap());
			modelBuilder.Configurations.Add(new ACA_CurriculoPeriodoMap());
			modelBuilder.Configurations.Add(new ACA_CursoMap());
			modelBuilder.Configurations.Add(new ACA_DocenteMap());
			modelBuilder.Configurations.Add(new ACA_TipoCurriculoPeriodoMap());
			modelBuilder.Configurations.Add(new ACA_TipoDisciplinaMap());
			modelBuilder.Configurations.Add(new ACA_TipoModalidadeEnsinoMap());
			modelBuilder.Configurations.Add(new ACA_TipoNivelEnsinoMap());
			modelBuilder.Configurations.Add(new ACA_TipoTurnoMap());
			modelBuilder.Configurations.Add(new ESC_EscolaMap());
			modelBuilder.Configurations.Add(new MTR_MatriculaTurmaMap());
			modelBuilder.Configurations.Add(new MTR_MatriculaTurmaDisciplinaMap());
			modelBuilder.Configurations.Add(new SYS_UnidadeAdministrativaMap());
			modelBuilder.Configurations.Add(new TUR_TurmaMap());
			modelBuilder.Configurations.Add(new TUR_TurmaCurriculoMap());
			modelBuilder.Configurations.Add(new TUR_TurmaDisciplinaMap());
			modelBuilder.Configurations.Add(new TUR_TurmaDocenteMap());
			modelBuilder.Configurations.Add(new TUR_TurmaTipoCurriculoPeriodoMap());
		}

		public DbSet<ACA_Aluno> ACA_Aluno { get; set; }
		public DbSet<ACA_CalendarioAnual> ACA_CalendarioAnual { get; set; }
		public DbSet<ACA_Curriculo> ACA_Curriculo { get; set; }
		public DbSet<ACA_CurriculoDisciplina> ACA_CurriculoDisciplina { get; set; }
		public DbSet<ACA_CurriculoPeriodo> ACA_CurriculoPeriodo { get; set; }
		public DbSet<ACA_Curso> ACA_Curso { get; set; }
		public DbSet<ACA_Docente> ACA_Docente { get; set; }
		public DbSet<ACA_TipoCurriculoPeriodo> ACA_TipoCurriculoPeriodo { get; set; }
		public DbSet<ACA_TipoDisciplina> ACA_TipoDisciplina { get; set; }
		public DbSet<ACA_TipoModalidadeEnsino> ACA_TipoModalidadeEnsino { get; set; }
		public DbSet<ACA_TipoNivelEnsino> ACA_TipoNivelEnsino { get; set; }
		public DbSet<ACA_TipoTurno> ACA_TipoTurno { get; set; }
		public DbSet<ESC_Escola> ESC_Escola { get; set; }
		public DbSet<MTR_MatriculaTurma> MTR_MatriculaTurma { get; set; }
		public DbSet<MTR_MatriculaTurmaDisciplina> MTR_MatriculaTurmaDisciplina { get; set; }
		public DbSet<SYS_UnidadeAdministrativa> SYS_UnidadeAdministrativa { get; set; }
		public DbSet<TUR_Turma> TUR_Turma { get; set; }
		public DbSet<TUR_TurmaCurriculo> TUR_TurmaCurriculo { get; set; }
		public DbSet<TUR_TurmaDisciplina> TUR_TurmaDisciplina { get; set; }
		public DbSet<TUR_TurmaDocente> TUR_TurmaDocente { get; set; }
		public DbSet<TUR_TurmaTipoCurriculoPeriodo> TUR_TurmaTipoCurriculoPeriodo { get; set; }
	}
}
