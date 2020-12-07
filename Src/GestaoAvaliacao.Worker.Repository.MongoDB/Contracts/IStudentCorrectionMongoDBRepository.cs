using GestaoAvaliacao.MongoEntities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Contracts
{
    public interface IStudentCorrectionMongoDBRepository
    {
        Task<StudentCorrection> GetEntityAsync(StudentCorrection entity, CancellationToken cancellationToken);
        Task<IEnumerable<StudentCorrection>> GetClassCorrectionsAsync(long testId, long turId, CancellationToken cancellationToken);
        Task InsertOrReplaceAsync(StudentCorrection entity, CancellationToken cancellationToken);
    }
}