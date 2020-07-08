using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ImportacaoDeQuestionariosSME.Data
{
    public class DapperContext : IDapperContext
    {
        private IDbConnection _dbConnection;
        public const int DefaultTimeout = 90;

        public async Task ExecuteAsync(string query, object parametros = null, int? timeout = DefaultTimeout)
            => await GetSqlConnection().ExecuteAsync(query, parametros, commandTimeout: timeout);

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object parametros = null, int? timeout = DefaultTimeout)
            => await GetSqlConnection().QueryAsync<T>(query, parametros, commandTimeout: timeout);

        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string query, Func<TFirst, TSecond, TThird, TReturn> map,
            object parametros = null, string splitOn = "Id", int? commandTimeout = DefaultTimeout)
            => await GetSqlConnection().QueryAsync(query, map, parametros, splitOn: splitOn, commandTimeout: commandTimeout);

        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string query, Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
            object parametros = null, string splitOn = "Id", int? commandTimeout = DefaultTimeout)
            => await GetSqlConnection().QueryAsync(query, map, parametros, splitOn: splitOn, commandTimeout: commandTimeout);

        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string query, Func<TFirst, TSecond, TReturn> map,
            object parametros = null, string splitOn = "Id", int? commandTimeout = DefaultTimeout)
            => await GetSqlConnection().QueryAsync(query, map, parametros, splitOn: splitOn, commandTimeout: commandTimeout);

        public async Task<T> QuerySingleOrDefaultAsync<T>(string query, object parametros = null, int? timeout = DefaultTimeout)
            => await GetSqlConnection().QuerySingleOrDefaultAsync<T>(query, parametros, commandTimeout: timeout);

        public async Task<bool> ColumnExistsAsync(string table, string column)
        {
            var query = $@"SELECT TOP 1 1
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE table_name = '{table}'
                        AND column_name = '{column}'";
            var result = await GetSqlConnection().QueryAsync<int>(query);
            return result?.Any() ?? false;
        }

        private IDbConnection GetSqlConnection()
        {
            _dbConnection = _dbConnection ?? new SqlConnection(ConfigurationManager.ConnectionStrings["ProvaSPContext"].ConnectionString);
            if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();

            return _dbConnection;
        }
    }
}