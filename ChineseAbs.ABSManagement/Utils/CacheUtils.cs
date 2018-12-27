using System;
using System.Collections.Generic;
using ChineseAbs.ABSManagement.Utils.Cache;

namespace ChineseAbs.ABSManagement.Utils
{
    public static class CacheUtils
    {
        //构建一个缓存器，每次加载一条数据
        public static ICache<TKey, TValue> Build<TKey, TValue>(Func<TKey, TValue> loader)
        {
            return new SingleLoaderCache<TKey, TValue>(loader);
        }

        //构建一个缓存器，每次加载多条数据
        public static ICache<TKey, TValue> Build<TKey, TValue>(Func<TKey, IEnumerable<TValue>> loader, Func<TValue, TKey> keySelector)
        {
            return new MultiLoaderCache<TKey, TValue>(loader, keySelector);
        }
    }
}
