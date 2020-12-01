using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.StudentsTestSent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Database.Contexts.EF
{
    public interface IGestaoAvaliacaoWorkerContext
    {
        DbSet<StudentTestSent> StudentTestsSent { get; }
        DbSet<Parameter> Parameters { get; }
        DbSet<TestSectionStatusCorrection> TestsSectionStatusCorrection { get; }

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}