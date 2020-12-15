using GestaoAvaliacao.Worker.Database.IoC;
using GestaoAvaliacao.Worker.Database.MongoDB.IoC;
using GestaoAvaliacao.Worker.IoC.Contracts;
using GestaoAvaliacao.Worker.Rabbit.IoC;
using GestaoAvaliacao.Worker.Repository.IoC;
using GestaoAvaliacao.Worker.Repository.MongoDB.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace GestaoAvaliacao.Worker.IoC
{
    public class IoCStartup : IIoCStartup
    {
        private ICollection<IIoCRegisterBootstrap> _ioCRegisterBootstraps = new List<IIoCRegisterBootstrap>();

        public IoCStartup()
            : this(new RegisterDatabase(), new RegisterDatabaseMongoDB(), new RegisterRepository(), new RegisterMongoDBRepository(), new RegisterRabbit())
        {
        }

        public IoCStartup(params IIoCRegisterBootstrap[] ioCRegisterBootstraps)
        {
            _ioCRegisterBootstraps = ioCRegisterBootstraps.ToList();
        }

        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            if (_ioCRegisterBootstraps == null || !_ioCRegisterBootstraps.Any()) return;

            foreach (var ioCRegister in _ioCRegisterBootstraps)
                ioCRegister.Register(services, configuration);
        }
    }
}