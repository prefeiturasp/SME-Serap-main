using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Database.Contexts.Dapper
{
    public class GestaoAvaliacaoWorkerDapperContext : IGestaoAvaliacaoWorkerDapperContext
    {
        private readonly IConfiguration _config;
        private IDbConnection _dbConnection;

        public GestaoAvaliacaoWorkerDapperContext(IConfiguration config)
        {
            _config = config;
        }

        public async Task<T> QuerySingleAsync<T>(string query, object parametros = null, int? timeout = null)
            => await GetSqlConnection().QuerySingleAsync<T>(query, parametros, commandTimeout: timeout);

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object parametros = null, int? timeout = null)
            => await GetSqlConnection().QueryAsync<T>(query, parametros, commandTimeout: timeout);

        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string query, Func<TFirst, TSecond, TReturn> map,
            object parametros = null, string splitOn = "Id", int? commandTimeout = null)
            => await GetSqlConnection().QueryAsync(query, map, parametros, splitOn: splitOn, commandTimeout: commandTimeout);

        public async Task<T> QuerySingleOrDefaultAsync<T>(string query, object parametros = null, int? timeout = null)
            => await GetSqlConnection().QuerySingleOrDefaultAsync<T>(query, parametros, commandTimeout: timeout);

        private IDbConnection GetSqlConnection()
        {
            _dbConnection ??= new SqlConnection(_config.GetConnectionString("GestaoAvaliacaoWorkerContext"));
            if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();

            return _dbConnection;
        }
    }
}