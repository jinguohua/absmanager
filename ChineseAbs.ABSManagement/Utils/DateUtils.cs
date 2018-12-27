using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChineseAbs.ABSManagement.Utils
{
    public static class DateUtils
    {
        private static readonly DateTime m_originDate = new DateTime(1970, 1, 1);
        private static readonly long m_originTicks = m_originDate.Ticks;

        public static bool IsNormalDate(DateTime dateTime)
        {
            return new DateTime(1970, 1, 1) < dateTime && dateTime < new DateTime(2300, 1, 1);
        }

        public static string DateToString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string DateToDigitString(DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }

        //是否为纯数字格式日期，如20160725
        public static bool IsDigitDate(string date)
        {
            return !string.IsNullOrWhiteSpace(date)
                && date.Length == 8 && date.All(char.IsNumber);
        }

        //解析纯数字格式日期，如20160725
        public static DateTime ParseDigitDate(string date)
        {
            CommUtils.Assert(IsDigitDate(date), "Parse date string failed [" + date + "]");
            var val = int.Parse(date);
            return new DateTime(val / 10000, val / 100 % 100, val % 100);
        }

        //是否为日期，支持2016/07/25、2016-07-25、20160725、2016-7-25、2016/7/25
        public static bool IsDate(string date)
        {
            if (date.Contains('-') || date.Contains('/'))
            {
                DateTime dt;
                return DateTime.TryParse(date, out dt);
            }

            return IsDigitDate(date);
        }

        //是否为可空日期
        //日期：2016/07/25、2016-07-25、20160725、2016-7-25、2016/7/2
        //空："-"、""、"  "
        public static bool IsNullableDate(string date)
        {
            return date == null || date.Trim() == "-"
                || string.IsNullOrWhiteSpace(date) || IsDate(date);
        }

        //解析可空日期
        //日期：2016/07/25、2016-07-25、20160725、2016-7-25、2016/7/2
        //空："-"、""、"  "
        public static DateTime? Parse(string date)
        {
            if (date == null || date.Trim() == "-" || string.IsNullOrWhiteSpace(date))
            {
                return null;
            }

            if (date.Contains('-') || date.Contains('/'))
            {
                DateTime dt;
                if (DateTime.TryParse(date, out dt))
                {
                    return dt;
                }
            }
            else if (IsDigitDate(date))
            {
                return ParseDigitDate(date);
            }

            CommUtils.Assert(false, "解析日期[{0}]失败", date);
            return null;
        }

        public static long GetTicks(DateTime dateTime)
        {
            return (dateTime.Ticks - m_originTicks) / 10000;
        }

        public static DateTime GetNextTradingDay(DateTime date)
        {
            do { date = date.AddDays(1); }
            while (!CalendarCache.IsTradingDay(date));
            return date;
        }

        public static DateTime GetPreviousTradingDay(DateTime date)
        {
            do { date = date.AddDays(-1); }
            while (!CalendarCache.IsTradingDay(date));
            return date;
        }

        public static DateTime GetNextWorkingDay(DateTime date)
        {
            do { date = date.AddDays(1); }
            while (!CalendarCache.IsWorkingDay(date));
            return date;
        }

        public static DateTime GetPreviousWorkingDay(DateTime date)
        {
            do { date = date.AddDays(-1); }
            while (!CalendarCache.IsWorkingDay(date));
            return date;
        }

        public static DateTime AddWorkingDay(DateTime date, int nWorkingDays)
        {
            if (nWorkingDays > 0)
            {
                while (nWorkingDays != 0)
                {
                    date = GetNextWorkingDay(date);
                    --nWorkingDays;
                }
            }
            else
            {
                while (nWorkingDays != 0)
                {
                    date = GetPreviousWorkingDay(date);
                    ++nWorkingDays;
                }
            }
            return date;
        }

        public static DateTime AddTradingDay(DateTime date, int nTradingDays)
        {
            if (nTradingDays > 0)
            {
                while (nTradingDays != 0)
                {
                    date = GetNextTradingDay(date);
                    --nTradingDays;
                }
            }
            else
            {
                while (nTradingDays != 0)
                {
                    date = GetPreviousTradingDay(date);
                    ++nTradingDays;
                }
            }
            return date;
        }

        public static bool CheckIsLetterFormat(string templateTimeName)
        {
            string pattern = @"^[A-Za-z]+$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(templateTimeName);
        }

        public static bool CheckIsNumberFormat(string timeSpan)
        {
            string pattern = @"^[0-9]+$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(timeSpan);
        }

        public static List<DateTime> GenerateDateList(DateTime first, DateTime endDate,
            int timeSpan, TimeSpanUnit timeSpanUnit, TemplateTimeType dateType, bool isForward, bool ignoreReduplicateDays)
        {
            if (timeSpan < 1)
            {
                throw new ApplicationException("Time span can't be 0");
            }

            var dateList = new List<DateTime>();

            DateTime date = first;
            for (int i = 0; ; ++i)
            {
                //避免每个月日期数不同导致的的向下取整问题
                //e.g.
                //(20160131).AddMonths(1).AddMonths(1) = 20160329
                //(20160131).AddMonths(2) = 20160331
                var interval = timeSpan * i;
                switch (timeSpanUnit)
                {
                    case TimeSpanUnit.Year:
                        date = first.AddYears(interval);
                        break;
                    case TimeSpanUnit.Month:
                        date = first.AddMonths(interval);
                        break;
                    case TimeSpanUnit.Day:
                        date = first.AddDays(interval);
                        break;
                }

                if (dateType == TemplateTimeType.TradingDay && !CalendarCache.IsTradingDay(date))
                {
                    date = isForward ? GetPreviousTradingDay(date) : GetNextTradingDay(date);
                }
                else if (dateType == TemplateTimeType.WorkingDay && !CalendarCache.IsWorkingDay(date))
                {
                    date = isForward ? GetPreviousWorkingDay(date) : GetNextWorkingDay(date);
                }

                if (date > endDate)
                {
                    break;
                }

                bool isIgnore = false;
                isIgnore = ignoreReduplicateDays && dateList.Contains(date);
                if (!isIgnore)
                {
                    dateList.Add(date);
                }
            }

            return dateList;
        }

        public static bool CheckTriggerDateFormat(bool isLoop, string triggerDate)
        {
            if (!isLoop)
            {
                DateTime dateTrigger;
                bool isDateTime = DateTime.TryParse(triggerDate, out dateTrigger);
                return isDateTime;
            }

            if (triggerDate == "T")
            {
                return true;
            }

            string[] items = triggerDate.Split(new char[] { '-', '+' });
            if (items.Length == 2 && items[0] == "T")
            {
                int tradeDay;
                if (items[1] == "T.NaturalDay")
                {
                    return true;
                }
                else if (int.TryParse(items[1], out tradeDay) && 0 < tradeDay && tradeDay < 366)
                {
                    return true;
                }
            }

            return false;
        }

        public static string ParseDateSyntaxKey(string text)
        {
            string key = string.Empty;

            bool isAdd = text.Contains('+');
            bool isSub = text.Contains('-');
            if (!isAdd && !isSub)
            {
                key = text.Trim();
                return key;
            }

            int temp;
            key = text.Split(new[] { '+', '-' }, StringSplitOptions.RemoveEmptyEntries).First(item => !int.TryParse(item, out temp));
            if (!string.IsNullOrEmpty(key))
            {
                key = key.Trim();
            }

            return key;
        }

        private static bool IsDateTimeOrInt(object value)
        {
            return value is DateTime || value is int;
        }

        private static DateTime AddTradingDays(this DateTime date, int nDays)
        {
            bool isAdd = nDays > 0;
            bool isSub = nDays < 0;
            var begin = date;
            var end = date;

            int nTradeDay = 0;
            do
            {
                int nDeltaDay = Math.Abs(nDays) - nTradeDay;
                if (isAdd)
                {
                    end = end.AddDays(nDeltaDay);
                }
                else if (isSub)
                {
                    begin = begin.AddDays(0-nDeltaDay);
                }

                // For example: t-1 -> (endDate - beginData).workdays = 2
                var n = CalendarCache.NumOfTradingDay(begin, end);
                if (isAdd)
                {
                    n -= CalendarCache.IsTradingDay(begin) ? 1 : 0;
                }
                else if (isSub)
                {
                    n -= CalendarCache.IsTradingDay(end) ? 1 : 0;
                }
                nTradeDay = n;
            }
            while (nTradeDay < Math.Abs(nDays));

            return nDays > 0 ? end : begin;
        }

        private static DateTime AddWorkingDays(this DateTime date, int nDays)
        {
            bool isAdd = nDays > 0;
            bool isSub = nDays < 0;
            var begin = date;
            var end = date;

            int nWorkingDay = 0;
            do
            {
                int nDeltaDay = Math.Abs(nDays) - nWorkingDay;
                if (isAdd)
                {
                    end = end.AddDays(nDeltaDay);
                }
                else if (isSub)
                {
                    begin = begin.AddDays(0 - nDeltaDay);
                }

                // For example: t-1 -> (endDate - beginData).workdays = 2
                var n = CalendarCache.NumOfWorkingDay(begin, end);
                if (isAdd)
                {
                    n -= CalendarCache.IsWorkingDay(begin) ? 1 : 0;
                }
                else if (isSub)
                {
                    n -= CalendarCache.IsWorkingDay(end) ? 1 : 0;
                }
                nWorkingDay = n;
            }
            while (nWorkingDay < Math.Abs(nDays));

            return nDays > 0 ? end : begin;
        }

        private static object Calc(object left, object right, TemplateTimeType dateType)
        {
            if (!IsDateTimeOrInt(left) || !IsDateTimeOrInt(right))
            {
                throw new ApplicationException("Calc exception, type error.");
            }

            if (left is DateTime && right is DateTime)
            {
                throw new ApplicationException("Calc exception, can't add DateTime with DateTime.");
            }
            else if (left is DateTime && right is int)
            {
                switch (dateType)
                {
                    case TemplateTimeType.TradingDay:
                        return ((DateTime)left).AddTradingDays((int)right);
                    case TemplateTimeType.WorkingDay:
                        return ((DateTime)left).AddWorkingDays((int)right);
                    case TemplateTimeType.NaturalDay:
                        return ((DateTime)left).AddDays((int)right);
                }
            }
            else if (left is int && right is DateTime)
            {
                switch (dateType)
                {
                    case TemplateTimeType.TradingDay:
                        return ((DateTime)right).AddTradingDays((int)left);
                    case TemplateTimeType.WorkingDay:
                        return ((DateTime)right).AddWorkingDays((int)left);
                    case TemplateTimeType.NaturalDay:
                        return ((DateTime)right).AddDays((int)left);
                }
            }
            else if (left is int && right is int)
            {
                return (int)left + (int)right;
            }

            return null;
        }


        /// <summary>
        /// 将 T+n、L-n格式的文本转换为时间
        /// </summary>
        /// <param name="text">原始文本</param>
        /// <param name="timeDictionary">T、L对应的时间</param>
        /// <returns>转换结果</returns>
        public static DateTime ParseDateSyntax(string text, Dictionary<string, DateTime> timeDictionary)
        {
            var errMsg = "时间识别错误[" + text + "]";

            DateTime temp;
            if (DateTime.TryParse(text, out temp))
            {
                return temp;
            }

            // 解析 T、L
            if (!text.Contains('+') && !text.Contains('-'))
            {
                var key = text.Trim();
                if (!timeDictionary.ContainsKey(key))
                {
                    throw new ApplicationException(errMsg);
                }

                return timeDictionary[key];
            }

            var items = text.Split(new[] { '+', '-' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                .ConvertAll(item => item.Trim()).ToList();

            var chars = text.ToCharArray().ToList().Where(c => c == '+' || c == '-').ToList();

            object result = null;

            for (int i = 0; i < items.Count; ++i)
            {
                object value = items[i];

                //数值
                int v;
                var isInt = int.TryParse((string)value, out v);
                if (isInt)
                {
                    value = v;
                }
                else
                {
                    //键值
                    var isKey = timeDictionary.ContainsKey(items[i]);
                    if (isKey)
                    {
                        value = timeDictionary[items[i]];
                    }
                    else
                    {
                        var content = (string)value;
                        if (content.Contains(".NaturalDay"))
                        {
                            content = content.Replace(".NaturalDay", string.Empty);
                            if (timeDictionary.ContainsKey(content))
                            {
                                value = timeDictionary[content].Day;
                            }
                        }
                    }
                }

                if (i == 0)
                {
                    result = value;
                }
                else
                {
                    bool isAdd = (chars[i - 1] == '+');

                    TemplateTimeType timeType = TemplateTimeType.NaturalDay;
                    if (isInt)
                    {
                        timeType = TemplateTimeType.TradingDay;
                    }
                    else if (value.ToString().Contains("workingDay"))
                    {
                        timeType = TemplateTimeType.WorkingDay;
                        value = int.Parse(value.ToString().Replace("workingDay", ""));
                    }
                    else if (value.ToString().Contains("day"))
                    {
                        timeType = TemplateTimeType.NaturalDay;
                        value = int.Parse(value.ToString().Replace("day", ""));
                    }

                    result = Calc(result, (isAdd ? value : 0 - (int)value), timeType);
                }
            }

            return (DateTime)result;
        }
    }
}
