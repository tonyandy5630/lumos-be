using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        public async Task<bool> CreateBookingAsync(Booking booking, CreateBookingDTO createBookingDTO)
        {
            try
            {
                if (booking == null)
                {
                    throw new ArgumentNullException(nameof(booking), "Booking object cannot be null");
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

                dbContext.Bookings.Add(booking);
                await dbContext.SaveChangesAsync();

                foreach (var cartItem in createBookingDTO.CartModel)
                {
                    // Tạo booking detail cho mỗi reportId
                    var bookingDetail = new BookingDetail
                    {
                        BookingId = booking.BookingId,
                        Note = createBookingDTO.Note,
                        ReportId = cartItem.ReportId,
                        CreatedDate = booking.CreatedDate,
                        CreatedBy = createBookingDTO.CreatedBy
                    };
                    dbContext.BookingDetails.Add(bookingDetail);
                    await dbContext.SaveChangesAsync();
                    // Tạo booking log cho mỗi booking detail
                    var bookingLog = new BookingLog
                    {
                        BookingId = booking.BookingId,
                        Status = 1, // Status mặc định khi tạo booking detail
                        CreatedDate = booking.CreatedDate,
                        Note = createBookingDTO.Note,
                        CreatedBy = createBookingDTO.CreatedBy
                    };
                    dbContext.BookingLogs.Add(bookingLog);
                    await dbContext.SaveChangesAsync();
                    foreach (var service in cartItem.Services)
                    {
                        var serviceBooking = new ServiceBooking
                        {
                            ServiceId = service.ServiceId,
                            DetailId = bookingDetail.DetailId, // Sử dụng DetailId từ BookingDetail mới tạo
                            Price = (int?)service.Price,
                            Description = service.Description,
                            CreatedDate = booking.CreatedDate,
                            LastUpdate = (DateTime)booking.CreatedDate,
                            UpdatedBy = createBookingDTO.CreatedBy
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
                                CategoryId = categoryId, // Gán CategoryId từ cơ sở dữ liệu
                                CreatedDate = (DateTime)booking.CreatedDate,
                                LastUpdate = (DateTime)booking.CreatedDate,
                                CreatedBy = createBookingDTO.CreatedBy,
                                UpdatedBy = createBookingDTO.CreatedBy
                            };
                            dbContext.ServiceDetails.Add(serviceDetail);
                        }
                    }
                }

                // Lưu thay đổi vào database
                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateBookingAsync: {ex.Message}", ex);
                return false;
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

    }
}
