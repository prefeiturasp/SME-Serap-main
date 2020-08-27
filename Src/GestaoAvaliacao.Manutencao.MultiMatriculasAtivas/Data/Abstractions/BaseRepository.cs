using MSTech.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.Abstractions
{
    internal abstract class BaseRepository
    {
        public IDbConnection Connection
        {
            get
            {
                TalkDBTransactionCollection collection = new TalkDBTransactionCollection();
                return new SqlConnection(collection["GestaoAvaliacao"].GetConnection.ConnectionString);
            }
        }
    }
}