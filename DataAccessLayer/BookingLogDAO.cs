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
                        booking => booking.CreatedBy,
                        customer => customer.Fullname,
                        (booking, customers) => new { Booking = booking, Customers = customers })
                    .SelectMany(
                        bc => bc.Customers.DefaultIfEmpty(),
                        (bc, customer) => new { Booking = bc.Booking, Customer = customer })
                    .GroupJoin(dbContext.Partners,
                        bc => bc.Booking.CreatedBy,
                        partner => partner.Email,
                        (bc, partners) => new { BookingCustomer = bc, Partners = partners })
                    .SelectMany(
                        bcp => bcp.Partners.DefaultIfEmpty(),
                        (bcp, partner) => new { BookingCustomer = bcp.BookingCustomer, Partner = partner })
                    .Where(bcp => (bcp.BookingCustomer.Customer != null && bcp.BookingCustomer.Customer.Email == email)
                                  || (bcp.BookingCustomer.Booking.BookingLogs.OrderByDescending(bl => bl.CreatedDate)
                                        .FirstOrDefault().Status == 0 && bcp.Partner != null && bcp.Partner.Email == email))
                    .Select(bcp =>
                        new PendingBookingDTO
                        {
                            BookingId = bcp.BookingCustomer.Booking.BookingId,
                            Status = bcp.BookingCustomer.Booking.BookingLogs.OrderByDescending(bl => bl.CreatedDate)
                                        .FirstOrDefault().Status ?? 0,
                            // Map any other properties as needed
                        })
                    .ToListAsync();

                return pendingBookings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPendingBookingsByEmailAsync: {ex.Message}", ex);
                throw;
            }
        }

    }
}
