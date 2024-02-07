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

                if (createBookingDTO.ReportId > 0)
                {
                    var report = await dbContext.MedicalReports.FindAsync(createBookingDTO.ReportId);
                    if (report == null)
                    {
                        throw new ArgumentException($"Medical report with ID {createBookingDTO.ReportId} not found");
                    }
                }
                booking.Code =GenerateCode.GenerateTableCode("booking");
                booking.CreatedDate = DateTime.Now;
                dbContext.Bookings.Add(booking);
                await dbContext.SaveChangesAsync();

                if (booking.BookingId != null && booking.BookingId > 0)
                {
                    var bookingDetail = new BookingDetail
                    {
                        BookingId = booking.BookingId,
                        Note = createBookingDTO.Note,
                        ReportId = createBookingDTO.ReportId,
                        CreatedDate = booking.CreatedDate,
                        CreatedBy = createBookingDTO.CreatedBy
                    };
                    dbContext.BookingDetails.Add(bookingDetail);

                    var bookingLog = new BookingLog
                    {
                        BookingId = booking.BookingId,
                        Status = 1,
                        CreatedDate = booking.CreatedDate,
                        Note = createBookingDTO.Note,
                        CreatedBy = createBookingDTO.CreatedBy
                    };
                    dbContext.BookingLogs.Add(bookingLog);

                    await dbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateBookingAsync: {ex.Message}", ex);
                return false;
            }
        }

    }
}
