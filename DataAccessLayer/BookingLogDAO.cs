using BussinessObject;
using DataTransferObject.DTO;
using Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utils;

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
        public async Task<List<BookingDTO>> GetIncomingBookingsByEmailAsync(string email)
        {
            try
            {
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
                if (customer == null)
                {
                    return new List<BookingDTO>();
                }

                var allPendingLogs = await GetAllPendingBookingLogsAsync();
                var pendingBookings = GroupPendingBookings(allPendingLogs);
                var result = await FilterAndMapIncomingBookingsAsync(pendingBookings, customer);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetIncomingBookingsByEmailAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<BookingDTO>> GetIncomingBookingsByCustomerIdAsync(int customerId)
        {
            try
            {
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    return new List<BookingDTO>();
                }

                var allBookingLogs = await GetAllPendingBookingLogsAsync();
                var pendingBookings = GroupPendingBookings(allBookingLogs);
                var result = await FilterAndMapIncomingBookingsAsync(pendingBookings, customer);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetIncomingBookingsByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        } 
        public async Task<List<BookingDTO>> GetBookingsByCustomerIdAsync(string email)
        {
            try
            {
                var customer = await FindCustomerByEmailAsync(email);
                if (customer == null)
                {
                    return new List<BookingDTO>();
                }

                var allBookingLogs = await GetAllPendingBookingLogsAsync();
                var pendingBookings = GroupPendingBookings(allBookingLogs);
                var result = await FilterAndMapIncomingBookingsAsync(pendingBookings, customer);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }
        private async Task<Customer> FindCustomerByEmailAsync(string email)
        {
            return await dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        
        private async Task<bool> IsBookingStatusValidAsync(int bookingId, Customer customer)
        {
            var hasStatusGreaterThan2 = await dbContext.BookingLogs.AnyAsync(bl => bl.BookingId == bookingId && bl.Status > 2);
            if (hasStatusGreaterThan2)
            {
                return false;
            }

            var bookingDetail = await dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == bookingId);
            if (bookingDetail == null)
            {
                return false;
            }

            var medicalReport = await dbContext.MedicalReports.FirstOrDefaultAsync(mr => mr.ReportId == bookingDetail.ReportId && mr.CustomerId == customer.CustomerId);
            if (medicalReport == null)
            {
                return false;
            }

            var booking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return false;
            }

            var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
            if (serviceBooking == null)
            {
                return false;
            }

            var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId);
            if (partnerService == null)
            {
                return false;
            }

            var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerService.PartnerId);
            if (partner == null)
            {
                return false;
            }

            var serviceStatus = partnerService.Status;
            /*            var partnerStatus = partner.Status;*/
            var customerStatus = customer.Status;

            if (serviceStatus != 1 || customerStatus != 1)
            {
                return false;
            }

            return true;
        }

        public async Task<List<MedicalServiceDTO>> GetMedicalServiceDTOsAsync(int bookingId)
        {
            var medicalReports = await dbContext.BookingDetails
                .Where(detail => detail.BookingId == bookingId)
                .Select(detail => detail.ReportId)
                .ToListAsync();

            var medicalServiceDTOs = new List<MedicalServiceDTO>();

            foreach (var reportId in medicalReports)
            {
                var medicalName = await dbContext.MedicalReports
                    .Where(mr => mr.ReportId == reportId)
                    .Select(mr => mr.Fullname)
                    .FirstOrDefaultAsync();

                var serviceDTOs = await dbContext.BookingDetails
                    .Where(detail => detail.BookingId == bookingId && detail.ReportId == reportId) // Filter by ReportId
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
                    }).ToListAsync();

                medicalServiceDTOs.Add(new MedicalServiceDTO
                {
                    MedicalName = medicalName,
                    Services = serviceDTOs
                });
            }

            return medicalServiceDTOs;
        }

        public async Task<string> GetPartnerNameAsync(int partnerId)
        {
            var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerId);
            return partner?.DisplayName ?? "Partner status là 0";
        }


        public async Task<DateTime> GetBookingDateAsync(int bookingId)
        {
            var bookingDate = await dbContext.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => b.CreatedDate)
                .FirstOrDefaultAsync();

            return bookingDate != null ? (DateTime)bookingDate : DateTime.MinValue;
        }

        public async Task<int?> GetBookingTimeAsync(int bookingId)
        {
            var bookingTime = await dbContext.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => b.bookingTime)
                .FirstOrDefaultAsync();

            return bookingTime;
        }

        public async Task<string> GetBookingAddressAsync(int bookingId)
        {
            var address = await dbContext.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => b.Address)
                .FirstOrDefaultAsync();

            return address ?? "Unknown Address";
        }

        public async Task<string> GetPaymentMethodAsync(int bookingId)
        {
            var paymentName = (from b in dbContext.Bookings
                               join pay in dbContext.PaymentMethods on b.PaymentId equals pay.PaymentId
                               where b.BookingId == 3
                               select pay.Name).FirstOrDefaultAsync();

            return await paymentName;
        }
        public async Task<List<BookingLog>> GetAllPendingBookingLogsAsync()
        {
            return await dbContext.BookingLogs
                .Where(bl => bl.Status == 1 || bl.Status == 2)
                .GroupBy(bl => bl.BookingId)
                .Select(g => g.OrderByDescending(bl => bl.CreatedDate).FirstOrDefault())
                .ToListAsync();
        }

        public List<IGrouping<int, BookingLog>> GroupPendingBookings(List<BookingLog> allBookingLogs)
        {
            return allBookingLogs
                 .Where(bl => bl.BookingId != null)
                .GroupBy(bl => bl.BookingId.Value)
                .ToList();
        }
        public async Task<int> GetPartnerIdFromBookingIdAsync(int bookingId)
        {
            var partnerService = await dbContext.ServiceBookings
                .Include(sb => sb.Service)
                    .ThenInclude(service => service.Partner)
                .Where(sb => sb.Detail.BookingId == bookingId)
                .Select(sb => sb.Service.PartnerId)
                .FirstOrDefaultAsync();

            return partnerService;
        }
        public async Task<int?> GetTotalPriceFromBookingByidAsync(int bookingId)
        {
            var totalprice = await dbContext.Bookings
                .Where(sb => sb.BookingId == bookingId)
                .Select(sb => sb.TotalPrice)
                .FirstOrDefaultAsync();

            return totalprice;
        }
        public async Task<string> GetNoteFromBookingByidAsync(int bookingId)
        {
            var totalprice = await dbContext.BookingDetails
                .Where(sb => sb.BookingId == bookingId)
                .Select(sb => sb.Note)
                .FirstOrDefaultAsync();

            return totalprice;
        }

        private async Task<List<BookingDTO>> FilterAndMapIncomingBookingsAsync(List<IGrouping<int, BookingLog>> pendingBookings, Customer customer)
        {
            var result = new List<BookingDTO>();

            foreach (var group in pendingBookings)
            {
                var bookingId = group.Key;
                var statuses = group.Select(bl => bl.Status).Distinct().ToList();
                var bookingDetail = await dbContext.BookingDetails
                .FirstOrDefaultAsync(bd => bd.BookingId == bookingId);
                if (bookingDetail == null)
                {
                    // Xử lý khi không tìm thấy BookingDetail
                    continue;
                }
                var reportId = bookingDetail.ReportId;
                foreach (var status in statuses)
                {
                    if (await IsBookingStatusValidAsync(bookingId, customer))
                    {
                        var partnerId = await GetPartnerIdFromBookingIdAsync(bookingId);
                        var medicalServiceDTOs = await GetMedicalServiceDTOsAsync(bookingId);

                        result.Add(new BookingDTO
                        {
                            BookingId = bookingId,
                            Status = EnumUtils.GetBookingEnumByStatus(status),
                            Partner = await GetPartnerNameAsync(partnerId),
                            TotalPrice = await GetTotalPriceFromBookingByidAsync(bookingId),
                            BookingDate = await GetBookingDateAsync(bookingId),
                            bookingTime = (int)await GetBookingTimeAsync(bookingId),
                            Address = await GetBookingAddressAsync(bookingId),
                            PaymentMethod = await GetPaymentMethodAsync(bookingId),
                            Note = await GetNoteFromBookingByidAsync(bookingId),
                            Customer = await BookingDAO.Instance.GetCustomerByReportIdAsync(reportId),
                            MedicalServices = medicalServiceDTOs
                        });
                    }
                }
            }

            return result;
        }

        internal Task<string> GetPartnerIdFromBookingIdAsync(int? bookingId)
        {
            throw new NotImplementedException();
        }
    }
}
