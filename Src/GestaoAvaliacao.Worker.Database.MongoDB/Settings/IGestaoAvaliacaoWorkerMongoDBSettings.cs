namespace GestaoAvaliacao.Worker.Database.MongoDB.Settings
{
    public interface IGestaoAvaliacaoWorkerMongoDBSettings
    {
        public string ConnectionString { get; }
        public string Database { get; }
        public short ConnectTimeoutInMinutes { get; set; }
    }
}