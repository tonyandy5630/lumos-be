using Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class EnumUtils
    {
        public static string GetBookingEnumByStatus(int? status)
        {
            return status switch
            {
                0 => BookingStatusEnum.Canceled.ToString(),
                1 => BookingStatusEnum.Pending.ToString(),
                2 => BookingStatusEnum.Doing.ToString(),
                3 => BookingStatusEnum.Finished.ToString(),
                4 => BookingStatusEnum.Completed.ToString(),
                _ => "Unknown",
            };
        }
    }
}
