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
using Enum;
using RequestEntity;

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
        public async Task<bool> DeclineBooking(BookingLogDeclineRequest updateBookingStatus, string email)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var latestBookingLog = await GetLatestBookingLogAsync(updateBookingStatus.BookingId);

                if (latestBookingLog.Status < (int) BookingStatusEnum.Canceled || latestBookingLog.Status > (int)BookingStatusEnum.Completed)
                {
                    response.message = "The status of the latest booking log is invalid or not allowed.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                }

                if (latestBookingLog.Status != (int)BookingStatusEnum.Pending)
                {
                    response.message = $"The status of the latest booking log is not {((BookingStatusEnum)latestBookingLog.Status).ToString()}." +
                                       " Cannot update.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                }

                BookingLog newBookingLog = new BookingLog
                {
                    BookingId = updateBookingStatus.BookingId,
                    Note = updateBookingStatus.CancellationReason,
                    Status = (int)BookingStatusEnum.Canceled,
                    CreatedDate = DateTime.Now,
                    CreatedBy = email
                };
                bool createLogResult = await _unitOfWork.BookingLogRepo.CreateBookingLogAsync(newBookingLog);

                if (!createLogResult)
                {
                    return false;
                }

                await _unitOfWork.BookingRepo.UpdatePaymentLinkIdAsync(updateBookingStatus.BookingId, "");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeclineBooking: {ex.Message}", ex);
                return false;
            }
        }
        public async Task<bool> AcceptBooking(int id, string email)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var latestBookingLog = await GetLatestBookingLogAsync(id);

                if (latestBookingLog.Status < (int)BookingStatusEnum.Canceled || latestBookingLog.Status > (int)BookingStatusEnum.Completed)
                {
                    response.message = "The status of the latest booking log is invalid or not allowed.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                }
                if (latestBookingLog.Status == (int)BookingStatusEnum.Completed ||
                    latestBookingLog.Status == (int)BookingStatusEnum.Canceled ||
                    latestBookingLog.Status == (int)BookingStatusEnum.WaitingForPayment)
                {
                    response.message = $"The status of the latest booking log is {((BookingStatusEnum)latestBookingLog.Status).ToString()}." +
                                       " Cannot update.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                }

                BookingLog newBookingLog = new BookingLog
                {
                    BookingId = id,
                    Note = latestBookingLog.Note,
                    Status = latestBookingLog.Status + 1,
                    CreatedDate = DateTime.Now,
                    CreatedBy = email
                };
                bool result = await _unitOfWork.BookingLogRepo.CreateBookingLogAsync(newBookingLog);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AcceptBooking: {ex.Message}", ex);
                return false;
            }
        }
        public async Task<bool> ChangeStatusToPending(BookingLogAcceptRequest updateBookingStatus, string email)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var latestBookingLog = await GetLatestBookingLogAsync(updateBookingStatus.BookingId);

                if (latestBookingLog.Status < (int)BookingStatusEnum.Canceled || latestBookingLog.Status > (int)BookingStatusEnum.Completed)
                {
                    response.message = "The status of the latest booking log is invalid or not allowed.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                }

                if (latestBookingLog.Status != (int)BookingStatusEnum.WaitingForPayment)
                {
                    response.message = $"The status of the latest booking log is not {((BookingStatusEnum)latestBookingLog.Status).ToString()}." +
                                       " Cannot update.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                }
                BookingLog newBookingLog = new BookingLog
                {
                    BookingId = updateBookingStatus.BookingId,
                    Note = latestBookingLog.Note,
                    Status = (int)BookingStatusEnum.Pending,
                    CreatedDate = DateTime.Now,
                    CreatedBy = email
                };
                bool createLogResult = await _unitOfWork.BookingLogRepo.CreateBookingLogAsync(newBookingLog);

                if (!createLogResult)
                {
                    return false;
                }

                await _unitOfWork.BookingRepo.UpdatePaymentLinkIdAsync(updateBookingStatus.BookingId, updateBookingStatus.PaymentLinkId);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ChangeStatusToPending: {ex.Message}", ex);
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

        public async Task<List<BillDTO>> GetBookingsBillsByCustomerIdAsync(string email)
        {
            try
            {
                var bookingbill = await _unitOfWork.BookingLogRepo.GetBookingsBillsByCustomerIdAsync(email);
                return bookingbill;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsBillsByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }
    }
}
