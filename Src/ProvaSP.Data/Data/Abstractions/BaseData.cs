using ProvaSP.Data.Funcionalidades;
using System.Data;
using System.Data.SqlClient;

namespace ProvaSP.Data.Data.Abstractions
{
    public abstract class BaseData
    {
        private IDbConnection _dbConnection;

        protected IDbConnection GetSqlConnection()
        {
            _dbConnection = _dbConnection ?? new SqlConnection(StringsConexao.ProvaSP);
            if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();

            return _dbConnection;
        }
    }
}