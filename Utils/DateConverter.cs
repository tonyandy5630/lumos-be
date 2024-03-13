using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class DateConverter
    {
        public static DateTime GetUTCTime()
        {
            DateTime utcNow = DateTime.UtcNow;

            // Specify the time zone (UTC+7 in this case)
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Convert UTC time to the desired time zone
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZoneInfo);
            return localTime;
        }
    }
}
