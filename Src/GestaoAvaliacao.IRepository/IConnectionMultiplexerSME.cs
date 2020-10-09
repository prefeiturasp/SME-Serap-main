using StackExchange.Redis;

namespace GestaoAvaliacao.IRepository
{
    public interface IConnectionMultiplexerSME
    {
        IDatabase GetDatabase();
    }
}
