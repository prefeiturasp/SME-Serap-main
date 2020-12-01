using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.StudentsTestSent;
using Microsoft.EntityFrameworkCore;

namespace GestaoAvaliacao.Worker.Database.Contexts.EF
{
    public class GestaoAvaliacaoWorkerContext : DbContext, IGestaoAvaliacaoWorkerContext
    {
        public DbSet<StudentTestSent> StudentTestsSent { get; internal set; }
        public DbSet<Parameter> Parameters { get; internal set; }
        public DbSet<TestSectionStatusCorrection> TestsSectionStatusCorrection { get; internal set; }
    }
}