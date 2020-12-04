using GestaoAvaliacao.Worker.Domain.Entities.Parameters;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Contracts
{
    public interface IParameterRepository
    {
        Task<ParameterEntityWorker> GetAsync(string key, CancellationToken cancellationToken);
    }
}