namespace GestaoAvaliacao.Worker.Database.MongoDB.Settings
{
    public class GestaoAvaliacaoWorkerMongoDBSettings : IGestaoAvaliacaoWorkerMongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}