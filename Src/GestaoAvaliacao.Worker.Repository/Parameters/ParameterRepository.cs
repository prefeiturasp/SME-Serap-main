using GestaoAvaliacao.Worker.Database.Contexts.EF;
using GestaoAvaliacao.Worker.Domain.Entities.Parameters;
using GestaoAvaliacao.Worker.Repository.Base;
using GestaoAvaliacao.Worker.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Parameters
{
    public class ParameterRepository : BaseWorkerRepository<ParameterEntityWorker>, IParameterRepository
    {
        public ParameterRepository(IGestaoAvaliacaoWorkerContext gestaoAvaliacaoWorkerContext)
            : base(gestaoAvaliacaoWorkerContext)
        {
        }

        protected override DbSet<ParameterEntityWorker> DbSet => _gestaoAvaliacaoWorkerContext.Parameters;

        public Task<ParameterEntityWorker> GetAsync(string key, CancellationToken cancellationToken)
            => GetFirstOrDefaultAsync(x => x.Key == key, cancellationToken);
    }
}