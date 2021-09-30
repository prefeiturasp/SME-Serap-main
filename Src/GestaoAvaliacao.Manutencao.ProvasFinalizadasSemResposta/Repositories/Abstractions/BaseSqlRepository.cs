using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.Abstractions
{
    internal abstract class BaseSqlRepository
    {
        protected IDbConnection Connection
        {
            get
            {
                var conn = ConfigurationManager.AppSettings["Sql_Connection"];
                return new SqlConnection(conn);
            }
        }
    }
}