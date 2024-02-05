using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.InterfaceService
{
    public interface IBookingService
    {
        Task<BookingDetail> GetBookingDetailByBookingIdAsync(int id);
        Task<List<Booking>> GetBookingsByMedicalReportIdAsync(int medicalReportId);
    }
}
