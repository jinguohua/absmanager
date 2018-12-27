using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils
{
    //TODO：Update cache every year
    public static class CalendarCache
    {
        public static bool IsTradingDay(DateTime date)
        {
            InitCalendar();

            return m_tradeDays.Contains(DateToInt(date));
        }

        public static bool IsWorkingDay(DateTime date)
        {
            InitCalendar();

            return m_workDays.Contains(DateToInt(date));
        }
        
        public static int NumOfWorkingDay(DateTime startDate, DateTime endDate)
        {
            InitCalendar();

            var start = DateToInt(startDate);
            var end = DateToInt(endDate);

            var sum = m_sumDays[end].Item1 - m_sumDays[start].Item1;
            if (IsWorkingDay(endDate))
            {
                sum += 1;
            }

            return sum;
        }

        public static int NumOfTradingDay(DateTime startDate, DateTime endDate)
        {
            InitCalendar();

            var start = DateToInt(startDate);
            var end = DateToInt(endDate);

            var sum = m_sumDays[end].Item2 - m_sumDays[start].Item2;
            if (IsTradingDay(endDate))
            {
                sum += 1;
            }

            return sum;
        }

        private static int DateToInt(DateTime date)
        {
            return date.Year * 10000 + date.Month * 100 + date.Day;
        }


        private static void InitCalendar()
        {
            if (!m_isCached)
            {
                var json = JsonCalendarReader.CalendarDataReader.CalendarJsonData;
                var dateList = JsonCalendarReader.CalendarDataReader.DeserializeCalendarData(json);

                var workingDayList = new int[] { 20180929, 20180930, 20190929, 20191012, 20200927, 20201010, 20210926, 20211009,
                    20220925, 20221008, 20230930, 20231008, 20240929, 20241012, 20250928, 20251011 };

                int index = 0;

                var curYear = DateTime.Now.Year;
                var begin = DateTime.Parse(dateList[0].Date);
                var end = DateTime.Parse(dateList[dateList.Count - 1].Date);
                for (var date = begin; date <= end && index < dateList.Count; date = date.AddDays(1))
                {
                    var holiday = dateList[index];
                    var curHoliday = DateToInt(DateTime.Parse(holiday.Date));

                    var curDate = DateToInt(date);
                    while (curHoliday < curDate)
                    {
                        ++index;
                        holiday = dateList[index];
                        curHoliday = DateToInt(DateTime.Parse(holiday.Date));
                    }

                    var isTradeDay = curHoliday != curDate;
                    var isWorkDay = (curHoliday != curDate) || (holiday.IsWorkDay) || ((curDate / 10000) > curYear && workingDayList.Contains(curDate));

                    m_sumDays[curDate] = new Tuple<int, int>(m_workDays.Count, m_tradeDays.Count);

                    if (isTradeDay)
                    {
                        m_tradeDays.Add(curDate);
                    }

                    if (isWorkDay)
                    {
                        m_workDays.Add(curDate);
                    }
                }

                m_isCached = true;
            }
        }

        private static bool m_isCached = false;

        private static HashSet<int> m_workDays = new HashSet<int>();
        private static HashSet<int> m_tradeDays = new HashSet<int>();

        //Sum of workdays, tradeDays before the key date.
        private static Dictionary<int, Tuple<int, int>> m_sumDays = new Dictionary<int,Tuple<int,int>>();
    }
}
