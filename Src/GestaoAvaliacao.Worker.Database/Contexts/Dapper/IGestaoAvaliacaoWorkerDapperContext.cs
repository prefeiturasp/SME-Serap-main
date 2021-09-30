using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Database.Contexts.Dapper
{
    public interface IGestaoAvaliacaoWorkerDapperContext
    {
        Task<T> QuerySingleAsync<T>(string query, object parametros = null, int? timeout = null);
        Task<IEnumerable<T>> QueryAsync<T>(string query, object parametros = null, int? timeout = null);
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string query, Func<TFirst, TSecond, TReturn> map,
            object parametros = null, string splitOn = "Id", int? commandTimeout = null);
        Task<T> QuerySingleOrDefaultAsync<T>(string query, object parametros = null, int? timeout = null);
    }
}