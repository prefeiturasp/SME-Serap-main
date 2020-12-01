using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Worker.Database.Contexts.EF;
using GestaoAvaliacao.Worker.Repository.Base;
using GestaoAvaliacao.Worker.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Tests
{
    public class TestSectionStatusCorrectionRepository : BaseWorkerRepository<TestSectionStatusCorrection>, ITestSectionStatusCorrectionRepository
    {
        public TestSectionStatusCorrectionRepository(IGestaoAvaliacaoWorkerContext gestaoAvaliacaoWorkerContext)
            : base(gestaoAvaliacaoWorkerContext)
        {
        }

        protected override DbSet<TestSectionStatusCorrection> DbSet => _gestaoAvaliacaoWorkerContext.TestsSectionStatusCorrection;

        public Task<TestSectionStatusCorrection> GetFirstOrDefaultAsync(long testId, long turId, CancellationToken cancellationToken)
            => GetFirstOrDefaultAsync(x => x.Test_Id == testId && x.tur_id == turId, cancellationToken);
    }
}