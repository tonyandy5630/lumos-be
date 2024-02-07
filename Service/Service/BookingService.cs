using AutoMapper;
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
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateBookingAsync(Booking booking, CreateBookingDTO createBookingDTO)
        {
            try
            {
                bool result = await _unitOfWork.BookingRepo.CreateBookingAsync(booking, createBookingDTO);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateBookingAsync: {ex.Message}", ex);
                return false;
            }
        }


        public async Task<BookingDetail> GetBookingDetailByBookingIdAsync(int id)
        {
            try
            {
                return await _unitOfWork.BookingRepo.GetBookingDetailByBookingIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingDetailByBookingIdAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Booking>> GetBookingsByMedicalReportIdAsync(int medicalReportId)
        {
            try
            {
                return await _unitOfWork.BookingRepo.GetBookingsByMedicalReportIdAsync(medicalReportId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsByMedicalReportIdAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
