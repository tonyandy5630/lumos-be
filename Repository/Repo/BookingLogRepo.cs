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
    public class BookingLogRepo : IBookingLogRepo
    {
        public BookingLogRepo(LumosDBContext context ) { }

        public Task<bool> CreateBookingLogAsync(BookingLog bookingLog) => BookingLogDAO.Instance.CreateBookingLogAsync(bookingLog);

        public Task<BookingLog> GetLatestBookingLogAsync(int bookingId) => BookingLogDAO.Instance.GetLatestBookingLogAsync(bookingId);

        public Task<List<BookingDTO>> GetIncomingBookingsByCustomerIdAsync(int customerId) => BookingLogDAO.Instance.GetIncomingBookingsByCustomerIdAsync(customerId);
        public Task<List<BookingDTO>> GetBookingsByCustomerIdAsync(string email) => BookingLogDAO.Instance.GetBookingsByCustomerIdAsync(email);
        public Task<List<BookingDTO>> GetIncomingBookingsByEmailAsync(string email) => BookingLogDAO.Instance.GetIncomingBookingsByEmailAsync(email);

        public Task<bool> UpdateBookingLogStatusForCustomerAsync(int bookingLogId, int newStatus) => BookingLogDAO.Instance.UpdateBookingLogStatusForCustomerAsync(bookingLogId, newStatus);

        public Task<bool> UpdateBookingLogStatusForPartnerAsync(int bookingLogId, int newStatus) => BookingLogDAO.Instance.UpdateBookingLogStatusForPartnerAsync(bookingLogId, newStatus);

    }
}
