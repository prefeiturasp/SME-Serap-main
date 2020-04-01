using MSTech.Data.Common;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Caching;

namespace GestaoAvaliacao.Repository.Context
{
    public class ConnectionReadOnly
    {

        private static readonly MemoryCache _cache = new MemoryCache(typeof(ConnectionReadOnly).FullName);

        public IDbConnection Connection
        {
            get
            {
                TalkDBTransactionCollection talkDBTransaction = GetItem("talkDBTransaction") as TalkDBTransactionCollection;
                return new SqlConnection(talkDBTransaction["GestaoAvaliacao"].GetConnection.ConnectionString);
            }
        }
        
        public static object GetItem(string key)
        {
           return AddOrGetExisting(key, () => InitTalkDBTransactionCollection());
        }

        private static T AddOrGetExisting<T>(string key, Func<T> valueFactory)
        {
            var cachePolicy = new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(10)
            };

            var newValue = new Lazy<T>(valueFactory);
            var oldValue = _cache.AddOrGetExisting(key, newValue, cachePolicy) as Lazy<T>;
            try
            {
                return (oldValue ?? newValue).Value;
            }
            catch
            {
                _cache.Remove(key);
                throw;
            }
        }

        private static object InitTalkDBTransactionCollection()
        {
            return new TalkDBTransactionCollection();
        }

    }
}