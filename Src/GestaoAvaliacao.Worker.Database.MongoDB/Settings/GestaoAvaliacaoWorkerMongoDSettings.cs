namespace GestaoAvaliacao.Worker.Database.MongoDB.Settings
{
    public class GestaoAvaliacaoWorkerMongoDSettings : IGestaoAvaliacaoWorkerMongoDSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}