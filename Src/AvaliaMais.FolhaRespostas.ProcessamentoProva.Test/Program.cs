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
using System;

namespace AvaliaMais.FolhaRespostas.ProcessamentoProva.Test
{
    public static class Program
	{
        static void Main()
        {
            throw new NotSupportedException();
        }
        }

	internal static class Bootstrap
	{
		public static void Start()
		{
            Container Container = new Container();

			Container.Register<IProcessamentoProvaRepository, ProcessamentoProvaRepository>(Lifestyle.Transient);
			Container.Register<IProcessamentoProvaService, ProcessamentoProvaService>(Lifestyle.Transient);
			Container.Register<IProcessamentoInicialRepository, ProcessamentoInicialRepository>(Lifestyle.Transient);
			Container.Register<IProcessamentoInicialService, ProcessamentoInicialService>(Lifestyle.Transient);
			Container.Register<IProcessamentoAppService, ProcessamentoAppService>(Lifestyle.Transient);
			Container.Register<MongoDbContext>(Lifestyle.Transient);

			Container.Verify();
		}
	}
}
