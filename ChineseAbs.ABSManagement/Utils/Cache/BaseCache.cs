using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Utils.Cache
{
    public abstract class BaseCache<TKey, TValue>
    {
        public List<TValue> ToList()
        {
            var list = new List<TValue>();
            foreach (var keyValue in m_dict)
            {
                list.Add(keyValue.Value);
            }
            return list;
        }

        public int Count
        {
            get
            {
                return m_dict.Count;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue val;
                if (m_dict.TryGetValue(key, out val))
                {
                    return val;
                }

                return Load(key);
            }
        }

        protected abstract TValue Load(TKey key);

        protected Dictionary<TKey, TValue> m_dict = new Dictionary<TKey, TValue>();
    }
}
