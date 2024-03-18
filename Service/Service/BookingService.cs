using AutoMapper;
using BussinessObject;
using DataTransferObject.DTO;
using Enum;
using Repository.Interface.IUnitOfWork;
using Service.InterfaceService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.Service
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BookingDTO>> GetPartnerBookingsByStatusAsync(string partnerEmail, int stats)
        {
            try
            {
                // validate partner
                Partner? partner = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(partnerEmail);
                if (partner == null)
                {
                    throw new NullReferenceException("Partner is ban or not existed");
                }
                BookingStatusEnum statusEnum = EnumUtils.GetBookingEnumByStatus(stats);
                if (statusEnum == BookingStatusEnum.Unknown)
                {
                    throw new NotSupportedException("Invalid status");
                }

                List<BookingDTO> pendingBookings = await _unitOfWork.BookingRepo.GetBookingByStatusIdAndPartnerId(statusEnum, partner.PartnerId);
                return pendingBookings;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BookingCreationResultDTO> CreateBookingAsync(Booking booking, CreateBookingDTO createBookingDTO, string email)
        {
            try
            {
                var partnerSchedule = await _unitOfWork.ScheduleRepo.GetSchedulesByPartnerIdAsync(createBookingDTO.PartnerId);
                var result = await _unitOfWork.BookingRepo.CreateBookingAsync(booking, createBookingDTO, email);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateBookingAsync: {ex.Message}", ex);
                return null;
            }
        }

        public async Task<BookingDTO> GetBookingDetailByBookingIdAsync(int bookingId)
        {
            try
            {
                BookingDTO? booking =  await _unitOfWork.BookingRepo.GetLatestBookingByBookingIdAsync(bookingId);
                if (booking == null)
                    throw new NullReferenceException("Cannot find booking");

                Customer? customer = await _unitOfWork.CustomerRepo.GetCustomerByBookingIdAsync(bookingId);
                if (customer == null)
                    throw new NullReferenceException("Cannot find customer");
                Partner? bookingPartner = await _unitOfWork.PartnerRepo.GetPartnerByBookingIdAsync(bookingId);

                List<MedicalServiceDTO> bookingMedServices = new();
                List<MedicalReport> bookingMedReports = (List<MedicalReport>)await _unitOfWork.MedicalReportRepo.GetMedicalReportsByBookingIdAsync(bookingId);
                foreach(var report in bookingMedReports)
                {
                    if(report == null) continue;
                    List<PartnerServiceDTO> reportBookedServices = await _unitOfWork.PartnerServiceRepo.GetServiceBookedByMedicalReportIdAndBookingId(report.ReportId, bookingId);
                    MedicalServiceDTO medicalService = new MedicalServiceDTO
                    {
                        MedicalName = report.Fullname,
                        Services = reportBookedServices,
                    };
                    bookingMedServices.Add(medicalService);
                }

                booking.MedicalServices = bookingMedServices;
                if(bookingPartner != null)
                    booking.Partner = bookingPartner.DisplayName;
                booking.Customer = customer;
                return booking;
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
        public async Task<List<Booking>> GetIncompleteBookingsByCustomerIdAsync(int customerId)
        {
            try
            {
                return await _unitOfWork.BookingRepo.GetIncompleteBookingsByCustomerIdAsync(customerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetIncompleteBookingsByCustomerIdAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Booking>> GetIncompleteBookingsByReportIdAsync(int reportId)
        {
            try
            {
                return await _unitOfWork.BookingRepo.GetIncompleteBookingsByReportIdAsync(reportId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetIncompleteBookingsByReportIdAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Booking>> GetAllIncompleteBookingsAsync()
        {
            try
            {
                return await _unitOfWork.BookingRepo.GetAllIncompleteBookingsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllIncompleteBookingsAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<TopBookedServiceDTO>> GetTopBookedServicesAsync(int top)
        {
            try
            {
                var topServices = await _unitOfWork.BookingRepo.GetTopBookedServicesAsync(top);

                return topServices;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTopBookedServicesAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<TopBookingSummaryDTO> GetAllBookedServicesByPartnerEmailAsync(string email)
        {
            try
            {
                var topServices = await _unitOfWork.BookingRepo.GetAllBookedServicesByPartnerEmailAsync(email);

                return topServices;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBookedServicesAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<int?>> GetAllBookingsForYearAsync(int year)
        {
            try
            {
                List<TotalBookingMonthlyStat> topServices = await _unitOfWork.BookingRepo.GetAllBookingsForYearAsync(year);
                List<int?> bookingsStat = new List<int?>();

                // topServices does not have any stats return 12 0s
                if (topServices == null)
                {
                    bookingsStat = (List<int?>)Enumerable.Repeat(0, 12);
                    return bookingsStat;
                }

                for (int month = 1; month <= 12; month++)
                {

                    int? bookingMonthInDB = topServices?.FirstOrDefault(t => t.Month == month)?.totalBooking;
                    int? totalBooking = 0;
                    if (bookingMonthInDB != null)
                    {
                        totalBooking = bookingMonthInDB;
                    }
                    bookingsStat.Add(totalBooking);
                }


                return bookingsStat;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBookingsForYearAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
