using SAFS.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core
{
    public static class CacheHelper
    {
        static System.Collections.Concurrent.ConcurrentBag<string> keys = new System.Collections.Concurrent.ConcurrentBag<string>();

        private static ICache GetCache()
        {
            return SAFS.Core.Caching.CacheManager.GetCacher("Default");
        }

        public static void Set(string key, object value)
        {
            keys.Add(key);
            GetCache().Set(key, value);
        }

        public static T Get<T>(string key, Func<T> loadFunc = null)
        {
            var v = GetCache().Get<T>(key);
            if (v == null && loadFunc != null)
            {
                lock (key)
                {
                    v = GetCache().Get<T>(key);
                    if (v == null)
                    {
                        var data = loadFunc();
                        Set(key, data);
                    }
                }
            }
            return GetCache().Get<T>(key);
        }

        public static void Remove(string key)
        {
            //keys.TryTake(out key);
            //if (!String.IsNullOrEmpty(key))
            GetCache().Remove(key);
        }
    }

    public static class CacheCenter
    {
        public static readonly string ProjectCacheKey = "ProjectCachekey";
        public static readonly string ProjctNoteCachekey = "ProjctNoteCachekey";
        public static readonly string ProjectAccountCachekey = "ProjectAccountCachekey";
        public static readonly string AccountCachekey = "AccountCachekey";
    }
}
