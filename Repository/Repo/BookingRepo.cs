using BussinessObject;
using DataAccessLayer;
using DataTransferObject.DTO;
using Enum;
using Repository.Interface;
using Repository.Interface.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class BookingRepo: IBookingRepo
    {

        public BookingRepo(LumosDBContext context)
        {
        }

        public  Task<List<Booking>> GetAllAppBookingAsync () => BookingDAO.Instance.GetAllBookingInAppAsync();
        public Task<Booking?> GetBookingByDetailIdAsync(int detailid) => BookingDAO.Instance.GetBookingsByDetailIdAsync(detailid);
        public Task<bool> CreateBookingAsync(Booking booking, CreateBookingDTO createBookingDTO, string email) => BookingDAO.Instance.CreateBookingAsync(booking, createBookingDTO,email);  

        public Task<List<Booking>> GetBookingsByMedicalReportIdAsync(int medicalReportId) => BookingDAO.Instance.GetBookingsByMedicalReportIdAsync(medicalReportId);
        public Task<List<Booking>> GetIncompleteBookingsByCustomerIdAsync(int customerId) => BookingDAO.Instance.GetIncompleteBookingsByCustomerIdAsync(customerId);

        public Task<List<Booking>> GetIncompleteBookingsByReportIdAsync(int reportId) => BookingDAO.Instance.GetIncompleteBookingsByReportIdAsync((int)reportId);   

        public Task<List<Booking>> GetAllIncompleteBookingsAsync() => BookingDAO.Instance.GetAllIncompleteBookingsAsync();
        public Task<List<TopBookedServiceDTO>> GetTopBookedServicesAsync(int top) => BookingDAO.Instance.GetTopBookedServicesAsync(top);
        public Task<TopBookingSummaryDTO> GetAllBookedServicesByPartnerEmailAsync(string email) => BookingDAO.Instance.GetAllBookedServicesByPartnerEmailAsync(email);

        public Task<List<TotalBookingMonthlyStat>> GetAllBookingsForYearAsync(int year) => BookingDAO.Instance.GetAllBookingsForYearAsync(year);

        public Task<BookingDTO?> GetLatestBookingByBookingIdAsync(int bookingId) => BookingDAO.Instance.GetLatestBookingByBookingIdAsync(bookingId);

        public Task<List<int>> GetBookingIdsByPartnerIdAsync(int partnerId) => BookingDAO.Instance.GetBookingIdsByPartnerIdAsync(partnerId);

        public async Task<List<BookingDTO>> GetBookingByStatusIdAndPartnerId(BookingStatusEnum status, int partnerId)
        {
            List<BookingDTO> bookings = new List<BookingDTO>();
            List<int> partnerBookingIds = await GetBookingIdsByPartnerIdAsync(partnerId);
            foreach (int bookingId in partnerBookingIds)
            {
                Customer? customer = await CustomerDAO.Instance.GetCustomerByBookingIdAsync(bookingId);
                if (customer == null)
                {
                    continue;
                }
                BookingDTO? latestBooking = await GetLatestBookingByBookingIdAsync(bookingId);
                if (latestBooking != null && latestBooking.Status != (int)status)
                {
                    continue;
                }
                latestBooking.Customer = customer;
                bookings.Add(latestBooking);
            }
            return bookings;
        }

        public Task<int> CountBookingInAppAsync() => BookingDAO.Instance.CountAllBookingInAppAsync();   
    }
}
