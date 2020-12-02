using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.StudentsTestSent;
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

        public DbSet<StudentTestSent> StudentTestsSent { get; internal set; }
        public DbSet<Parameter> Parameters { get; internal set; }
        public DbSet<TestSectionStatusCorrection> TestsSectionStatusCorrection { get; internal set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString(nameof(GestaoAvaliacaoWorkerContext));
            optionsBuilder.UseSqlServer(connectionString, x =>
            {
                x.EnableRetryOnFailure(MaxRetryConnectionCount);
            });

            base.OnConfiguring(optionsBuilder);
        }
    }
}