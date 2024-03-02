using BussinessObject;
using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.InterfaceService
{
    public interface IBookingLogService
    {
        Task<bool> UpdateBookingLogStatusForPartnerAsync(int bookingLogId, int newStatus);
        Task<bool> UpdateBookingLogStatusForCustomerAsync(int bookingLogId, int newStatus);
        Task<BookingLog> GetLatestBookingLogAsync(int bookingId);
        Task<bool> AcceptBooking(int bookingid, string email);
        Task<bool> DeclineBooking(int bookingid, string email);
        Task<List<BookingDTO>> GetIncomingBookingsByEmailAsync(string email);
        Task<List<BookingDTO>> GetIncomingBookingsByCustomerIdAsync(int customerId);
        Task<List<BookingDTO>> GetBookingsByCustomerIdAsync(string email);
    }
}
