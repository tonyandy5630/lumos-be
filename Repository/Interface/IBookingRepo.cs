using BussinessObject;
using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IBookingRepo
    {
        Task<BookingDetail> GetBookingDetailByBookingIdAsync(int id);
        Task<List<Booking>> GetBookingsByMedicalReportIdAsync(int medicalReportId);
        Task<bool> CreateBookingAsync(Booking booking, CreateBookingDTO createBookingDTO, string email);
        Task<List<Booking>> GetIncompleteBookingsByCustomerIdAsync(int customerId);
        Task<List<Booking>> GetIncompleteBookingsByReportIdAsync(int reportId);
        Task<List<Booking>> GetAllIncompleteBookingsAsync();
        Task<List<TopBookedServiceDTO>> GetTopBookedServicesAsync(int top);
        Task<TopBookingSummaryDTO> GetAllBookedServicesByPartnerEmailAsync(string email);
        Task<List<TotalBookingMonthlyStat>> GetAllBookingsForYearAsync(int year);
        Task<BookingDTO> GetBookingDetailInforByBookingIdAsync(int id);

        Task<Booking?> GetBookingByDetailIdAsync(int detailid);

        Task<IncomingBookingDTO?> GetLatestBookingByBookingIdAsync(int bookingId);

    }
}
