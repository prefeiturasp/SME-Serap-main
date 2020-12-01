using GestaoAvaliacao.Worker.Database.Contexts.Dapper;
using GestaoAvaliacao.Worker.Database.Contexts.EF;
using GestaoAvaliacao.Worker.IoC.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoAvaliacao.Worker.Database.IoC
{
    public class RegisterDatabase : IIoCRegisterBootstrap
    {
        private const int MaxRetryConnectionCount = 5;

        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IGestaoAvaliacaoWorkerContext, GestaoAvaliacaoWorkerContext>(opts =>
            {
                var connectionString = configuration.GetConnectionString(nameof(GestaoAvaliacaoWorkerContext));
                opts.UseSqlServer(connectionString, x =>
                {
                    x.EnableRetryOnFailure(MaxRetryConnectionCount);
                });
            });

            services.AddTransient<IGestaoAvaliacaoWorkerDapperContext, GestaoAvaliacaoWorkerDapperContext>();
        }
    }
}