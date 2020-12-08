using GestaoAvaliacao.Worker.Domain.MongoDB.Entities.Tests;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Contracts
{
    public interface IStudentCorrectionMongoDBRepository
    {
        Task<StudentCorrectionEntityWorker> GetEntityAsync(StudentCorrectionEntityWorker entity, CancellationToken cancellationToken);

        Task<IEnumerable<StudentCorrectionEntityWorker>> GetClassCorrectionsAsync(long testId, long turId, CancellationToken cancellationToken);

        Task InsertOrReplaceAsync(StudentCorrectionEntityWorker entity, CancellationToken cancellationToken);
    }
}