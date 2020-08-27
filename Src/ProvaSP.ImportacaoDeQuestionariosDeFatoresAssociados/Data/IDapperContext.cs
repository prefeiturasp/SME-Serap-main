using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data
{
    public interface IDapperContext
    {
        Task ExecuteAsync(string query, object parametros = null, int? timeout = DapperContext.DefaultTimeout);

        Task<IEnumerable<T>> QueryAsync<T>(string query, object parametros = null, int? timeout = DapperContext.DefaultTimeout);

        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string query, Func<TFirst, TSecond, TReturn> map,
            object parametros = null, string splitOn = "Id", int? commandTimeout = DapperContext.DefaultTimeout);

        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string query, Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
            object parametros = null, string splitOn = "Id", int? commandTimeout = DapperContext.DefaultTimeout);

        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string query, Func<TFirst, TSecond, TThird, TReturn> map,
            object parametros = null, string splitOn = "Id", int? commandTimeout = DapperContext.DefaultTimeout);

        Task<T> QuerySingleOrDefaultAsync<T>(string query, object parametros = null, int? timeout = DapperContext.DefaultTimeout);

        Task<bool> ColumnExistsAsync(string table, string column);
    }
}