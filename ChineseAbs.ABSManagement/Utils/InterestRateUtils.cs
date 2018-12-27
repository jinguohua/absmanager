using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils
{
    public static class InterestRateUtils
    {
        /// <summary>
        /// 重置当期利率
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static Dictionary<string, double> RateResetRecords(Dictionary<string, string> variables)
        {
            var rateResetRecords = new Dictionary<string, double>();
            foreach (var keyValue in variables)
            {
                if (keyValue.Key.EndsWith(".Reset", StringComparison.CurrentCultureIgnoreCase))
                {
                    string doubleText = keyValue.Value;
                    bool isPercent = keyValue.Value.EndsWith("%");
                    if (isPercent)
                    {
                        doubleText = keyValue.Value.Replace("%", "");
                    }
                    double d;
                    if (double.TryParse(doubleText, out d))
                    {
                        if (isPercent)
                        {
                            d /= 100d;
                        }

                        rateResetRecords[keyValue.Key.Replace(".Reset", "")] = d;
                    }
                }
            }
            return rateResetRecords;
        }

        /// <summary>
        /// 计算当期浮动利率
        /// </summary>
        /// <param name="couponString"></param>
        /// <param name="rateResetRecords"></param>
        /// <returns></returns>
        public static string CalculateCurrentCouponRate(string couponString, Dictionary<string, double> rateResetRecords)
        {
            if (couponString.Any(Char.IsLetter))
            {
                var expression = new ExpressionUtils(couponString);
                rateResetRecords.Where(keyValue => couponString.Contains(keyValue.Key))
                    .ToList().ForEach(item => { expression.AddParam(item.Key, item.Value); });

                couponString = (expression.Eval<double>() * 100).ToString("n2") + "%";
            }

            //移除利率和百分号之间的空格
            couponString = couponString.Replace(" ", "");

            return couponString;
        }

        /// <summary>
        /// 计算 收益率 = （实际收益金额/投资金额）* 365 / （到期时间-开始时间）
        /// </summary>
        /// <param name="gains"></param>
        /// <param name="money"></param>
        /// <param name="endTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static double CalculateYield(double gains, double money, DateTime endTime, DateTime startTime)
        {
            var gainsDivideMoney = gains / money; 
            var dateTimeDifference = (endTime -startTime).TotalDays;
            CommUtils.Assert(dateTimeDifference != 0.0, "[开始时间]和[到期时间]相等,无法计算[实际收益率]");

            var daysOfYear = 365;
            var yield = gainsDivideMoney * daysOfYear / dateTimeDifference; 
            return yield;
        }

        /// <summary>
        /// 计算 预计收益金额 = （（预计收益金额*）*（到期时间-开始时间）/ 365） * 投资金额
        /// </summary>
        public static double CalculateGains(double yield, double money, DateTime endTime, DateTime startTime)
        {
            var dateTimeDifference = (endTime - startTime).TotalDays;
            var daysOfYear = 365;

            var gains = (yield * dateTimeDifference / daysOfYear) * money; 
            return gains;
        }
    }
}
