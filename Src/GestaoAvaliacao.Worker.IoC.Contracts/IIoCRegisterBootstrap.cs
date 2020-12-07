using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoAvaliacao.Worker.IoC.Contracts
{
    public interface IIoCRegisterBootstrap
    {
        void Register(IServiceCollection services, IConfiguration configuration);
    }
}