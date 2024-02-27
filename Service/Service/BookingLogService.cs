using BussinessObject;
using DataTransferObject.DTO;
using Repository.Interface.IUnitOfWork;
using Service.InterfaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class BookingLogService : IBookingLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookingLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> UpdateBookingLogStatusForPartnerAsync(int bookingLogId, int newStatus)
        {
            try
            {
                bool result = await _unitOfWork.BookingLogRepo.UpdateBookingLogStatusForPartnerAsync(bookingLogId, newStatus);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateBookingLogStatusForPartnerAsync: {ex.Message}", ex);
                return false;
            }
        }
        public async Task<bool> UpdateBookingLogStatusForCustomerAsync(int bookingLogId, int newStatus)
        {
            try
            {
                bool result = await _unitOfWork.BookingLogRepo.UpdateBookingLogStatusForCustomerAsync(bookingLogId, newStatus);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateBookingLogStatusForCustomerAsync: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<BookingLog> GetLatestBookingLogAsync(int bookingId)
        {
            try
            {
                var latestBookingLog = await _unitOfWork.BookingLogRepo.GetLatestBookingLogAsync(bookingId);
                return latestBookingLog;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLatestBookingLogAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<bool> CreateBookingLogAsync(BookingLog bookingLog)
        {
            try
            {
                bool result = await _unitOfWork.BookingLogRepo.CreateBookingLogAsync(bookingLog);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateBookingLogAsync: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<List<PendingBookingDTO>> GetPendingBookingsByEmailAsync(string email)
        {
            try
            {
                var pendingBookings = await _unitOfWork.BookingLogRepo.GetPendingBookingsByEmailAsync(email);
                return pendingBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPendingBookingsByEmailAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<PendingBookingDTO>> GetPendingBookingsByCustomerIdAsync(int customerId)
        {
            try
            {
                var pendingBookings = await _unitOfWork.BookingLogRepo.GetPendingBookingsByCustomerIdAsync(customerId);
                return pendingBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPendingBookingsByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<List<PendingBookingDTO>> GetBookingsByCustomerIdAsync(string email)
        {
            try
            {
                var pendingBookings = await _unitOfWork.BookingLogRepo.GetBookingsByCustomerIdAsync(email);
                return pendingBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<PendingBookingDTO>> GetBookingsHaveStatus1ByEmailAsync(string email)
        {
            try
            {
                var pendingBookings = await _unitOfWork.BookingLogRepo.GetBookingsHaveStatus1ByEmailAsync(email);
                return pendingBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsHaveStatus1ByEmailAsync: {ex.Message}", ex);
                throw;
            }
        }
    }
}
