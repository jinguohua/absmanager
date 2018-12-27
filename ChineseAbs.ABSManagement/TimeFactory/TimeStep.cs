using ChineseAbs.ABSManagement.Utils;
using System;

namespace ChineseAbs.ABSManagement.TimeFactory
{
    public enum TimeUnit
    {
        Year,
        Month,
        Day,
        TradingDay,
        WorkingDay
    }

    public enum DateType
    {
        Day,
        TradingDay,
        WorkingDay
    }

    public enum PeriodType
    {
        Year,
        Month,
        Week
    }

    public static class DateTimeEx
    {
        public static DateTime AddTimeUnit(this DateTime dateTime, int interval, TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Day:
                    return dateTime.AddDays(interval);
                case TimeUnit.Month:
                    return dateTime.AddMonths(interval);
                case TimeUnit.Year:
                    return dateTime.AddYears(interval);
                case TimeUnit.WorkingDay:
                    return DateUtils.AddWorkingDay(dateTime, interval);
                case TimeUnit.TradingDay:
                    return DateUtils.AddTradingDay(dateTime, interval);
                default:
                    return dateTime;
            }
        }

        public static DateTime AddTimeStep(this DateTime dateTime, TimeStep timeStep)
        {
            return dateTime.AddTimeUnit(timeStep.Interval, timeStep.Unit);
        }
    }

    public class TimeStep
    {
        public TimeStep()
        {
        }

        public TimeStep(int interval, TimeUnit unit)
        {
            Interval = interval;
            Unit = unit;
        }

        public int Interval { get; set; }

        public TimeUnit Unit { get; set; }
    }
}
