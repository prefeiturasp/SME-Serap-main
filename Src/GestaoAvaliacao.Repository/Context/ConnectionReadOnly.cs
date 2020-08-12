using MSTech.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace GestaoAvaliacao.Repository.Context
{
    public class ConnectionReadOnly
    {
        public IDbConnection Connection
        {
            get
            {
                TalkDBTransactionCollection collection = new TalkDBTransactionCollection();
                return new SqlConnection(collection["GestaoAvaliacao"].GetConnection.ConnectionString);
            }
        }

        internal IDbConnection ConnectionCoreSSO
        {
            get
            {
                var collection = new TalkDBTransactionCollection();
                return new SqlConnection(collection["CoreSSO"].GetConnection.ConnectionString);
            }
        }
    }
}
