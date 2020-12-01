namespace GestaoAvaliacao.Worker.Database.MongoDB.Settings
{
    public interface IGestaoAvaliacaoWorkerMongoDSettings
    {
        public string ConnectionString { get;}
        public string Database { get; }
    }
}