using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridescent.Utils
{
    public static class DateTimeUtils
    {
        /// <summary>
        /// 计算两个时间相差的年份，结果为周年
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <returns></returns>
        public static int DiffYearWithFloor(DateTime dateTime1, DateTime dateTime2)
        {
            int year = Math.Abs(dateTime1.Year - dateTime2.Year);
            if (year != 0 && (dateTime1.Month != dateTime2.Month || dateTime1.Day != dateTime2.Day))
            {
                bool isTime1HasLaterDay = dateTime1.Month > dateTime2.Month ||
                                          (dateTime1.Month == dateTime2.Month && dateTime1.Day > dateTime2.Day);
                if (isTime1HasLaterDay)
                    year += dateTime1.Year > dateTime2.Year ? 0 : -1;
                else
                    year += dateTime1.Year > dateTime2.Year ? -1 : 0;
            }

            return year;
        }

    }
}
