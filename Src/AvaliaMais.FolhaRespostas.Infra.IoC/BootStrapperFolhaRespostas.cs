using AvaliaMais.FolhaRespostas.Application;
using AvaliaMais.FolhaRespostas.Application.Interfaces;
using AvaliaMais.FolhaRespostas.Data.MongoDB.Context;
using AvaliaMais.FolhaRespostas.Data.MongoDB.Repository;
using AvaliaMais.FolhaRespostas.Data.SQLServer.Repository;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial.Interfaces;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial.Services;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva.Interfaces;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva.Services;
using SimpleInjector;

namespace AvaliaMais.FolhaRespostas.Infra.IoC
{
    public static class BootStrapperFolhaRespostas
	{
		public static void Register(Container container)
		{
			container.Register<IProcessamentoProvaRepository, ProcessamentoProvaRepository>(Lifestyle.Scoped);
			container.Register<IProcessamentoProvaService, ProcessamentoProvaService>(Lifestyle.Scoped);
			container.Register<IProcessamentoInicialRepository, ProcessamentoInicialRepository>(Lifestyle.Scoped);
			container.Register<IProcessamentoInicialService, ProcessamentoInicialService>(Lifestyle.Scoped);
			container.Register<IProcessamentoAppService, ProcessamentoAppService>(Lifestyle.Scoped);

			container.Register<MongoDbContext>(Lifestyle.Scoped);
		}
	}
}
