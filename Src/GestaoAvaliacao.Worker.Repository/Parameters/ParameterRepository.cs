using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Worker.Database.Contexts.EF;
using GestaoAvaliacao.Worker.Repository.Base;
using GestaoAvaliacao.Worker.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Parameters
{
    public class ParameterRepository : BaseWorkerRepository<Parameter>, IParameterRepository
    {
        public ParameterRepository(IGestaoAvaliacaoWorkerContext gestaoAvaliacaoWorkerContext)
            : base(gestaoAvaliacaoWorkerContext)
        {
        }

        protected override DbSet<Parameter> DbSet => _gestaoAvaliacaoWorkerContext.Parameters;

        public Task<Parameter> GetAsync(string key, CancellationToken cancellationToken)
            => GetFirstOrDefaultAsync(x => x.Key == key, cancellationToken);
    }
}