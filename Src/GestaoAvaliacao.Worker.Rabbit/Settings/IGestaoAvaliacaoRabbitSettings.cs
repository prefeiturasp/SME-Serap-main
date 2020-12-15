namespace GestaoAvaliacao.Worker.Rabbit.Settings
{
    public interface IGestaoAvaliacaoRabbitSettings
    {
        string HostName { get; }
        string UserName { get; }
        string Password { get; }
        string VirtualHost { get; }
        string QueueName { get; }
        string ExchangeGestaoAvaliacao { get; }
    }
}