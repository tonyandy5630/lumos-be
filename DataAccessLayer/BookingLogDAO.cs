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
        public async Task<List<BookingDTO>> GetIncomingBookingsByEmailAsync(string email)
        {
            try
            {
                using var dbContext = new LumosDBContext();
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
                if (customer == null)
                {
                    return new List<BookingDTO>();
                }

                var allbookingbill = await GetAllPendingBookingLogsAsync();
                var bookingbill = GroupPendingBookings(allbookingbill);
                var result = await FilterAndMapIncomingBookingsAsync(bookingbill, customer);

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
                using var dbContext = new LumosDBContext();
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

                var allBookingLogs = await GetAllBookingLogsAsync();
                var pendingBookings = GroupPendingBookings(allBookingLogs);
                var result = await FilterAndMapAllBookingsAsync(pendingBookings, customer);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<List<BillDTO>> GetBookingsBillsByCustomerIdAsync(string email)
        {
            try
            {
                var customer = await FindCustomerByEmailAsync(email);
                if (customer == null)
                {
                    return new List<BillDTO>();
                }

                var allBookingLogs = await GetALLBookingBillsAsync();
                var pendingBookings = GroupPendingBookings(allBookingLogs);
                var result = await FilterAndMapAllBillBookingsAsync(pendingBookings, customer);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<List<BillDetailDTO>> GetBookingsBillsByBookingidAsync(int bookingId)
        {
            try
            {
                var allBookingLogs = await GetALLBookingBillsAsync();
                var BillDetails = GroupPendingBookings(allBookingLogs);
                var result = await FilterAndMapAllBillBookingsByBookingIdAsync(BillDetails, bookingId);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsBillsByBookingidAsync: {ex.Message}", ex);
                throw;
            }
        }

        private async Task<List<BillDetailDTO>> FilterAndMapAllBillBookingsByBookingIdAsync(List<IGrouping<int, BookingLog>> BillDetails, int bookingId)
        {
            var result = new List<BillDetailDTO>();

            foreach (var group in BillDetails)
            {
                if (group.Key == bookingId)
                {
                    var statuses = group.Select(bl => bl.Status).Distinct().ToList();
                    var customer = await FindCustomerByBookingIdAsync(bookingId);
                    if (customer == null)
                    {
                        return new List<BillDetailDTO>();
                    }
                    foreach (var status in statuses)
                    {
                        
                        var isPay = await GetBookingStatusListAndCheckIsPayAsync(bookingId);
                        if (await CheckStatusForGetAllBooking(bookingId, customer))
                        {
                            var partnerName = await GetPartnerIdFromBookingIdAsync(bookingId);
                            var medicalServiceDTOs = await GetMedicalServiceBillDTOsAsync(bookingId);
                            var bookingInfo = await GetBookingByBookidAsync(bookingId);
                            result.Add(new BillDetailDTO
                            {
                                BookingId = bookingId,
                                BookingCode = bookingInfo.Code,
                                Status = status,
                                Address = bookingInfo.Address,
                                Partner = partnerName,
                                PaymentMethod = await GetPaymentMethodAsync(bookingId),
                                TotalPrice = bookingInfo.TotalPrice,
                                BookingDate = (DateTime)bookingInfo.CreatedDate,
                                bookingTime = bookingInfo.bookingTime,
                                Note = await GetNoteFromBookingByidAsync(bookingId),
                                IsPay = isPay,
                                MedicalServices = medicalServiceDTOs,
                            });
                        }
                    }
                }
            }

            return result;
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
        public async Task<Customer> FindCustomerByBookingIdAsync(int bookingId)
        {
            try
            {
                using var dbContext = new LumosDBContext();
                var customer = await (from bd in dbContext.BookingDetails
                                      join mr in dbContext.MedicalReports on bd.ReportId equals mr.ReportId
                                      where bd.BookingId == bookingId
                                      select mr.Customer).FirstOrDefaultAsync();

                return customer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindCustomerByBookingIdAsync: {ex.Message}", ex);
                throw;
            }
        }

        private async Task<Customer> FindCustomerByEmailAsync(string email)
        {
            using var dbContext = new LumosDBContext();
            return await dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email && c.Status == 1);
        }

        
        private async Task<bool> IsBookingStatusValidAsync(int bookingId, Customer customer)
        {
            using var dbContext = new LumosDBContext();
            var hasStatusGreaterThan2 = await dbContext.BookingLogs.AnyAsync(bl => bl.BookingId == bookingId && bl.Status > (int)BookingStatusEnum.Doing);
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

        private async Task<bool> CheckStatusForGetAllBooking(int bookingId, Customer customer)
        {
            using var dbContext = new LumosDBContext();
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
                        Price = serviceBooking.Service.Price,
                    }).ToListAsync();

                medicalServiceDTOs.Add(new BillMedicalDTO
                {
                    MedicalName = medicalName,
                    Services = serviceDTOs
                });
            }

            return medicalServiceDTOs;
        }

        public async Task<string> GetPartnerNameAsync(int partnerId)
        {
            using var dbContext = new LumosDBContext();
            var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerId);
            return partner?.DisplayName ?? "Partner status là 0";
        }


        public async Task<DateTime> GetBookingDateAsync(int bookingId)
        {
            using var dbContext = new LumosDBContext();
            var bookingDate = await dbContext.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => b.CreatedDate)
                .FirstOrDefaultAsync();

            return bookingDate != null ? (DateTime)bookingDate : DateTime.MinValue;
        }

        public async Task<int?> GetBookingTimeAsync(int bookingId)
        {
            using var dbContext = new LumosDBContext();
            var bookingTime = await dbContext.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => b.bookingTime)
                .FirstOrDefaultAsync();

            return bookingTime;
        }

        public async Task<string> GetBookingAddressAsync(int bookingId)
        {
            using var dbContext = new LumosDBContext();
            var address = await dbContext.Bookings
                .Where(b => b.BookingId == bookingId)
                .Select(b => b.Address)
                .FirstOrDefaultAsync();

            return address ?? "Unknown Address";
        }

        public async Task<string> GetPaymentMethodAsync(int bookingId)
        {
            using var dbContext = new LumosDBContext();
            var paymentName = (from b in dbContext.Bookings
                               join pay in dbContext.PaymentMethods on b.PaymentId equals pay.PaymentId
                               where b.BookingId == 3
                               select pay.Name).FirstOrDefaultAsync();

            return await paymentName;
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

        public List<IGrouping<int, BookingLog>> GroupPendingBookings(List<BookingLog> allBookingLogs)
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
        public async Task<string> GetNoteFromBookingByidAsync(int bookingId)
        {
            using var dbContext = new LumosDBContext();
            var totalprice = await dbContext.BookingDetails
                .Where(sb => sb.BookingId == bookingId)
                .Select(sb => sb.Note)
                .FirstOrDefaultAsync();

            return totalprice;
        }
        public async Task<string> GetPaymentLinkIdFromBookingByidAsync(int bookingId)
        {
            using var dbContext = new LumosDBContext();
            var totalprice = await dbContext.Bookings
                .Where(sb => sb.BookingId == bookingId)
                .Select(sb => sb.PaymentLinkId)
                .FirstOrDefaultAsync();

            return totalprice;
        }

        private async Task<List<BookingDTO>> FilterAndMapIncomingBookingsAsync(List<IGrouping<int, BookingLog>> pendingBookings, Customer customer)
        {
            using var dbContext = new LumosDBContext();
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
                        var partnerName = await GetPartnerIdFromBookingIdAsync(bookingId);
                        var medicalServiceDTOs = await GetMedicalServiceDTOsAsync(bookingId);
                        var bookinginfor = await GetBookingByBookidAsync(bookingId);
                        result.Add(new BookingDTO
                        {
                            BookingId = bookingId,
                            BookingCode = bookinginfor.Code,
                            Status = status,
                            PaymentLinkId = bookinginfor.PaymentLinkId,
                            Partner = partnerName,
                            TotalPrice = bookinginfor.TotalPrice,
                            BookingDate = (DateTime)bookinginfor.CreatedDate,
                            bookingTime = bookinginfor.bookingTime,
                            Address = bookinginfor.Address,
                            PaymentMethod = await GetPaymentMethodAsync(bookingId),
                            Note = await GetNoteFromBookingByidAsync(bookingId),
                            Customer = customer,
                            MedicalServices = medicalServiceDTOs
                        });
                    }
                }
            }

            return result;
        }
        private async Task<List<BookingDTO>> FilterAndMapAllBookingsAsync(List<IGrouping<int, BookingLog>> pendingBookings, Customer customer)
        {
            using var dbContext = new LumosDBContext();
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
                    if (await CheckStatusForGetAllBooking(bookingId, customer))
                    {
                        var partnerName = await GetPartnerIdFromBookingIdAsync(bookingId);
                        var medicalServiceDTOs = await GetMedicalServiceDTOsAsync(bookingId);
                        var bookinginfor = await GetBookingByBookidAsync(bookingId);
                        result.Add(new BookingDTO
                        {
                            BookingId = bookingId,
                            BookingCode = bookinginfor.Code,
                            Status = status,
                            Partner = partnerName,
                            PaymentLinkId = bookinginfor.PaymentLinkId,
                            TotalPrice = bookinginfor.TotalPrice,
                            BookingDate = bookinginfor.BookingDate,
                            bookingTime = bookinginfor.bookingTime,
                            Address = bookinginfor.Address,
                            PaymentMethod = await GetPaymentMethodAsync(bookingId),
                            Note = await GetNoteFromBookingByidAsync(bookingId),
                            Customer = customer,
                            MedicalServices = medicalServiceDTOs
                        });
                    }
                }
            }
            var waitingForPaymentBookings = result.Where(b => b.Status == (int)BookingStatusEnum.WaitingForPayment).ToList();
            if (waitingForPaymentBookings.Any())
            {
                result = waitingForPaymentBookings.Concat(result.Except(waitingForPaymentBookings)).ToList();
            }

            return result;
        }
        private async Task<List<BillDTO>> FilterAndMapAllBillBookingsAsync(List<IGrouping<int, BookingLog>> pendingBookings, Customer customer)
        {
            using var dbContext = new LumosDBContext();
            var result = new List<BillDTO>();

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
                    if (await CheckStatusForGetAllBooking(bookingId, customer))
                    {
                        var partnerName = await GetPartnerIdFromBookingIdAsync(bookingId);
                        var medicalServiceDTOs = await GetMedicalServiceDTOsAsync(bookingId);
                        var bookinginfor = await GetBookingByBookidAsync(bookingId);
                        result.Add(new BillDTO
                        {
                            BookingId = bookingId,
                            BookingCode = bookinginfor.Code,
                            Status = status,
                            PartnerName = partnerName,
                            TotalPrice = bookinginfor.TotalPrice,
                            BookingDate = bookinginfor.BookingDate,
                            bookingTime = bookinginfor.bookingTime,
                            Note = await GetNoteFromBookingByidAsync(bookingId),
                        });
                    }
                }
            }
            var waitingForPaymentBookings = result.Where(b => b.Status == (int)BookingStatusEnum.WaitingForPayment).ToList();
            if (waitingForPaymentBookings.Any())
            {
                result = waitingForPaymentBookings.Concat(result.Except(waitingForPaymentBookings)).ToList();
            }

            return result;
        }
    }
}
