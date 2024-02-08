using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IBookingLogRepo
    {
        Task<bool> UpdateBookingLogStatusForPartnerAsync(int bookingLogId, int newStatus);
        Task<bool> UpdateBookingLogStatusForCustomerAsync(int bookingLogId, int newStatus);
    }
}
