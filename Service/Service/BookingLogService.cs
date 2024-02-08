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
    }
}
