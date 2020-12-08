using GestaoAvaliacao.Worker.Database.MongoDB.Contexts;
using GestaoAvaliacao.Worker.Domain.MongoDB.Entities.Tests;
using GestaoAvaliacao.Worker.Repository.MongoDB.Base;
using GestaoAvaliacao.Worker.Repository.MongoDB.Contracts;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Tests
{
    public class StudentCorrectionMongoDBRepository : BaseWorkerMongoRepository<StudentCorrectionEntityWorker>, IStudentCorrectionMongoDBRepository
    {
        public StudentCorrectionMongoDBRepository(IGestaoAvaliacaoWorkerMongoDBContext gestaoAvaliacaoWorkerMongoDBContext)
            : base(gestaoAvaliacaoWorkerMongoDBContext)
        {
        }

        public Task<IEnumerable<StudentCorrectionEntityWorker>> GetClassCorrectionsAsync(long testId, long turId, CancellationToken cancellationToken)
        {
            var filter1 = Builders<StudentCorrectionEntityWorker>.Filter.Eq("Test_Id", testId);
            var filter2 = Builders<StudentCorrectionEntityWorker>.Filter.Eq("tur_id", turId);
            var filter = Builders<StudentCorrectionEntityWorker>.Filter.And(filter1, filter2);
            return FindAsync(filter, cancellationToken);
        }
    }
}