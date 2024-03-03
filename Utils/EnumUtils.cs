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
        public static BookingStatusEnum GetBookingEnumByStatus(int? status)
        {
            return status switch
            {
                0 => BookingStatusEnum.Canceled,
                1 => BookingStatusEnum.Pending,
                2 => BookingStatusEnum.Doing,
                3 => BookingStatusEnum.Finished,
                4 => BookingStatusEnum.Completed,
                _ => BookingStatusEnum.Unknown,
            };
        }
    }
}
