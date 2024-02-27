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
                        s.WorkShift == createBookingDTO.bookingTime);

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
                    booking.bookingTime = createBookingDTO.bookingTime;
                    booking.From = DateTime.Now;
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
                            Note = null,
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

        public async Task<TopBookingSummaryDTO> GetAllBookedServicesByPartnerEmailAsync(string partnerEmail)
        {
            try
            {
                var allServices = await dbContext.ServiceBookings
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

                int totalBookings = await dbContext.ServiceBookings
                    .Include(sb => sb.Service)
                    .Where(sb => sb.Service.Partner.Email == partnerEmail)
                    .GroupBy(sb => sb.DetailId)
                    .Select(group => group.Select(sb => sb.ServiceId).Distinct().Count())
                    .CountAsync();


                int returnPatients = allServices.Count(s => s.NumberOfBooking > 2);

                decimal earning = await dbContext.ServiceBookings
                    .Include(sb => sb.Service)
                    .Where(sb => sb.Detail != null
                                 && sb.Detail.Booking != null
                                 && sb.Service.Partner.Email == partnerEmail
                                 && sb.Detail.Booking.BookingLogs.Any(bl => bl.Status == 4))
                    .SumAsync(sb => sb.Service.Price);

                int operations = await dbContext.ServiceBookings
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
        public async Task<BookingDTO> GetBookingDetailInforByBookingIdAsync(int id)
        {
            try
            {
                var bookingDetail = await dbContext.BookingDetails
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
                var latestBookingLogStatus = await dbContext.BookingLogs
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
