using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoAvaliacao.Worker.IoC
{
    public interface IIoCStartup
    {
        void Register(IServiceCollection services, IConfiguration configuration);
    }
}