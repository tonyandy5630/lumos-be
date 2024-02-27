using BussinessObject;
using DataTransferObject.DTO;
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
        private readonly LumosDBContext dbContext;

        public BookingDAO(LumosDBContext dbContext)
        {
            this.dbContext = dbContext;
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
        public async Task<BookingDetail> GetBookingDetailByBookingIdAsync(int id)
        {
            try
            {
                var bookingdetails = await dbContext.BookingDetails.SingleOrDefaultAsync(u => u.BookingId == id);
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
                var bookings = await dbContext.BookingDetails
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
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (booking == null)
                    {
                        throw new ArgumentNullException(nameof(booking), "Booking object cannot be null");
                    }

                    var partnerSchedules = await dbContext.Schedules
                            .Where(s => s.PartnerId == createBookingDTO.PartnerId)
                            .ToListAsync();

                    // Kiểm tra xem giá trị nhập vào có trùng với bất kỳ lịch trình nào của đối tác không
                    var isScheduleMatched = partnerSchedules.Any(s =>
                        s.DayOfWeek == createBookingDTO.DayOfWeek &&
                        s.WorkShift == createBookingDTO.WorkShift);

                    if (!isScheduleMatched)
                    {
                        throw new Exception("DayOfWeek or WorkShift is not available for the partner");
                    }

                    if (booking.PaymentId.HasValue)
                    {
                        var paymentMethod = await dbContext.PaymentMethods.FindAsync(booking.PaymentId);
                        if (paymentMethod == null)
                        {
                            throw new ArgumentException($"Payment method with ID {booking.PaymentId} not found");
                        }
                        booking.Payment = paymentMethod;
                    }

                    booking.Code = GenerateCode.GenerateTableCode("booking");
                    booking.CreatedDate = DateTime.Now;
                    booking.bookingTime = createBookingDTO.WorkShift;
                    dbContext.Bookings.Add(booking);
                    await dbContext.SaveChangesAsync();

                    foreach (var cartItem in createBookingDTO.CartModel)
                    {
                        var bookingDetail = new BookingDetail
                        {
                            BookingId = booking.BookingId,
                            Note = createBookingDTO.Note,
                            ReportId = cartItem.ReportId,
                            CreatedDate = booking.CreatedDate,
                            CreatedBy = email
                        };
                        dbContext.BookingDetails.Add(bookingDetail);
                        await dbContext.SaveChangesAsync();
                        var bookingLog = new BookingLog
                        {
                            BookingId = booking.BookingId,
                            Status = 1, // Status mặc định khi tạo booking detail
                            CreatedDate = booking.CreatedDate,
                            Note = createBookingDTO.Note,
                            CreatedBy = email
                        };
                        dbContext.BookingLogs.Add(bookingLog);
                        await dbContext.SaveChangesAsync();
                        foreach (var service in cartItem.Services)
                        {
                            var serviceBooking = new ServiceBooking
                            {
                                ServiceId = service.ServiceId,
                                DetailId = bookingDetail.DetailId,
                                Price = (int?)service.Price,
                                Description = null,
                                CreatedDate = booking.CreatedDate,
                                LastUpdate = (DateTime)booking.CreatedDate,
                                UpdatedBy = email
                            };

                            dbContext.ServiceBookings.Add(serviceBooking);

                            var categoryId = await dbContext.ServiceDetails
                            .Where(s => s.ServiceId == service.ServiceId)
                            .Select(s => s.CategoryId)
                            .FirstOrDefaultAsync();

                            if (categoryId != null)
                            {
                                var serviceDetail = new ServiceDetail
                                {
                                    ServiceId = service.ServiceId,
                                    CategoryId = categoryId,
                                    CreatedDate = (DateTime)booking.CreatedDate,
                                    LastUpdate = (DateTime)booking.CreatedDate,
                                    CreatedBy = email,
                                    UpdatedBy = email
                                };
                                dbContext.ServiceDetails.Add(serviceDetail);
                            }
                        }
                    }

                    await dbContext.SaveChangesAsync();

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

        public async Task<List<Booking>> GetAllIncompleteBookingsAsync()
        {
            try
            {
                var incompleteBookingIds = await dbContext.BookingLogs
                   .Where(bl => bl.Status == 0)
                   .Select(bl => bl.BookingId)
                   .ToListAsync();

                var incompleteBookings = await dbContext.Bookings
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
                var incompleteBookingIds = await dbContext.BookingLogs
                   .Where(bl => bl.Status == 0)
                   .Select(bl => bl.BookingId)
                   .ToListAsync();

                var incompleteBookings = await dbContext.BookingDetails
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
                var incompleteBookingIds = await dbContext.BookingLogs
                   .Where(bl => bl.Status == 0)
                   .Select(bl => bl.BookingId)
                   .ToListAsync();

                var incompleteBookings = await dbContext.BookingDetails
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
                var topServices = await dbContext.ServiceBookings
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
      
        /*public async Task<List<TopBookedServiceDTO>> GetAllTopBookedServicesAsync()
        {
            try
            {
                var topServices = await dbContext.ServiceBookings
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
                    .ToListAsync();

                return topServices;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllTopBookedServicesAsync: {ex.Message}", ex);
                throw;
            }
        }*/

       public async Task<List<TotalBookingMonthlyStat>> GetAllBookingsForYearAsync(int year)
        {
            try
            {
                var result = await dbContext.Bookings
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
    }
}
