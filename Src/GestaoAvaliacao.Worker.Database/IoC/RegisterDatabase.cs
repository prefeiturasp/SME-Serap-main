using GestaoAvaliacao.Worker.Database.Contexts.Dapper;
using GestaoAvaliacao.Worker.Database.Contexts.EF;
using GestaoAvaliacao.Worker.IoC.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoAvaliacao.Worker.Database.IoC
{
    public class RegisterDatabase : IIoCRegisterBootstrap
    {
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IGestaoAvaliacaoWorkerContext, GestaoAvaliacaoWorkerContext>(ServiceLifetime.Transient);
            services.AddTransient<IGestaoAvaliacaoWorkerDapperContext, GestaoAvaliacaoWorkerDapperContext>();
        }
    }
}