using BussinessObject;
using DataAccessLayer;
using DataTransferObject.DTO;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class BookingRepo: IBookingRepo
    {
        private readonly BookingDAO _bookingDAO;

        public BookingRepo(BookingDAO bookingDAO)
        {
            _bookingDAO = bookingDAO;
        }

        public BookingRepo(LumosDBContext context) { }

        public Task<bool> CreateBookingAsync(Booking booking, CreateBookingDTO createBookingDTO, string email) => BookingDAO.Instance.CreateBookingAsync(booking, createBookingDTO,email);  

        public Task<BookingDetail> GetBookingDetailByBookingIdAsync(int id) =>BookingDAO.Instance.GetBookingDetailByBookingIdAsync(id);

        public Task<List<Booking>> GetBookingsByMedicalReportIdAsync(int medicalReportId) => BookingDAO.Instance.GetBookingsByMedicalReportIdAsync(medicalReportId);
        public Task<List<Booking>> GetIncompleteBookingsByCustomerIdAsync(int customerId) => BookingDAO.Instance.GetIncompleteBookingsByCustomerIdAsync(customerId);

        public Task<List<Booking>> GetIncompleteBookingsByReportIdAsync(int reportId) => BookingDAO.Instance.GetIncompleteBookingsByReportIdAsync((int)reportId);   

        public Task<List<Booking>> GetAllIncompleteBookingsAsync() => BookingDAO.Instance.GetAllIncompleteBookingsAsync();
        public Task<List<TopBookedServiceDTO>> GetTopBookedServicesAsync(int top) => BookingDAO.Instance.GetTopBookedServicesAsync(top);
        public Task<TopBookingSummaryDTO> GetAllBookedServicesByPartnerEmailAsync(string email) => BookingDAO.Instance.GetAllBookedServicesByPartnerEmailAsync(email);

        public Task<List<TotalBookingMonthlyStat>> GetAllBookingsForYearAsync(int year) => BookingDAO.Instance.GetAllBookingsForYearAsync(year);

        public Task<BookingDTO> GetBookingDetailInforByBookingIdAsync(int id) => BookingDAO.Instance.GetBookingDetailInforByBookingIdAsync((int)id);
    }
}
