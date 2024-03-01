using BussinessObject;
using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.InterfaceService
{
    public interface IBookingService
    {
        Task<List<Booking>> GetBookingsByMedicalReportIdAsync(int medicalReportId);
        Task<bool> CreateBookingAsync(Booking booking, CreateBookingDTO createBookingDTO, string email);
        Task<List<Booking>> GetIncompleteBookingsByCustomerIdAsync(int customerId);
        Task<List<Booking>> GetIncompleteBookingsByReportIdAsync(int reportId);
        Task<List<Booking>> GetAllIncompleteBookingsAsync();
        Task<List<TopBookedServiceDTO>> GetTopBookedServicesAsync(int top);
        Task<TopBookingSummaryDTO> GetAllBookedServicesByPartnerEmailAsync(string email);
        Task<List<int?>> GetAllBookingsForYearAsync(int year);
        Task<BookingDTO> GetBookingDetailByBookingIdAsync(int id);

        Task<List<BookingDTO>> GetPartnerPendingBookingsDTOAsync(string  partnerEmail);
    }
}
