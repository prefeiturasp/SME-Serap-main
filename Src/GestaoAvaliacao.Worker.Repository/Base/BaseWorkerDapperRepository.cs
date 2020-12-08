using GestaoAvaliacao.Worker.Database.Contexts.Dapper;

namespace GestaoAvaliacao.Worker.Repository.Base
{
    public abstract class BaseWorkerDapperRepository
    {
        protected readonly IGestaoAvaliacaoWorkerDapperContext _gestaoAvaliacaoWorkerDapperContext;

        public BaseWorkerDapperRepository(IGestaoAvaliacaoWorkerDapperContext gestaoAvaliacaoWorkerDapperContext)
        {
            _gestaoAvaliacaoWorkerDapperContext = gestaoAvaliacaoWorkerDapperContext;
        }
    }
}