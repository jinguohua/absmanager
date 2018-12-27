using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChineseAbs.ABSManagement.Foundation
{
    public static class Extension
    {
        public static string ToStringSafe(this object value)
        {
            if (value == null)
                return "";
            else
                return value.ToString();
        }

    }
}
