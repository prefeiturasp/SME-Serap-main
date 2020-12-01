using GestaoAvaliacao.Entities.StudentsTestSent;
using GestaoAvaliacao.Worker.Database.Contexts.EF;
using GestaoAvaliacao.Worker.Repository.Base;
using GestaoAvaliacao.Worker.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Tests
{
    public class StudentTestSentRepository : BaseWorkerRepository<StudentTestSent>, IStudentTestSentRepository
    {
        public StudentTestSentRepository(IGestaoAvaliacaoWorkerContext gestaoAvaliacaoWorkerContext)
            : base(gestaoAvaliacaoWorkerContext)
        {
        }

        protected override DbSet<StudentTestSent> DbSet => _gestaoAvaliacaoWorkerContext.StudentTestsSent;

        public Task<StudentTestSent> GetFirstToProcessAsync(CancellationToken cancellationToken)
            => GetFirstOrDefaultAsync(x => x.Situation == Util.StudentTestSentSituation.Pending, cancellationToken);
    }
}