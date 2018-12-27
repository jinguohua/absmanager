using System;

namespace ChineseAbs.ABSManagement.Utils
{
    public static class MathUtils
    {
        private static readonly double m_moneyPrecisionValue = 0.000001;

        /// <summary>
        /// 比较两个表示金额的double数是否相等
        /// 注：仅用于比较金额（其它用途中，该误差可能被放大）
        /// EQ = Equal
        /// </summary>
        public static bool MoneyEQ(double l, double r)
        {
            return Math.Abs(l - r) < m_moneyPrecisionValue;
        }

        //GT: >
        public static bool MoneyGT(double l, double r)
        {
            return !MoneyEQ(l, r) && l > r;
        }

        //LT: <
        public static bool MoneyLT(double l, double r)
        {
            return !MoneyEQ(l, r) && l < r;
        }

        //GTE: >=
        public static bool MoneyGTE(double l, double r)
        {
            return MoneyEQ(l, r) || l > r;
        }

        //LTE: <=
        public static bool MoneyLTE(double l, double r)
        {
            return MoneyEQ(l, r) || l < r;
        }

        //NE: !=
        public static bool MoneyNE(double l, double r)
        {
            return !MoneyEQ(l, r);
        }

        public static double ParseDouble(string content)
        {
            var isPercent = content.Contains("%");
            if (isPercent)
            {
                content = content.Replace("%", string.Empty);
            }

            double value;
            if (!double.TryParse(content, out value))
            {
                CommUtils.Assert(false, "解析浮点数失败：{0}", content);
            }

            return isPercent ? value / 100.0 : value;
        }
    }
}
