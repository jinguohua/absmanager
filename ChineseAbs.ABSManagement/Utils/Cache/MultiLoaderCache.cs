using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Utils.Cache
{
    public class MultiLoaderCache<TKey, TValue> : BaseCache<TKey, TValue>, ICache<TKey, TValue>
    {
        public MultiLoaderCache(Func<TKey, IEnumerable<TValue>> loader, Func<TValue, TKey> keySelector)
        {
            m_loader = loader;
            m_keySelector = keySelector;
        }

        protected override TValue Load(TKey key)
        {
            if (m_loader != null && m_keySelector != null)
            {
                var items = m_loader(key);
                foreach (var item in items)
                {
                    m_dict[m_keySelector(item)] = item;
                }
            }

            return m_dict[key];
        }

        Func<TKey, IEnumerable<TValue>> m_loader;
        Func<TValue, TKey> m_keySelector;
    }
}
