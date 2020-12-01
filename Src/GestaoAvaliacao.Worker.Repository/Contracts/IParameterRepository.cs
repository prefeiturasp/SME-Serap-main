using GestaoAvaliacao.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Contracts
{
    public interface IParameterRepository
    {
        Task<Parameter> GetAsync(string key, CancellationToken cancellationToken);
    }
}