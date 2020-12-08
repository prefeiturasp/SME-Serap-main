using GestaoAvaliacao.Worker.Domain.MongoDB.Entities.Tests;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Contracts
{
    public interface ITestTemplateMongoDBRepository
    {
        Task<TestTemplateEntityWorker> FindOneAsync(TestTemplateEntityWorker entity, CancellationToken cancellationToken);

        Task<TestTemplateEntityWorker> ReplaceAsync(TestTemplateEntityWorker entity, CancellationToken cancellationToken);
    }
}