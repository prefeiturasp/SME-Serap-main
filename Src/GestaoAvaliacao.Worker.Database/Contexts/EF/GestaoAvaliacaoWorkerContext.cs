using GestaoAvaliacao.Worker.Domain.Entities.Parameters;
using GestaoAvaliacao.Worker.Domain.Entities.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GestaoAvaliacao.Worker.Database.Contexts.EF
{
    public class GestaoAvaliacaoWorkerContext : DbContext, IGestaoAvaliacaoWorkerContext
    {
        private readonly IConfiguration _configuration;
        private const int MaxRetryConnectionCount = 5;

        public GestaoAvaliacaoWorkerContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<StudentTestSentEntityWorker> StudentTestsSent { get; internal set; }
        public DbSet<ParameterEntityWorker> Parameters { get; internal set; }
        public DbSet<TestSectionStatusCorrectionEntityWorker> TestsSectionStatusCorrection { get; internal set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString(nameof(GestaoAvaliacaoWorkerContext));
            optionsBuilder.UseSqlServer(connectionString, x =>
            {
                x.EnableRetryOnFailure(MaxRetryConnectionCount);
            });

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IGestaoAvaliacaoWorkerContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}