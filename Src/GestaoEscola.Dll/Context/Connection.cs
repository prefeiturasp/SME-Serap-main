using MSTech.Data.Common;

namespace GestaoEscolar.Repository.Context
{
    public static class Connection
    {
        public static string GetConnectionString(string connectionName)
        {
            TalkDBTransactionCollection collection = new TalkDBTransactionCollection();
            return collection[connectionName].GetConnection.ConnectionString;
        }
    }
}
