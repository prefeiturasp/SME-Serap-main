﻿using GestaoAvaliacao.Worker.Database.Contexts.EF;
using GestaoAvaliacao.Worker.Domain.Entities.Tests;
using GestaoAvaliacao.Worker.Repository.Base;
using GestaoAvaliacao.Worker.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Tests
{
    public class TestSectionStatusCorrectionRepository : BaseWorkerRepository<TestSectionStatusCorrectionEntityWorker>, ITestSectionStatusCorrectionRepository
    {
        public TestSectionStatusCorrectionRepository(IGestaoAvaliacaoWorkerContext gestaoAvaliacaoWorkerContext)
            : base(gestaoAvaliacaoWorkerContext)
        {
        }

        protected override DbSet<TestSectionStatusCorrectionEntityWorker> DbSet => _gestaoAvaliacaoWorkerContext.TestsSectionStatusCorrection;

        public Task<TestSectionStatusCorrectionEntityWorker> GetFirstOrDefaultAsync(long testId, long turId, CancellationToken cancellationToken)
            => GetFirstOrDefaultAsync(x => x.Test_Id == testId && x.tur_id == turId, cancellationToken);
    }
}