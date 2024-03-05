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


        public static BookingLogDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BookingLogDAO();
                }
                return instance;
            }
        }

        public async Task<bool> UpdateBookingLogStatusForPartnerAsync(int bookingLogId, int newStatus)
        {
            try
            {
                using var dbContext = new LumosDBContext();
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
                using var dbContext = new LumosDBContext();
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
                using var dbContext = new LumosDBContext();
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
                using var dbContext = new LumosDBContext();
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

        public async Task<string> GetBookingStatusListAndCheckIsPayAsync(int bookingId)
        {
            try
            {
                using var dbContext = new LumosDBContext();
                // Lấy danh sách trạng thái của booking dựa trên bookingId
                var statuses = await dbContext.BookingLogs
                    .Where(bl => bl.BookingId == bookingId)
                    .Select(bl => bl.Status)
                    .ToListAsync();

                if (statuses.Count == 2 && (statuses.All(s => s == (int)BookingStatusEnum.WaitingForPayment) && statuses.All(s => s == (int)BookingStatusEnum.Canceled)))
                {
                    return "No";
                }
                // Nếu danh sách có đúng 2 phần tử nhưng không đều là 0 hoặc không đều là 1, hoặc nếu có nhiều hơn 2 phần tử và trong đó có ít nhất một phần tử là 0 hoặc 1, thì trả về "Yes"
                else if ((statuses.Count == 2 && (statuses.All(s => s != (int)BookingStatusEnum.WaitingForPayment) || statuses.All(s => s != (int)BookingStatusEnum.Canceled))) || statuses.Count > 2 && (statuses.Contains((int)BookingStatusEnum.WaitingForPayment) || statuses.Contains((int)BookingStatusEnum.Canceled)))
                {
                    return "Yes";
                }
                // Trường hợp còn lại, trả về "No"
                else
                {
                    return "No";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingStatusListAndCheckIsPayAsync: {ex.Message}", ex);
                throw;
            }
        }
       
        public async Task<bool> IsBookingStatusValidAsync(int bookingId, Customer customer)
        {
            using var dbContext = new LumosDBContext();
            var hasStatusGreaterThan2 = await dbContext.BookingLogs.AnyAsync(bl => bl.BookingId == bookingId && bl.Status > (int)BookingStatusEnum.Doing);
            if (hasStatusGreaterThan2)
            {
                return false;
            }
            var bookingDetail = await dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == bookingId);
            var medicalReport = await dbContext.MedicalReports.FirstOrDefaultAsync(mr => mr.ReportId == bookingDetail.ReportId && mr.CustomerId == customer.CustomerId);
            var booking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
            var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId);
            var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerService.PartnerId);

            if (bookingDetail == null || medicalReport == null || booking == null || serviceBooking == null || partnerService == null || partner == null)
            {
                return false;
            }

            var serviceStatus = partnerService.Status;
            var customerStatus = customer.Status;

            if (serviceStatus != 1 || customerStatus != 1)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CheckStatusForGetAllBooking(int bookingId, Customer customer)
        {
            using var dbContext = new LumosDBContext();
            var bookingDetail = await dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == bookingId);
            var medicalReport = await dbContext.MedicalReports.FirstOrDefaultAsync(mr => mr.ReportId == bookingDetail.ReportId && mr.CustomerId == customer.CustomerId);
            var booking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
            var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId);
            var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerService.PartnerId);

            if (bookingDetail == null || medicalReport == null || booking == null || serviceBooking == null || partnerService == null || partner == null)
            {
                return false;
            }

            var serviceStatus = partnerService.Status;
            var customerStatus = customer.Status;

            if (serviceStatus != 1 || customerStatus != 1)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> CheckStatusForGetAllBookingWithPartner(int bookingId, Partner inputPartner)
        {
            using var dbContext = new LumosDBContext();
            var bookingDetail = await dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == bookingId);
            var medicalReport = await dbContext.MedicalReports.FirstOrDefaultAsync(mr => mr.ReportId == bookingDetail.ReportId);
            var booking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
            var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId && ps.PartnerId == inputPartner.PartnerId);

            if (bookingDetail == null || medicalReport == null || booking == null || serviceBooking == null || partnerService == null || inputPartner == null)
            {
                return false;
            }

            var serviceStatus = partnerService.Status;
            var partnerStatus = inputPartner.Status;

            if (serviceStatus != 1)
            {
                return false;
            }

            return true;
        }
        public async Task<List<MedicalServiceDTO>> GetMedicalServiceDTOsAsync(int bookingId)
        {
            using var dbContext = new LumosDBContext();
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
                        Price = serviceBooking.Price,
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
        public async Task<List<BillMedicalDTO>> GetMedicalServiceBillDTOsAsync(int bookingId)
        {
            using var dbContext = new LumosDBContext();
            var medicalReports = await dbContext.BookingDetails
                .Where(detail => detail.BookingId == bookingId)
                .Select(detail => detail.ReportId)
                .ToListAsync();

            var medicalServiceDTOs = new List<BillMedicalDTO>();

            foreach (var reportId in medicalReports)
            {
                var medicalName = await dbContext.MedicalReports
                    .Where(mr => mr.ReportId == reportId)
                    .Select(mr => mr.Fullname)
                    .FirstOrDefaultAsync();

                var serviceDTOs = await dbContext.BookingDetails
                    .Where(detail => detail.BookingId == bookingId && detail.ReportId == reportId) // Filter by ReportId
                    .SelectMany(detail => detail.ServiceBookings)
                    .Select(serviceBooking => new BillServiceDTO
                    {
                        Name = serviceBooking.Service.Name,
                        Price = serviceBooking.Price,
                    }).ToListAsync();

                medicalServiceDTOs.Add(new BillMedicalDTO
                {
                    MedicalName = medicalName,
                    Services = serviceDTOs
                });
            }

            return medicalServiceDTOs;
        }

        public async Task<List<BookingLog>> GetAllLogAsync()
        {
            using var dbContext = new LumosDBContext();
            return await dbContext.BookingLogs
                .GroupBy(bl => bl.BookingId)
                .Select(g => g.OrderByDescending(bl => bl.CreatedDate).FirstOrDefault())
                .ToListAsync();
        }
        public async Task<List<BookingLog>> GetAllPendingBookingLogsAsync()
        {
            using var dbContext = new LumosDBContext();
            return await dbContext.BookingLogs
                .Where(bl => bl.Status == (int)BookingStatusEnum.Pending 
                || bl.Status == (int)BookingStatusEnum.Doing)
                .GroupBy(bl => bl.BookingId)
                .Select(g => g.OrderByDescending(bl => bl.CreatedDate).FirstOrDefault())
                .ToListAsync();
        }
        public async Task<List<BookingLog>> GetAllBookingLogsAsync()
        {
            using var dbContext = new LumosDBContext();
            return await dbContext.BookingLogs
                .Where(bl => bl.Status != (int)BookingStatusEnum.WaitingForPayment)
                .GroupBy(bl => bl.BookingId)
                .Select(g => g.OrderByDescending(bl => bl.CreatedDate).FirstOrDefault())
                .ToListAsync();
        }
        public async Task<List<BookingLog>> GetALLBookingBillsAsync()
        {
            using var dbContext = new LumosDBContext();
            return await dbContext.BookingLogs
                .GroupBy(bl => bl.BookingId)
                .Select(g => g.OrderByDescending(bl => bl.CreatedDate).FirstOrDefault())
                .ToListAsync();
        }

        public List<IGrouping<int, BookingLog>> GroupBookings(List<BookingLog> allBookingLogs)
        {
            return allBookingLogs
                 .Where(bl => bl.BookingId != null)
                .GroupBy(bl => bl.BookingId.Value)
                .ToList();
        }
        public async Task<string> GetPartnerIdFromBookingIdAsync(int bookingId)
        {
            using var dbContext = new LumosDBContext();
            var partnerService = await dbContext.ServiceBookings
                .Include(sb => sb.Service)
                    .ThenInclude(service => service.Partner)
                .Where(sb => sb.Detail.BookingId == bookingId)
                .Select(sb => sb.Service.Partner.DisplayName)
                .FirstOrDefaultAsync();

            return partnerService;
        }
        public async Task<Booking> GetBookingByBookidAsync(int bookingId)
        {
            try
            {
                using var dbContext = new LumosDBContext();
                Booking booking = await dbContext.Bookings
                    .Where(sb => sb.BookingId == bookingId)
                    .FirstOrDefaultAsync();
                return booking;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAllCustomersAsync", ex);
            }
        }
        public async Task<int?> GetTotalPriceFromBookingByidAsync(int bookingId)
        {
            using var dbContext = new LumosDBContext();
            var totalprice = await dbContext.Bookings
                .Where(sb => sb.BookingId == bookingId)
                .Select(sb => sb.TotalPrice)
                .FirstOrDefaultAsync();

            return totalprice;
        }

        public async Task<BookingInfoDTO> GetBookingDetailsBillsByIdAsync(int bookingId)
        {
            try
            {
                using var dbContext = new LumosDBContext();

                var bookingDetailsAndCustomer = await (from b in dbContext.Bookings
                                                       join pay in dbContext.PaymentMethods on b.PaymentId equals pay.PaymentId
                                                       join bd in dbContext.BookingDetails on b.BookingId equals bd.BookingId
                                                       join mr in dbContext.MedicalReports on bd.ReportId equals mr.ReportId
                                                       join sb in dbContext.ServiceBookings on bd.DetailId equals sb.DetailId
                                                       where b.BookingId == bookingId
                                                       select new BookingInfoDTO
                                                       {
                                                           PaymentMethod = pay.Name,
                                                           Note = bd.Note,
                                                           Booking = b,
                                                           Customer = mr.Customer,
                                                           PartnerName = sb.Service.Partner.DisplayName // Add partner name here
                                                       }).FirstOrDefaultAsync();

                return bookingDetailsAndCustomer;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetBookingDetailsBillsByIdAsync", ex);
            }
        }
        public async Task<BookingInfoDTO> GetBookingDetailsByIdAsync(int bookingId)
        {
            try
            {
                using var dbContext = new LumosDBContext();

                var bookingDetailsAndCustomer = await (from b in dbContext.Bookings
                                                       join pay in dbContext.PaymentMethods on b.PaymentId equals pay.PaymentId
                                                       join bd in dbContext.BookingDetails on b.BookingId equals bd.BookingId
                                                       join mr in dbContext.MedicalReports on bd.ReportId equals mr.ReportId
                                                       join sb in dbContext.ServiceBookings on bd.DetailId equals sb.DetailId
                                                       where b.BookingId == bookingId
                                                       select new BookingInfoDTO
                                                       {
                                                           PaymentMethod = pay.Name,
                                                           Note = dbContext.BookingLogs
                                                               .Where(bl => bl.BookingId == bookingId)
                                                               .OrderByDescending(bl => bl.CreatedDate)
                                                               .Select(bl => bl.Note)
                                                               .FirstOrDefault(),
                                                           Booking = b,
                                                           Customer = mr.Customer,
                                                           PartnerName = sb.Service.Partner.DisplayName // Add partner name here
                                                       }).FirstOrDefaultAsync();

                return bookingDetailsAndCustomer;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetBookingDetailsByIdAsync", ex);
            }
        }

    }
}
