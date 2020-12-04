using GestaoAvaliacao.Worker.Domain.Entities.Parameters;
using GestaoAvaliacao.Worker.Domain.Entities.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Database.Contexts.EF
{
    public interface IGestaoAvaliacaoWorkerContext
    {
        DbSet<StudentTestSentEntityWorker> StudentTestsSent { get; }
        DbSet<ParameterEntityWorker> Parameters { get; }
        DbSet<TestSectionStatusCorrectionEntityWorker> TestsSectionStatusCorrection { get; }

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}