using System;

namespace ChineseAbs.ABSManagement.Utils.Cache
{
    public class SingleLoaderCache<TKey, TValue> : BaseCache<TKey, TValue>, ICache<TKey, TValue>
    {
        public SingleLoaderCache(Func<TKey, TValue> loader)
        {
            m_loader = loader;
        }

        protected override TValue Load(TKey key)
        {
            TValue val;
            if (m_loader != null)
            {
                val = m_loader(key);
                m_dict[key] = val;
            }

            return m_dict[key];
        }

        Func<TKey, TValue> m_loader;
    }
}
