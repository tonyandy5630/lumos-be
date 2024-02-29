﻿using BussinessObject;
using DataTransferObject.DTO;
using Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DataAccessLayer
{
    public class BookingDAO
    {
        private static BookingDAO instance = null;
        private readonly LumosDBContext _context;

        public BookingDAO(LumosDBContext _context)
        {
            this._context = _context;
        }

        public static BookingDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BookingDAO(new LumosDBContext());
                }
                return instance;
            }
        }

        public async Task<Booking?> GetBookingsByDetailIdAsync(int detailId)
        {
            try
            {
                var bookings = (from bd in _context.BookingDetails
                                join b in _context.Bookings on bd.BookingId equals b.BookingId
                                where bd.DetailId == detailId
                                select b).FirstOrDefaultAsync();


                return await bookings;

            }catch(Exception ex)
            {
                throw new Exception();
            }

        }

        public async Task<IncomingBookingDTO?> GetLatestBookingByBookingIdAsync(int bookingId)
        {
            try
            {
                var booking = (from bl in _context.BookingLogs
                                      join b in _context.Bookings on bl.BookingId equals b.BookingId
                                      join pay in _context.PaymentMethods on b.PaymentId equals pay.PaymentId
                                      where bl.BookingId == bookingId
                                      orderby bl.CreatedDate descending
                                      select new IncomingBookingDTO
                                      {
                                          Address = b.Address,
                                          BookingDate = b.BookingDate,
                                          BookingId = b.BookingId,
                                          bookingTime = b.bookingTime,
                                          PaymentMethod = pay.Name,
                                          Status = EnumUtils.GetBookingEnumByStatus(bl.Status)
                                      }).Take(1).FirstOrDefaultAsync();
                return await booking;
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<BookingDetail> GetBookingDetailByBookingIdAsync(int id)
        {
            try
            {
                var bookingdetails = await _context.BookingDetails.SingleOrDefaultAsync(u => u.BookingId == id);
                return bookingdetails;
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
                var bookings = await _context.BookingDetails
                .Where(bd => bd.ReportId == medicalReportId)
                .Select(bd => bd.Booking)
                .ToListAsync();

                return bookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsByMedicalReportIdAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> CreateBookingAsync(Booking booking, CreateBookingDTO createBookingDTO, string email)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (booking == null)
                    {
                        throw new ArgumentNullException(nameof(booking), "Booking object cannot be null");
                    }

                    await ValidatePartnerScheduleAsync(createBookingDTO.PartnerId, createBookingDTO.DayOfWeek, createBookingDTO.bookingTime);

                    await ValidatePartnerStatusAsync(createBookingDTO.PartnerId);

                    await ValidateCustomerStatusAsync(email);

                    await ProcessPaymentMethodAsync(booking);

                    booking.Code = GenerateCode.GenerateTableCode("booking");
                    booking.CreatedDate = DateTime.Now;
                    booking.bookingTime = createBookingDTO.bookingTime;
                    booking.From = DateTime.Now;
                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();

                    foreach (var cartItem in createBookingDTO.CartModel)
                    {
                        await ProcessBookingDetailAsync(booking, createBookingDTO.Note, cartItem.ReportId, email);

                        await ProcessBookingLogAsync(booking, email);

                        await ProcessServiceBookingsAsync(cartItem.Services, booking, email);
                    }

                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in CreateBookingAsync: {ex.Message}", ex);
                    transaction.Rollback();
                    return false;
                }
            }
        }

        private async Task ValidatePartnerScheduleAsync(int partnerId, int dayOfWeek, int bookingTime)
        {
            var partnerSchedules = await _context.Schedules
                .Where(s => s.PartnerId == partnerId)
                .ToListAsync();

            var isScheduleMatched = partnerSchedules.Any(s =>
                s.DayOfWeek == dayOfWeek &&
                s.WorkShift == bookingTime);

            if (!isScheduleMatched)
            {
                throw new Exception("DayOfWeek or WorkShift is not available for the partner");
            }
        }

        private async Task ValidatePartnerStatusAsync(int partnerId)
        {
            var partner = await _context.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerId);
            if (partner == null || partner.Status != 1)
            {
                throw new Exception("Partner is not available");
            }
        }

        private async Task ValidateCustomerStatusAsync(string email)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null || customer.Status != 1)
            {
                throw new Exception("Customer is not available");
            }
        }

        private async Task ProcessPaymentMethodAsync(Booking booking)
        {
            if (booking.PaymentId.HasValue)
            {
                var paymentMethod = await _context.PaymentMethods.FindAsync(booking.PaymentId);
                if (paymentMethod == null)
                {
                    throw new ArgumentException($"Payment method with ID {booking.PaymentId} not found");
                }
                booking.Payment = paymentMethod;
            }
        }

        private async Task ProcessBookingDetailAsync(Booking booking, string note, int reportId, string email)
        {
            var bookingDetail = new BookingDetail
            {
                BookingId = booking.BookingId,
                Note = note,
                ReportId = reportId,
                CreatedDate = booking.CreatedDate,
                CreatedBy = email
            };
            _context.BookingDetails.Add(bookingDetail);
            await _context.SaveChangesAsync();
        }

        private async Task ProcessBookingLogAsync(Booking booking, string email)
        {
            var bookingLog = new BookingLog
            {
                BookingId = booking.BookingId,
                Status = 1, // Status mặc định khi tạo booking detail
                CreatedDate = booking.CreatedDate,
                Note = null,
                CreatedBy = email
            };
            _context.BookingLogs.Add(bookingLog);
            await _context.SaveChangesAsync();
        }

        private async Task ProcessServiceBookingsAsync(IEnumerable<ServiceDTO> services, Booking booking, string email)
        {
            foreach (var service in services)
            {
                var scheckervices = await _context.PartnerServices.FirstOrDefaultAsync(s => s.ServiceId == service.ServiceId);
                if (scheckervices == null || scheckervices.Status != 1)
                {
                    throw new Exception("Service is not available");
                }
                var serviceBooking = new ServiceBooking
                {
                    ServiceId = service.ServiceId,
                    DetailId = booking.BookingDetails.FirstOrDefault().DetailId, // You may need to adjust this based on your business logic
                    Price = (int?)service.Price,
                    Description = null,
                    CreatedDate = booking.CreatedDate,
                    LastUpdate = (DateTime)booking.CreatedDate,
                    UpdatedBy = email
                };

                _context.ServiceBookings.Add(serviceBooking);
            }
        }


        public async Task<List<Booking>> GetAllIncompleteBookingsAsync()
        {
            try
            {
                var incompleteBookingIds = await _context.BookingLogs
                   .Where(bl => bl.Status == 0)
                   .Select(bl => bl.BookingId)
                   .ToListAsync();

                var incompleteBookings = await _context.Bookings
                    .Where(b => incompleteBookingIds.Contains(b.BookingId))
                    .ToListAsync();

                return incompleteBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllIncompleteBookingsAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<Booking>> GetIncompleteBookingsByCustomerIdAsync(int customerId)
        {
            try
            {
                var incompleteBookingIds = await _context.BookingLogs
                   .Where(bl => bl.Status == 0)
                   .Select(bl => bl.BookingId)
                   .ToListAsync();

                var incompleteBookings = await _context.BookingDetails
                    .Where(bd => incompleteBookingIds.Contains(bd.BookingId))
                    .Where(bd => bd.Report != null && bd.Report.CustomerId == customerId)
                    .Select(bd => bd.Booking)
                    .ToListAsync();

                return incompleteBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetIncompleteBookingsByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<Booking>> GetIncompleteBookingsByReportIdAsync(int reportId)
        {
            try
            {
                var incompleteBookingIds = await _context.BookingLogs
                   .Where(bl => bl.Status == 0)
                   .Select(bl => bl.BookingId)
                   .ToListAsync();

                var incompleteBookings = await _context.BookingDetails
                    .Where(b => incompleteBookingIds.Contains(b.BookingId) && b.ReportId == reportId)
                    .Select(b => b.Booking)
                    .ToListAsync();

                return incompleteBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetIncompleteBookingsByReportIdAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<List<TopBookedServiceDTO>> GetTopBookedServicesAsync(int top)
        {
            try
            {
                var topServices = await _context.ServiceBookings
                    .Include(sb => sb.Service)
                    .GroupBy(sb => new { sb.ServiceId, sb.Service.Name, sb.Service.PartnerId })
                    .Select(g => new TopBookedServiceDTO
                    {
                        ServiceId = (int)g.Key.ServiceId,
                        PartnerId = (int)g.Key.PartnerId,
                        ServiceName = g.Key.Name,
                        PartnerName = g.Select(sb => sb.Service.Partner.PartnerName).FirstOrDefault(),
                        Rating = g.Select(sb => sb.Service.Rating).FirstOrDefault(),
                        NumberOfBooking = g.Count()
                    })
                    .OrderByDescending(g => g.NumberOfBooking)
                    .Take(top)
                    .ToListAsync();

                return topServices;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTopBookedServicesAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<TopBookingSummaryDTO> GetAllBookedServicesByPartnerEmailAsync(string partnerEmail)
        {
            try
            {
                var allServices = await _context.ServiceBookings
                    .Include(sb => sb.Service)
                    .Where(sb => sb.Detail != null
                                 && sb.Detail.Booking != null
                                 && sb.Service.Partner.Email == partnerEmail)
                    .GroupBy(sb => new { sb.ServiceId, sb.Service.Name, sb.Service.PartnerId })
                    .Select(g => new TopBookedServiceDTO
                    {
                        ServiceId = (int)g.Key.ServiceId,
                        PartnerId = (int)g.Key.PartnerId,
                        ServiceName = g.Key.Name,
                        PartnerName = g.Select(sb => sb.Service.Partner.PartnerName).FirstOrDefault(),
                        Rating = g.Select(sb => sb.Service.Rating).FirstOrDefault(),
                        NumberOfBooking = g.Count(),
                        Price = g.Select(sb => sb.Service.Price).FirstOrDefault()
                    })
                    .OrderByDescending(g => g.NumberOfBooking)
                    .ToListAsync();

                int totalBookings = await _context.ServiceBookings
                    .Include(sb => sb.Service)
                    .Where(sb => sb.Service.Partner.Email == partnerEmail)
                    .GroupBy(sb => sb.DetailId)
                    .Select(group => group.Select(sb => sb.ServiceId).Distinct().Count())
                    .CountAsync();


                int returnPatients = allServices.Count(s => s.NumberOfBooking > 2);

                decimal earning = await _context.ServiceBookings
                    .Include(sb => sb.Service)
                    .Where(sb => sb.Detail != null
                                 && sb.Detail.Booking != null
                                 && sb.Service.Partner.Email == partnerEmail
                                 && sb.Detail.Booking.BookingLogs.Any(bl => bl.Status == 4))
                    .SumAsync(sb => sb.Service.Price);

                int operations = await _context.ServiceBookings
                            .Include(sb => sb.Detail)
                            .Where(sb => sb.Detail != null
                                         && sb.Detail.Booking != null
                                         && sb.Service.Partner.Email == partnerEmail)
                            .GroupBy(sb => sb.Detail.BookingId)
                            .Where(g => g.OrderByDescending(sb => sb.Detail.Booking.BookingLogs.Max(bl => bl.CreatedDate)).FirstOrDefault().Detail.Booking.BookingLogs.Max(bl => bl.Status) == 2)
                            .CountAsync();

                return new TopBookingSummaryDTO
                {
                    TopBookedServices = allServices,
                    TotalBookings = totalBookings,
                    ReturnPatients = returnPatients,
                    Operations = operations,
                    Earning = earning
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBookedServicesByPartnerEmailAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<TotalBookingMonthlyStat>> GetAllBookingsForYearAsync(int year)
        {
            try
            {
                var result = await _context.Bookings
                    .Where(r => r.BookingDate.Year == year)
                    .GroupBy(b => new { Month = b.BookingDate.Month, Year = b.BookingDate.Year })
                    .Select(g => new
                    TotalBookingMonthlyStat {
                        Month = g.Key.Month,
                        totalBooking = g.Count()
                    })
                    .OrderBy(r => r.Month)
                    .ToListAsync();

                return  result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBookingsForYearAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<BookingDTO> GetBookingDetailInforByBookingIdAsync(int id)
        {
            try
            {
                var bookingDetail = await _context.BookingDetails
                    .Include(bd => bd.ServiceBookings)
                    .ThenInclude(sb => sb.Service)
                    .Include(bd => bd.ServiceBookings)
                        .ThenInclude(sb => sb.Service.ServiceDetails) 
                            .ThenInclude(sd => sd.Category) 
                    .Include(bd => bd.Booking)
                    .SingleOrDefaultAsync(u => u.BookingId == id);

                if (bookingDetail == null)
                {
                    throw new Exception($"Booking detail with ID {id} not found");
                }

                // Retrieve the latest booking log status
                var latestBookingLogStatus = await _context.BookingLogs
                    .Where(bl => bl.BookingId == id)
                    .OrderByDescending(bl => bl.CreatedDate)
                    .Select(bl => bl.Status)
                    .FirstOrDefaultAsync();

                var partnerServiceDTOs = bookingDetail.ServiceBookings.Select(serviceBooking => new PartnerServiceDTO
                {
                    ServiceId = (int)serviceBooking.ServiceId,
                    Name = serviceBooking.Service.Name,
                    Code = serviceBooking.Service.Code,
                    Duration = serviceBooking.Service.Duration,
                    Status = serviceBooking.Service.Status,
                    Description = serviceBooking.Service.Description,
                    Price = serviceBooking.Service.Price,
                    BookedQuantity = serviceBooking.Service.ServiceBookings.Count,
                    Rating = serviceBooking.Service.Rating,
                    Categories = serviceBooking.Service.ServiceDetails
                                    .Where(sd => sd.Category != null) // Ensure Category is not null
                                    .Select(sd => new ServiceCategoryDTO
                                    {
                                        CategoryId = sd.Category.CategoryId,
                                        Category = sd.Category.Category,
                                        Code = sd.Category.Code
                                    }).Distinct()
                                    .ToList()
                }).ToList();

                var bookingDTO = new BookingDTO
                {
                    bookingId = (int)bookingDetail.BookingId,
                    services = partnerServiceDTOs,
                    status = (int)latestBookingLogStatus // Set the status to the latest booking log status
                };

                return bookingDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingDetailByBookingIdAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

    }
}
