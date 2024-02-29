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
        public async Task<List<IncomingBookingDTO>> GetIncomingBookingsByEmailAsync(string email)
        {
            try
            {
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
                if (customer == null)
                {
                    return new List<IncomingBookingDTO>();
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

        public async Task<List<IncomingBookingDTO>> GetIncomingBookingsByCustomerIdAsync(int customerId)
        {
            try
            {
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    return new List<IncomingBookingDTO>();
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
        public async Task<List<IncomingBookingDTO>> GetBookingsByCustomerIdAsync(string email)
        {
            try
            {
                var customer = await FindCustomerByEmailAsync(email);
                if (customer == null)
                {
                    return new List<IncomingBookingDTO>();
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

        public async Task<List<IncomingBookingDTO>> GetBookingsHaveStatus1ByEmailAsync(string partnerEmail)
        {
            try
            {
                var partner = await FindPartnerByEmailAsync(partnerEmail);
                if (partner == null)
                {
                    return new List<IncomingBookingDTO>();
                }

                var allPendingLogs = await GetAllPendingBookingLogsAsync();
                var pendingBookings = GroupPendingBookings(allPendingLogs);
                var result = await FilterAndMapPendingBookingsAsync(pendingBookings, partner);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsHaveStatus1ByEmailAsync: {ex.Message}", ex);
                throw;
            }
        }

        /*function support to get booking*/
        private async Task<Partner> FindPartnerByEmailAsync(string partnerEmail)
        {
            return await dbContext.Partners.FirstOrDefaultAsync(p => p.Email == partnerEmail);
        }


        private async Task<Schedule> GetPartnerScheduleAsync(int partnerId)
        {
            return await dbContext.Schedules.FirstOrDefaultAsync(s => s.PartnerId == partnerId);
        }

        private async Task<string> GetPaymentMethodNameAsync(int paymentMethodId)
        {
            var paymentMethod = await dbContext.PaymentMethods.FirstOrDefaultAsync(p => p.PaymentId == paymentMethodId);
            return paymentMethod?.Name ?? "Unknown Payment Method";
        }


        private async Task<Booking> GetBookingByLogAsync(BookingLog log)
        {
            return await dbContext.Bookings.FirstOrDefaultAsync(b => b.BookingId == log.BookingId);
        }

        private async Task<bool> HasStatusGreaterThan1Async(Booking booking)
        {
            return await dbContext.BookingLogs.AnyAsync(bl => bl.BookingId == booking.BookingId && bl.Status > 1);
        }

        private async Task<BookingDetail> GetBookingDetailAsync(Booking booking)
        {
            return await dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == booking.BookingId);
        }

        private async Task<Customer> GetCustomerByBookingDetailAsync(BookingDetail bookingDetail)
        {
            var medicalReport = await dbContext.MedicalReports.FirstOrDefaultAsync(mr => mr.ReportId == bookingDetail.ReportId);
            if (medicalReport != null)
            {
                return await dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == medicalReport.CustomerId);
            }
            return null;
        }

        private async Task<ServiceBooking> GetServiceBookingByDetailAsync(BookingDetail bookingDetail)
        {
            return await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
        }

        private async Task<PartnerService> GetPartnerServiceAsync(ServiceBooking serviceBooking, Partner partner)
        {
            return await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId && ps.PartnerId == partner.PartnerId);
        }

        private async Task<bool> AreValidStatusesAsync(PartnerService partnerService, Partner partner, Customer customer)
        {
            var serviceStatus = partnerService.Status;
            var customerStatus = customer.Status;

            return serviceStatus == 1 && customerStatus == 1;
        }

        private async Task<List<BookingLog>> GetLatestPendingLogsAsync()
        {
            return await dbContext.BookingLogs
                .Where(bl => bl.Status == 1)
                .GroupBy(bl => bl.BookingId)
                .Select(g => g.OrderByDescending(bl => bl.CreatedDate).FirstOrDefault())
                .ToListAsync();
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

        private async Task<List<MedicalServiceDTO>> GetMedicalServiceDTOsAsync(int bookingId, Customer customer)
        {
            var medicalReports = await dbContext.BookingDetails
                .Where(detail => detail.BookingId == bookingId)
                .Select(detail => detail.ReportId)
                .ToListAsync();

            var medicalServiceDTOs = new List<MedicalServiceDTO>();

            foreach (var reportId in medicalReports)
            {
                var medicalName = await dbContext.MedicalReports
                    .Where(mr => mr.ReportId == reportId && mr.CustomerId == customer.CustomerId)
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

        private async Task<string> GetPartnerNameAsync(int partnerId)
        {
            var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerId);
            return partner?.DisplayName ?? "Partner status là 0";
        }


        private async Task<DateTime> GetBookingDateAsync(int bookingId)
        {
            var bookingDate = await dbContext.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => b.CreatedDate)
                .FirstOrDefaultAsync();

            return bookingDate != null ? (DateTime)bookingDate : DateTime.MinValue;
        }

        private async Task<int?> GetBookingTimeAsync(int bookingId)
        {
            var bookingTime = await dbContext.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => b.bookingTime)
                .FirstOrDefaultAsync();

            return bookingTime;
        }

        private async Task<string> GetBookingAddressAsync(int bookingId)
        {
            var address = await dbContext.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => b.Address)
                .FirstOrDefaultAsync();

            return address ?? "Unknown Address";
        }

        private async Task<int?> GetPaymentMethodAsync(int bookingId)
        {
            var paymentMethod = await dbContext.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => b.PaymentId)
                .FirstOrDefaultAsync();

            return paymentMethod;
        }
        private async Task<List<BookingLog>> GetAllPendingBookingLogsAsync()
        {
            return await dbContext.BookingLogs
                .Where(bl => bl.Status == 1 || bl.Status == 2)
                .GroupBy(bl => bl.BookingId)
                .Select(g => g.OrderByDescending(bl => bl.CreatedDate).FirstOrDefault())
                .ToListAsync();
        }

        private List<IGrouping<int, BookingLog>> GroupPendingBookings(List<BookingLog> allBookingLogs)
        {
            return allBookingLogs
                 .Where(bl => bl.BookingId != null)
                .GroupBy(bl => bl.BookingId.Value)
                .ToList();
        }
        private async Task<int> GetPartnerIdFromBookingIdAsync(int bookingId)
        {
            var partnerService = await dbContext.ServiceBookings
                .Include(sb => sb.Service)
                    .ThenInclude(service => service.Partner)
                .Where(sb => sb.Detail.BookingId == bookingId)
                .Select(sb => sb.Service.PartnerId)
                .FirstOrDefaultAsync();

            return partnerService;
        }

        private async Task<List<IncomingBookingDTO>> FilterAndMapIncomingBookingsAsync(List<IGrouping<int, BookingLog>> pendingBookings, Customer customer)
        {
            var result = new List<IncomingBookingDTO>();

            foreach (var group in pendingBookings)
            {
                var bookingId = group.Key;
                var statuses = group.Select(bl => bl.Status).Distinct().ToList();

                foreach (var status in statuses)
                {
                    if (await IsBookingStatusValidAsync(bookingId, customer))
                    {
                        var partnerId = await GetPartnerIdFromBookingIdAsync(bookingId);
                        var medicalServiceDTOs = await GetMedicalServiceDTOsAsync(bookingId, customer);

                        result.Add(new IncomingBookingDTO
                        {
                            BookingId = bookingId,
                            Status = (int)status,
                            Partner = await GetPartnerNameAsync(partnerId),
                            BookingDate = await GetBookingDateAsync(bookingId),
                            bookingTime = (int)await GetBookingTimeAsync(bookingId),
                            Address = await GetBookingAddressAsync(bookingId),
                            PaymentMethod = (int)await GetPaymentMethodAsync(bookingId),
                            MedicalService = medicalServiceDTOs
                        });
                    }
                }
            }

            return result;
        }
        private async Task<List<IncomingBookingDTO>> FilterAndMapPendingBookingsAsync(List<IGrouping<int, BookingLog>> pendingBookings, Partner partner)
        {
            var result = new List<IncomingBookingDTO>();

            foreach (var group in pendingBookings)
            {
                var bookingId = group.Key;
                var statuses = group.Select(bl => bl.Status).Distinct().ToList();

                foreach (var status in statuses)
                {
                    if (status == 1) // Filter only for status 1
                    {
                        var booking = await GetBookingByLogAsync(group.FirstOrDefault()); // Get booking from the first log
                        if (booking != null)
                        {
                            var customer = await GetCustomerByBookingAsync(booking);
                            if (customer != null)
                            {
                                var medicalServiceDTOs = await GetMedicalServiceDTOsAsync(booking.BookingId, customer);

                                result.Add(new IncomingBookingDTO
                                {
                                    BookingId = booking.BookingId,
                                    Status = (int)status,
                                    Partner = partner.DisplayName,
                                    BookingDate = await GetBookingDateAsync(booking.BookingId),
                                    bookingTime = (int)await GetBookingTimeAsync(booking.BookingId),
                                    Address = await GetBookingAddressAsync(booking.BookingId),
                                    PaymentMethod = (int)await GetPaymentMethodAsync(bookingId),
                                    MedicalService = medicalServiceDTOs
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }
        private async Task<Customer> GetCustomerByBookingAsync(Booking booking)
        {
            var bookingDetail = await GetBookingDetailAsync(booking);
            if (bookingDetail != null)
            {
                var medicalReport = await GetMedicalReportByDetailAsync(bookingDetail);
                if (medicalReport != null)
                {
                    return await GetCustomerByReportAsync(medicalReport);
                }
            }
            return null;
        }
        private async Task<MedicalReport> GetMedicalReportByDetailAsync(BookingDetail bookingDetail)
        {
            return await dbContext.MedicalReports.FirstOrDefaultAsync(mr => mr.ReportId == bookingDetail.ReportId);
        }

        private async Task<Customer> GetCustomerByReportAsync(MedicalReport medicalReport)
        {
            return await dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == medicalReport.CustomerId);
        }
        /*end function*/
    }
}
