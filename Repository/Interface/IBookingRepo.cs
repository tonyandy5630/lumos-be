using BussinessObject;
using DataTransferObject.DTO;
using Enum;
using RequestEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IBookingRepo
    {
        Task<List<Booking>> GetAllAppBookingAsync();
        Task<List<Booking>> GetBookingsByMedicalReportIdAsync(int medicalReportId);
        Task<BookingCreationResultDTO> CreateBookingAsync(Booking booking, CreateBookingDTO createBookingDTO, string email);
        Task<List<Booking>> GetIncompleteBookingsByCustomerIdAsync(int customerId);
        Task<List<Booking>> GetIncompleteBookingsByReportIdAsync(int reportId);
        Task<List<Booking>> GetAllIncompleteBookingsAsync();
        Task<List<TopBookedServiceDTO>> GetTopBookedServicesAsync(int top);
        Task<TopBookingSummaryDTO> GetAllBookedServicesByPartnerEmailAsync(string email);
        Task<List<TotalBookingMonthlyStat>> GetAllBookingsForYearAsync(int year);
        Task<Booking?> GetBookingByDetailIdAsync(int detailid);

        Task<BookingDTO?> GetLatestBookingByBookingIdAsync(int bookingId);
        Task<List<int>> GetBookingIdsByPartnerIdAsync(int partnerId);
        
        Task<List<BookingDTO>> GetBookingByStatusIdAndPartnerId(BookingStatusEnum status, int partnerId);
        Task<int> CountBookingInAppAsync();
        Task<List<(string ServiceName, int? Price, int Quantity)>> GetBookingServiceInfoAsync(int bookingId);
        Task<int?> GetTotalPriceByBookingIdAsync(int bookingId);
        Task UpdatePaymentLinkIdAndIsPaidAsync(int bookingid, string newPaymentLinkId);
        Task UpdateBookingComplete(int bookingid, FeedbackRequest feedback);
    }
}
