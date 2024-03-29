﻿using BussinessObject;
using DataTransferObject.DTO;
using Enum;
using Microsoft.EntityFrameworkCore;
using RequestEntity;
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
        public async Task<BookingDetail> GetBookingDetailByIdAsync(int bookingId)
        {
            using var _context = new LumosDBContext();
            return await _context.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == bookingId);
        }
        public async Task<int> CountAllBookingInAppAsync()
        {
            try
            {
                using var _context = new LumosDBContext();
                return await _context.Bookings.CountAsync();
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<List<Booking>> GetAllBookingInAppAsync()
        {
            try
            {
                using var _context = new LumosDBContext();
                return await _context.Bookings.ToListAsync();
            }
            catch
            {
                throw new Exception();
            }
        }


        public async Task<List<int>> GetBookingIdsByPartnerIdAsync(int partnerId)
        {
            try
            {
                using var _context = new LumosDBContext();
                List<int> bookings = await (from sb in _context.ServiceBookings
                                            join bd in _context.BookingDetails on sb.DetailId equals bd.DetailId
                                            join mr in _context.MedicalReports on bd.ReportId equals mr.ReportId
                                            join b in _context.Bookings on bd.BookingId equals b.BookingId
                                            join ps in _context.PartnerServices on sb.ServiceId equals ps.ServiceId
                                            join p in _context.Partners on ps.PartnerId equals p.PartnerId
                                            where p.PartnerId == partnerId
                                            select b.BookingId).Distinct().ToListAsync();
                return bookings;
            }
            catch
            {
                throw new Exception("Get booking By partner id async");
            }
        }

        public async Task<Booking?> GetBookingsByDetailIdAsync(int detailId)
        {
            try
            {
                using var _context = new LumosDBContext();
                var bookings = (from bd in _context.BookingDetails
                                join b in _context.Bookings on bd.BookingId equals b.BookingId
                                where bd.DetailId == detailId
                                select b).FirstOrDefaultAsync();


                return await bookings;

            }
            catch (Exception ex)
            {
                throw new Exception();
            }

        }


        public async Task<BookingDTO?> GetLatestBookingByBookingIdAsync(int bookingId)
        {
            try
            {
                using var _context = new LumosDBContext();
                var booking = (from bl in _context.BookingLogs
                               join b in _context.Bookings on bl.BookingId equals b.BookingId
                               join pay in _context.PaymentMethods on b.PaymentId equals pay.PaymentId
                               where bl.BookingId == bookingId
                               orderby bl.CreatedDate descending
                               select new BookingDTO
                               {
                                   Address = b.Address,
                                   BookingCode = b.Code,
                                   BookingDate = b.BookingDate,
                                   BookingId = b.BookingId,
                                   bookingTime = b.bookingTime,
                                   PaymentMethod = pay.Name,
                                   Status = bl.Status,
                                   TotalPrice = b.TotalPrice,
                                   Rating = b.Rating
                               }).Take(1).FirstOrDefaultAsync();
                return await booking;
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<List<Booking>> GetBookingsByMedicalReportIdAsync(int medicalReportId)
        {
            try
            {
                using var _context = new LumosDBContext();
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
        public async Task<BookingCreationResultDTO> CreateBookingAsync(Booking booking, CreateBookingDTO createBookingDTO, string email)
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
                    booking.CreatedDate = DateConverter.GetUTCTime();
                    booking.bookingTime = createBookingDTO.bookingTime;
                    booking.From = DateConverter.GetUTCTime();
                    booking.CreatedBy = email;
                    booking.PaymentLinkId = createBookingDTO.PaymentLinkId;
                    booking.Rating = 0;
                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();

                    foreach (var cartItem in createBookingDTO.CartModel)
                    {
                        await ProcessBookingDetailAsync(booking, createBookingDTO.Note, cartItem.ReportId, email);
                        IEnumerable<ServiceDTO> services = cartItem.Services.Select(serviceId => new ServiceDTO { ServiceId = serviceId.Value });
                        await ProcessServiceBookingsAsync(services, booking, email, cartItem.ReportId);
                    }
                    booking.TotalPrice = await CalculateTotalPriceAsync(booking.BookingId);
                    await ProcessBookingLogAsync(booking, email);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    var bookingCreationResult = new BookingCreationResultDTO
                    {
                        BookingId = booking.BookingId,
                        TotalPrice = booking.TotalPrice
                    };
                    return bookingCreationResult;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in CreateBookingAsync: {ex.Message}", ex);
                    transaction.Rollback();
                    return null;
                }
            }
        }
        private async Task<int> CalculateTotalPriceAsync(int bookingId)
        {
            int totalPrice = 0;

            // Lấy danh sách tất cả các chi tiết đặt hàng cho bookingId đã cho
            var bookingDetails = await _context.BookingDetails
                .Include(bd => bd.ServiceBookings)
                .Where(bd => bd.BookingId == bookingId)
                .ToListAsync();

            // Duyệt qua từng chi tiết đặt hàng
            foreach (var bookingDetail in bookingDetails)
            {
                // Duyệt qua từng dịch vụ trong chi tiết đặt hàng
                foreach (var serviceBooking in bookingDetail.ServiceBookings)
                {
                    // Thêm giá của dịch vụ vào tổng giá
                    totalPrice += serviceBooking.Price ?? 0;
                }
            }

            return totalPrice;
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
                var paymentMethod = await _context.PaymentMethods
                                .Where(p => p.PaymentId == booking.PaymentId && p.Status == 1)
                                .FirstOrDefaultAsync();
                if (paymentMethod == null)
                {
                    throw new ArgumentException($"Payment method with ID {booking.PaymentId} not found");
                }
            }
        }

        private async Task ProcessBookingDetailAsync(Booking booking, string note, int reportId, string email)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessBookingDetailAsync: {ex.Message}", ex);
                throw;
            }
        }


        private async Task ProcessBookingLogAsync(Booking booking, string email)
        {
            var bookingLog = new BookingLog
            {
                BookingId = booking.BookingId,
                Status = (int)BookingStatusEnum.WaitingForPayment,
                CreatedDate = booking.CreatedDate,
                Note = null,
                CreatedBy = email
            };
            _context.BookingLogs.Add(bookingLog);
            await _context.SaveChangesAsync();
        }

        private async Task ProcessServiceBookingsAsync(IEnumerable<ServiceDTO> services, Booking booking, string email, int reportid)
        {
            foreach (var serviceDTO in services)
            {
                var bookingDetail = booking.BookingDetails.FirstOrDefault(bd => bd.ReportId == reportid);
                var scheckervices = await _context.PartnerServices.FirstOrDefaultAsync(s => s.ServiceId == serviceDTO.ServiceId);
                if (scheckervices == null || scheckervices.Status != 1)
                {
                    throw new Exception($"Service with Id {serviceDTO.ServiceId} is not available");
                }

                var serviceBooking = new ServiceBooking
                {
                    ServiceId = serviceDTO.ServiceId,
                    DetailId = bookingDetail.DetailId,
                    Price = (int?)scheckervices.Price,
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
            using var _context = new LumosDBContext();
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
            using var _context = new LumosDBContext();
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
            using var _context = new LumosDBContext();
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
            using var _context = new LumosDBContext();
            try
            {
                var topServices = await _context.ServiceBookings
                    .Include(sb => sb.Service)
                    .GroupBy(sb => new { sb.ServiceId, sb.Service.Name, sb.Service.PartnerId, sb.Service.Code })
                    .Select(g => new TopBookedServiceDTO
                    {
                        ServiceId = (int)g.Key.ServiceId,
                        ServiceCode = g.Key.Code,
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
            using var _context = new LumosDBContext();
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
            using var _context = new LumosDBContext();
            try
            {
                var result = await _context.Bookings
                    .Where(r => r.BookingDate.Year == year)
                    .GroupBy(b => new { Month = b.BookingDate.Month, Year = b.BookingDate.Year })
                    .Select(g => new
                    TotalBookingMonthlyStat
                    {
                        Month = g.Key.Month,
                        totalBooking = g.Count()
                    })
                    .OrderBy(r => r.Month)
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBookingsForYearAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> GetMedicalNameByReportIdAsync(int? reportId)
        {
            using var _context = new LumosDBContext();
            try
            {
                var medicalName = await _context.MedicalReports
                    .Where(mr => mr.ReportId == reportId)
                    .Select(mr => mr.Fullname)
                    .FirstOrDefaultAsync();

                return medicalName ?? "Null";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMedicalNameByReportIdAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<Customer?> GetCustomerByReportIdAsync(int? reportId)
        {
            using var _context = new LumosDBContext();
            try
            {
                var medicalReport = await _context.MedicalReports
                    .Include(mr => mr.Customer)
                    .FirstOrDefaultAsync(mr => mr.ReportId == reportId);

                return medicalReport?.Customer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCustomerByReportIdAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<List<(string ServiceName, int? Price, int Quantity)>> GetBookingServiceInfoAsync(int bookingId)
        {
            try
            {
                using var _context = new LumosDBContext();
                var bookingServiceInfo = await (
                        from bd in _context.BookingDetails
                        join sb in _context.ServiceBookings on bd.DetailId equals sb.DetailId
                        join ps in _context.PartnerServices on sb.ServiceId equals ps.ServiceId
                        where bd.BookingId == bookingId
                        group new { ps, sb } by new { ps.Name, sb.Price, sb.DetailId } into g
                        select new
                        {
                            ServiceName = g.Key.Name,
                            Price = g.Key.Price,
                            Quantity = g.Count()
                        }
                    ).ToListAsync();

                var result = bookingServiceInfo.Select(info => (info.ServiceName, info.Price, info.Quantity)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingServiceInfoAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<int?> GetTotalPriceByBookingIdAsync(int bookingId)
        {
            try
            {
                using var _context = new LumosDBContext();

                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
                return booking?.TotalPrice;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTotalPriceByBookingIdAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task UpdatePaymentLinkIdAndIsPaidAsync(int bookingid, string newPaymentLinkId)
        {
            try
            {
                using (var _context = new LumosDBContext())
                {
                    var bookingToUpdate = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingid);

                    if (bookingToUpdate != null)
                    {
                        bookingToUpdate.isPaid = true;
                        bookingToUpdate.PaymentLinkId = newPaymentLinkId;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("Booking not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating PaymentLinkId for BookingId {bookingid}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task UpdateBookingComplete(int bookingid, FeedbackRequest feedback)
        {
            try
            {
                using (var _context = new LumosDBContext())
                {
                    var bookingToUpdate = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingid);

                    if (bookingToUpdate != null)
                    {
                        bookingToUpdate.Rating = feedback.rating;
                        bookingToUpdate.FeedbackLumos = feedback.feedbackLumos;
                        bookingToUpdate.FeedbackPartner = feedback.feedbackPartner;
                        bookingToUpdate.FeedbackImage = feedback.feedbackImage;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("Booking not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating PaymentLinkId for BookingId {bookingid}: {ex.Message}", ex);
                throw;
            }
        }
    }
}
