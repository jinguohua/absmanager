
using System;

namespace ChineseAbs.ABSManagement
{
    public struct LazyConstruct<T> where T : class, new()
    {
        public T Get(Func<T> func)
        {
            if (m_instance == null)
            {
                m_instance = func();
            }

            return m_instance;
        }

        public T Get()
        {
            if (m_instance == null)
            {
                m_instance = new T();
            }

            return m_instance;
        }

        private T m_instance;
    }
}
