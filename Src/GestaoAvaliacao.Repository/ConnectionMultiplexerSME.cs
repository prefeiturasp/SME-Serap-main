using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.LogFacade;
using StackExchange.Redis;
using System;

namespace GestaoAvaliacao.Repository
{
    public class ConnectionMultiplexerSME : IConnectionMultiplexerSME
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;

        public ConnectionMultiplexerSME(string host)
        {
            try
            {
                this.connectionMultiplexer = ConnectionMultiplexer
                    .Connect(string.Concat(host, $",ConnectTimeout={TimeSpan.FromSeconds(1).TotalMilliseconds}"));
            }
            catch (RedisConnectionException rcex)
            {
                LogFacade.LogFacade.SaveError(rcex, $"Erro de conexão com o servidor Redis.");
            }
            catch (Exception ex)
            {
                LogFacade.LogFacade.SaveError(ex);
            }
        }

        public IDatabase GetDatabase()
        {
            if (connectionMultiplexer == null || !connectionMultiplexer.IsConnected)
                return null;

            return connectionMultiplexer.GetDatabase();
        }
    }
}
