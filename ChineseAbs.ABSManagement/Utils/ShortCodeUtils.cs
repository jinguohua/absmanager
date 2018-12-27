using System;
using System.Text;

namespace ChineseAbs.ABSManagement.Utils
{
    public static class ShortCodeUtils
    {
        public static string Random()
        {
            var builder = new StringBuilder(m_digits);
            for (int i = 0; i < m_digits; ++i)
            {
                builder.Append(m_base36[m_random.Next(m_radix)]);
            }

            return builder.ToString();
        }

        private static Random m_random = new Random();

        private static readonly char[] m_base36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private static readonly int m_digits = 6;

        private static readonly int m_radix = 36;
    }
}
