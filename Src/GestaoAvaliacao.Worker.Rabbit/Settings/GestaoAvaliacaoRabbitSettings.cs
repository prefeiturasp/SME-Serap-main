namespace GestaoAvaliacao.Worker.Rabbit.Settings
{
    public class GestaoAvaliacaoRabbitSettings : IGestaoAvaliacaoRabbitSettings
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string QueueName { get; set; }
        public string ExchangeGestaoAvaliacao { get; set; }
    }
}