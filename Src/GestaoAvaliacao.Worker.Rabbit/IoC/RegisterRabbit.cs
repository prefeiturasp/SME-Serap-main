using GestaoAvaliacao.Worker.IoC.Contracts;
using GestaoAvaliacao.Worker.Rabbit.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GestaoAvaliacao.Worker.Rabbit.IoC
{
    public class RegisterRabbit : IIoCRegisterBootstrap
    {
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GestaoAvaliacaoRabbitSettings>(configuration.GetSection(nameof(GestaoAvaliacaoRabbitSettings)));
            services.AddSingleton<IGestaoAvaliacaoRabbitSettings>(sp => sp.GetRequiredService<IOptions<GestaoAvaliacaoRabbitSettings>>().Value);
        }
    }
}