using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class BookingLogDAO
    {
        private static BookingLogDAO instance = null;
        private readonly LumosDBContext dbContext;

        public BookingLogDAO(LumosDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public static BookingLogDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BookingLogDAO(new LumosDBContext());
                }
                return instance;
            }
        }

        public async Task<bool> UpdateBookingLogStatusForPartnerAsync(int bookingLogId, int newStatus)
        {
            try
            {
                var bookingLog = await dbContext.BookingLogs.FindAsync(bookingLogId);
                if (bookingLog == null)
                {
                    throw new ArgumentException($"Booking log with ID {bookingLogId} not found");
                }

                bookingLog.Status = newStatus;
                await dbContext.SaveChangesAsync();
                return true;
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
                var bookingLog = await dbContext.BookingLogs.FindAsync(bookingLogId);
                if (bookingLog == null)
                {
                    throw new ArgumentException($"Booking log with ID {bookingLogId} not found");
                }

                bookingLog.Status = newStatus;
                await dbContext.SaveChangesAsync();
                return true;
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
                var latestBookingLog = await dbContext.BookingLogs
                    .Where(log => log.BookingId == bookingId)
                    .OrderByDescending(log => log.CreatedDate)
                    .FirstOrDefaultAsync();

                return latestBookingLog;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLatestBookingLogAsync: {ex.Message}", ex);
                throw; 
            }
        }

        public async Task<bool> CreateBookingLogAsync(BookingLog bookingLog)
        {
            try
            {
                dbContext.BookingLogs.Add(bookingLog);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateBookingLogAsync: {ex.Message}", ex);
                return false;
            }
        }
        public async Task<List<PendingBookingDTO>> GetPendingBookingsByEmailAsync(string email)
        {
            try
            {
                var pendingBookings = await dbContext.Bookings
                    .GroupJoin(dbContext.Customers,
                        booking => booking.BookingDetails.FirstOrDefault().ReportId,
                        customer => customer.MedicalReports.FirstOrDefault().ReportId,
                        (booking, customers) => new { Booking = booking, Customers = customers })
                    .SelectMany(
                        bc => bc.Customers.DefaultIfEmpty(),
                        (bc, customer) => new { Booking = bc.Booking, Customer = customer })
                    .Where(bc =>
                        // Filter by customer email
                        (bc.Customer != null && bc.Customer.Email == email))
                    .Select(b =>
                        new PendingBookingDTO
                        {
                            BookingId = b.Booking.BookingId,
                            Status = b.Booking.BookingLogs.OrderByDescending(bl => bl.CreatedDate)
                                        .FirstOrDefault().Status ?? 0,
                            BookingDate = (DateTime)b.Booking.CreatedDate,
                            PartnerName = null, // Set PartnerName to null
                            From = null, // Set From to null
                            To = null, // Set To to null
                            Services = b.Booking.BookingDetails
                                        .SelectMany(detail => detail.ServiceBookings)
                                        .Select(serviceBooking => new PartnerServiceDTO
                                        {
                                            ServiceId = serviceBooking.Service.ServiceId,
                                            Name = serviceBooking.Service.Name,
                                            Code = serviceBooking.Service.Code,
                                            Duration = serviceBooking.Service.Duration,
                                            Status = serviceBooking.Service.Status,
                                            Description = serviceBooking.Service.Description,
                                            Price = serviceBooking.Service.Price,
                                            BookedQuantity = serviceBooking.Service.ServiceBookings.Count,
                                            Rating = serviceBooking.Service.Rating,
                                            Categories = serviceBooking.Service.ServiceDetails
                                                            .Select(detail => new ServiceCategoryDTO
                                                            {
                                                                CategoryId = detail.Category.CategoryId,
                                                                Category = detail.Category.Category,
                                                                Code = detail.Category.Code
                                                            })
                                                            .Distinct() // Ensure distinct categories
                                                            .ToList()
                                        }).ToList()
                        })
                    .ToListAsync();

                foreach (var pendingBooking in pendingBookings)
                {
                    var bookingDetail = await dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == pendingBooking.BookingId);
                    if (bookingDetail != null)
                    {
                        var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
                        if (serviceBooking != null)
                        {
                            var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId);
                            if (partnerService != null)
                            {
                                var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerService.PartnerId);
                                if (partner != null)
                                {
                                    pendingBooking.PartnerName = partner.PartnerName;
                                    /*var schedule = await dbContext.Schedules.FirstOrDefaultAsync(s => s.PartnerId == partner.PartnerId);
                                    if (schedule != null)
                                    {
                                        pendingBooking.From = schedule.From.ToString();
                                        pendingBooking.To = schedule.To.ToString();
                                    }*/
                                }
                            }
                        }
                    }
                }

                return pendingBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPendingBookingsByEmailAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<PendingBookingDTO>> GetPendingBookingsByCustomerIdAsync(int customerId)
        {
            try
            {
                var pendingBookings = await dbContext.Bookings
                    .Where(booking => booking.BookingDetails.Any(detail => detail.Report.CustomerId == customerId))
                    .Select(b =>
                        new PendingBookingDTO
                        {
                            BookingId = b.BookingId,
                            Status = (int)b.BookingLogs.OrderByDescending(bl => bl.CreatedDate)
                                        .FirstOrDefault().Status, // Remove the null-coalescing operator ??
                            BookingDate = (DateTime)b.CreatedDate,
                            PartnerName = null, // Set PartnerName to null
                            From = null, // Set From to null
                            To = null, // Set To to null
                            Services = b.BookingDetails
                                        .SelectMany(detail => detail.ServiceBookings)
                                        .Select(serviceBooking => new PartnerServiceDTO
                                        {
                                            ServiceId = serviceBooking.Service.ServiceId,
                                            Name = serviceBooking.Service.Name,
                                            Code = serviceBooking.Service.Code,
                                            Duration = serviceBooking.Service.Duration,
                                            Status = serviceBooking.Service.Status,
                                            Description = serviceBooking.Service.Description,
                                            Price = serviceBooking.Service.Price,
                                            BookedQuantity = serviceBooking.Service.ServiceBookings.Count,
                                            Rating = serviceBooking.Service.Rating,
                                            Categories = serviceBooking.Service.ServiceDetails
                                                            .Select(detail => new ServiceCategoryDTO
                                                            {
                                                                CategoryId = detail.Category.CategoryId,
                                                                Category = detail.Category.Category,
                                                                Code = detail.Category.Code
                                                            })
                                                            .Distinct() // Ensure distinct categories
                                                            .ToList()
                                        }).ToList()
                        })
                    .ToListAsync();

                foreach (var pendingBooking in pendingBookings)
                {
                    var bookingDetail = await dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == pendingBooking.BookingId);
                    if (bookingDetail != null)
                    {
                        var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
                        if (serviceBooking != null)
                        {
                            var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId);
                            if (partnerService != null)
                            {
                                var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerService.PartnerId);
                                if (partner != null)
                                {
                                    pendingBooking.PartnerName = partner.PartnerName;
                                    /*var schedule = await dbContext.Schedules.FirstOrDefaultAsync(s => s.PartnerId == partner.PartnerId);
                                    if (schedule != null)
                                    {
                                        pendingBooking.From = schedule.From.ToString();
                                        pendingBooking.To = schedule.To.ToString();
                                    }*/
                                }
                            }
                        }
                    }
                }

                return pendingBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPendingBookingsByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }


    }
}
