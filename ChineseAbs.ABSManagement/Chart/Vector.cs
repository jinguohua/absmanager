using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Chart
{
    public class Vector : List<double>
    {
        public Vector(double x)
        {
            this.Add(x);
        }

        public Vector(double x, double y)
        {
            this.Add(x);
            this.Add(y);
        }

        public Vector(DateTime x, double y)
        {
            this.Add((double)DateUtils.GetTicks(x));
            this.Add(y);
        }

        public Vector(DateTime x, decimal y)
        {
            this.Add((double)DateUtils.GetTicks(x));
            this.Add((double)y);
        }
    }
}
