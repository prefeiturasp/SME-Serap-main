using MSTech.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace GestaoEscolar.Repository.Context
{
    public class ConnectionReadOnly
    {
        public IDbConnection Connection
        {
            get
            {
                TalkDBTransactionCollection collection = new TalkDBTransactionCollection();
                return new SqlConnection(collection["GestaoEscolar"].GetConnection.ConnectionString);
            }
        }
    }
}
