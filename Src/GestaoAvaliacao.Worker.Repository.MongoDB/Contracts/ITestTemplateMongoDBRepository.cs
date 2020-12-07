using GestaoAvaliacao.MongoEntities;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Contracts
{
    public interface ITestTemplateMongoDBRepository
    {
        Task<TestTemplate> FindOneAsync(TestTemplate entity, CancellationToken cancellationToken);
        Task<TestTemplate> ReplaceAsync(TestTemplate entity, CancellationToken cancellationToken);
    }
}