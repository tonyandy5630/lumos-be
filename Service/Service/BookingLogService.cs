using BussinessObject;
using DataTransferObject.DTO;
using Repository.Interface.IUnitOfWork;
using Service.InterfaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;
using Utils;

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
        public async Task<bool> DeclineBooking(int id, string email)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var latestBookingLog = await GetLatestBookingLogAsync(id);

                if (latestBookingLog.Status < 0 || latestBookingLog.Status > 4)
                {
                    response.message = "The status of the latest booking log is invalid or not allowed.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                }

                if (latestBookingLog.Status != 1)
                {
                    response.message = "The status of the latest booking log is not Pending. Cannot decline.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                }

                BookingLog newBookingLog = new BookingLog
                {
                    BookingId = id,
                    Note = latestBookingLog.Note,
                    Status = 0,
                    CreatedDate = DateTime.Now,
                    CreatedBy = email
                };
                bool result = await _unitOfWork.BookingLogRepo.CreateBookingLogAsync(newBookingLog);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateBookingLogAsync: {ex.Message}", ex);
                return false;
            }
        }
        public async Task<bool> AcceptBooking(int id, string email)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var latestBookingLog = await GetLatestBookingLogAsync(id);

                if (latestBookingLog.Status < 0 || latestBookingLog.Status > 4)
                {
                    response.message = "The status of the latest booking log is invalid or not allowed.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                }

                if (latestBookingLog.Status == 4)
                {
                    response.message = "The status of the latest booking log is already Completed. Cannot update.";
                    response.StatusCode = ApiStatusCode.BadRequest;

                }
                if (latestBookingLog.Status == 0)
                {
                    response.message = "The status of the latest booking log is  Cancel. Cannot update.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                }

                BookingLog newBookingLog = new BookingLog
                {
                    BookingId = id,
                    Note = latestBookingLog.Note,
                    Status = 0,
                    CreatedDate = DateTime.Now,
                    CreatedBy = email
                };
                bool result = await _unitOfWork.BookingLogRepo.CreateBookingLogAsync(newBookingLog);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateBookingLogAsync: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<List<BookingDTO>> GetIncomingBookingsByEmailAsync(string email)
        {
            try
            {
                var pendingBookings = await _unitOfWork.BookingLogRepo.GetIncomingBookingsByEmailAsync(email);
                return pendingBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPendingBookingsByEmailAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<BookingDTO>> GetIncomingBookingsByCustomerIdAsync(int customerId)
        {
            try
            {
                var pendingBookings = await _unitOfWork.BookingLogRepo.GetIncomingBookingsByCustomerIdAsync(customerId);
                return pendingBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetIncomingBookingsByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<List<BookingDTO>> GetBookingsByCustomerIdAsync(string email)
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

       
    }
}
