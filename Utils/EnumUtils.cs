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
                1 => BookingStatusEnum.WaitingForPayment,
                2 => BookingStatusEnum.Pending,
                3 => BookingStatusEnum.Doing,
                4 => BookingStatusEnum.Finished,
                5 => BookingStatusEnum.Completed,
                _ => BookingStatusEnum.Unknown,
            };
        }
    }
}
