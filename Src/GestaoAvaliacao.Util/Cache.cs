using System;
using System.Web;

namespace GestaoAvaliacao.Util
{
    public class Cache
    {
        public static T GetFromCache<T>(string key)
        {
            if (HttpContext.Current != null && HttpContext.Current.Cache != null)
            {
                return (T)HttpContext.Current.Cache[key];
            }
            else
            {
                return default(T);
            }
        }

        public static void SetInCache(string key, object value, int hour)
        {
            if (HttpContext.Current != null && HttpContext.Current.Cache != null)
            {
                HttpContext.Current.Cache.Insert(key, value, null, DateTime.Now.AddHours(hour),
                System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
            }
        }

        public static void ClearCache(string key)
        {
            if (HttpContext.Current != null && HttpContext.Current.Cache != null)
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }
    }
}
