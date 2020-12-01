using GestaoAvaliacao.Worker.Database.MongoDB.Contexts;
using GestaoAvaliacao.Worker.Database.MongoDB.Settings;
using GestaoAvaliacao.Worker.IoC.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GestaoAvaliacao.Worker.Database.MongoDB.IoC
{
    public class RegisterDatabaseMongoDB : IIoCRegisterBootstrap
    {
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GestaoAvaliacaoWorkerMongoDSettings>(configuration.GetSection(nameof(GestaoAvaliacaoWorkerMongoDSettings)));
            services.AddSingleton<IGestaoAvaliacaoWorkerMongoDSettings>(sp => sp.GetRequiredService<IOptions<GestaoAvaliacaoWorkerMongoDSettings>>().Value);
            services.AddTransient<IGestaoAvaliacaoWorkerMongoDBContext, GestaoAvaliacaoWorkerMongoDBContext>();
        }
    }
}
