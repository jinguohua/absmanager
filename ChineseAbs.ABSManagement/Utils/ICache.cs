using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Utils
{
    public interface ICache<TKey, TValue>
    {
        int Count { get; }
        List<TValue> ToList();
        TValue this[TKey key]{ get; }
    }
}
