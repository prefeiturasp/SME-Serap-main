using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MSTech.Security.Cryptography;
using System.Configuration;

namespace AvaliaMais.FolhaRespostas.Data.MongoDB.Context
{
    public class MongoDbContext
	{
		public const string PROCESSAMENTO_COLLECTION_NAME = "ProcessamentoProva";
		public const string PROCESSAMENTO_DRE = "ProcessamentoDre";
		public const string PROCESSAMENTO_ESCOLA = "ProcessamentoEscola";
		public const string PROCESSAMENTO_TURMA = "ProcessamentoTurma";
		public const string PROCESSAMENTO_ALUNO = "ProcessamentoAluno";

		private static IMongoDatabase _database;
		private static IMongoClient _client;

		public MongoDbContext()
		{
			var cripto = new SymmetricAlgorithm(MSTech.Security.Cryptography.SymmetricAlgorithm.Tipo.TripleDES);
			if (_client == null)
			{
				var MongoDB_Connection = ConfigurationManager.AppSettings["MongoDB_Connection"];
                BsonDefaults.GuidRepresentation = GuidRepresentation.CSharpLegacy;
                _client = new MongoClient(cripto.Decrypt(MongoDB_Connection));
				ConfigurarCollections();
			}
			if (_database == null)
			{
				_database = Client.GetDatabase(ConfigurationManager.AppSettings["MongoDB_Database"]);
			}
		}

		private void ConfigurarCollections()
		{
			BsonClassMap.RegisterClassMap<DRE>(a =>
			{
				a.AutoMap();
				a.UnmapMember(b => b.Escolas);
			});

			BsonClassMap.RegisterClassMap<Escola>(a =>
			{
				a.AutoMap();
				a.UnmapMember(b => b.Turmas);
			});

			BsonClassMap.RegisterClassMap<Turma>(a =>
			{
				a.AutoMap();
				a.UnmapMember(b => b.Alunos);
			});
		}

		public static IMongoClient Client
		{
			get { return _client; }
		}

		public static IMongoDatabase DataBase
		{
			get { return _database; }
		}
		public IMongoCollection<ProcessamentoProva> ProcessamentoProva
		{
			get { return DataBase.GetCollection<ProcessamentoProva>(PROCESSAMENTO_COLLECTION_NAME); }
		}

		public IMongoCollection<DRE> ProcessamentoDRE
		{
			get { return DataBase.GetCollection<DRE>(PROCESSAMENTO_DRE); }
		}

		public IMongoCollection<Escola> ProcessamentoEscola
		{
			get { return DataBase.GetCollection<Escola>(PROCESSAMENTO_ESCOLA); }
		}

		public IMongoCollection<Turma> ProcessamentoTurma
		{
			get { return DataBase.GetCollection<Turma>(PROCESSAMENTO_TURMA); }
		}

		public IMongoCollection<Aluno> ProcessamentoAluno
		{
			get { return DataBase.GetCollection<Aluno>(PROCESSAMENTO_ALUNO); }
		}
	}
}
