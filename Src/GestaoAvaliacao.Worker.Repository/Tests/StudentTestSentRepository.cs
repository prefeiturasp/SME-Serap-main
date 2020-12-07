using GestaoAvaliacao.Util;
using GestaoAvaliacao.Worker.Database.Contexts.EF;
using GestaoAvaliacao.Worker.Domain.Entities.Tests;
using GestaoAvaliacao.Worker.Repository.Base;
using GestaoAvaliacao.Worker.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Tests
{
    public class StudentTestSentRepository : BaseWorkerRepository<StudentTestSentEntityWorker>, IStudentTestSentRepository
    {
        public StudentTestSentRepository(IGestaoAvaliacaoWorkerContext gestaoAvaliacaoWorkerContext)
            : base(gestaoAvaliacaoWorkerContext)
        {
        }

        protected override DbSet<StudentTestSentEntityWorker> DbSet => _gestaoAvaliacaoWorkerContext.StudentTestsSent;

        public Task<StudentTestSentEntityWorker> GetFirstToProcessAsync(CancellationToken cancellationToken)
            => GetFirstOrDefaultAsync(x => x.Situation == StudentTestSentSituation.Pending, cancellationToken);
    }
}