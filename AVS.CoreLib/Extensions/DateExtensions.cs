using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions
{
    public static class DateExtensions
    {
        /// <summary>
        /// @usage: foreach (var day in start.EachDay(end)){..}
        /// </summary>
        public static IEnumerable<DateTime> EachDay(this DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }

        public static IEnumerable<DateTime> EachWeek(this DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(7))
                yield return day;
        }
        public static IEnumerable<DateTime> EachMonth(this DateTime from, DateTime to)
        {
            for (var month = from.Date; month.Date <= to.Date || month.Month == to.Month; month = month.AddMonths(1))
                yield return month;
        }
        /// <summary>
        /// returns 1/01/Year
        /// </summary>
        public static DateTime StartOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }
        /// <summary>
        /// returns 1st of Month/Year
        /// </summary>
        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static bool WithinRange(this DateTime value, DateTime? from, DateTime? to)
        {
            if (from.HasValue && value < from.Value)
                return false;
            if (to.HasValue && value > to.Value)
                return false;
            return true;
        }
        public static bool WithinRange(this DateTime value, DateTime from, DateTime to)
        {
            if (value < from)
                return false;
            if (value > to)
                return false;
            return true;
        }
    }
}