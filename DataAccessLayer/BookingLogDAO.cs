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
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
                if (customer == null)
                {
                    return new List<PendingBookingDTO>();
                }

                // Lấy tất cả các BookingLogs có trạng thái 1 hoặc 2
                var allBookingLogs = await dbContext.BookingLogs
                    .Where(bl => bl.Status == 1 || bl.Status == 2)
                    .GroupBy(bl => bl.BookingId)
                    .Select(g => g.OrderByDescending(bl => bl.CreatedDate).FirstOrDefault())
                    .ToListAsync();

                // Lọc BookingLogs theo BookingId và nhóm chúng lại
                var pendingBookings = allBookingLogs
                    .GroupBy(bl => bl.BookingId)
                    .ToList();

                var result = new List<PendingBookingDTO>();

                foreach (var group in pendingBookings)
                {
                    var bookingId = group.Key;
                    var statuses = group.Select(bl => bl.Status).Distinct().ToList();

                    foreach (var status in statuses)
                    {
                        // Kiểm tra xem có bất kỳ trạng thái nào lớn hơn 2 không
                        var hasStatusGreaterThan2 = await dbContext.BookingLogs.AnyAsync(bl => bl.BookingId == bookingId && bl.Status > 2);
                        if (!hasStatusGreaterThan2)
                        {
                            // Tìm ReportId từ BookingDetail dựa trên BookingId
                            var bookingDetail = await dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == bookingId);
                            if (bookingDetail != null)
                            {
                                // Tìm CustomerId từ MedicalReport dựa trên ReportId
                                var medicalReport = await dbContext.MedicalReports.FirstOrDefaultAsync(mr => mr.ReportId == bookingDetail.ReportId && mr.CustomerId == customer.CustomerId);
                                if (medicalReport != null)
                                {
                                    var booking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
                                    if (booking != null)
                                    {
                                        // Kiểm tra status của dịch vụ, đối tác, hoặc khách hàng
                                        var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
                                        if (serviceBooking != null)
                                        {
                                            var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId);
                                            if (partnerService != null)
                                            {
                                                var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerService.PartnerId);
                                                if (partner != null)
                                                {
                                                    var serviceStatus = partnerService.Status;
                                                    var partnerStatus = partner.Status;
                                                    var customerStatus = customer.Status;

                                                    // Kiểm tra status của dịch vụ, đối tác, hoặc khách hàng
                                                    if (serviceStatus == 1 && partnerStatus == 1 && customerStatus == 1)
                                                    {
                                                        // Tiếp tục thêm thông tin vào danh sách kết quả
                                                        var pendingBooking = new PendingBookingDTO
                                                        {
                                                            BookingId = booking.BookingId,
                                                            Status = (int)status,
                                                            BookingDate = (DateTime)booking.CreatedDate,
                                                            Services = await dbContext.BookingDetails
                                                                    .Where(detail => detail.BookingId == bookingId)
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
                                                                }).Distinct()
                                                                .ToList()
                                                                    }).GroupBy(service => service.ServiceId)
                                                    .Select(group => group.First())
                                                    .ToListAsync(),
                                                            Address = booking.Address,
                                                            PaymentMethod = await dbContext.PaymentMethods
                                                            .Where(payment => payment.PaymentId == booking.PaymentId)
                                                            .Select(payment => payment.Name)
                                                            .FirstOrDefaultAsync(),
                                                            MedicalName = medicalReport.Fullname,
                                                            bookingTime = booking.bookingTime
                                                        };

                                                        if (serviceBooking != null)
                                                        {
                                                            pendingBooking.DisplayName = partner.DisplayName;

                                                            var schedule = await dbContext.Schedules.FirstOrDefaultAsync(s => s.PartnerId == partner.PartnerId);
                                                            if (schedule != null)
                                                            {
                                                                pendingBooking.From = schedule.From.ToString();
                                                                pendingBooking.To = schedule.To.ToString();
                                                                pendingBooking.WorkShift = schedule.WorkShift;
                                                            }
                                                        }

                                                        result.Add(pendingBooking);
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Invalid status for partner.");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return result;
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
                // Tìm CustomerId dựa trên email từ bảng Customer
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    // Nếu không tìm thấy khách hàng với id đã cung cấp, trả về danh sách trống
                    return new List<PendingBookingDTO>();
                }

                // Lấy tất cả các BookingLogs có trạng thái 1 hoặc 2 và lấy logs mới nhất cho mỗi booking
                var allBookingLogs = await dbContext.BookingLogs
                    .Where(bl => bl.Status == 1 || bl.Status == 2)
                    .GroupBy(bl => bl.BookingId)
                    .Select(g => g.OrderByDescending(bl => bl.CreatedDate).FirstOrDefault())
                    .ToListAsync();

                // Lọc BookingLogs theo BookingId và nhóm chúng lại
                var pendingBookings = allBookingLogs
                    .GroupBy(bl => bl.BookingId)
                    .ToList();

                var result = new List<PendingBookingDTO>();

                foreach (var group in pendingBookings)
                {
                    var bookingId = group.Key;
                    var statuses = group.Select(bl => bl.Status).Distinct().ToList();

                    foreach (var status in statuses)
                    {
                        // Kiểm tra xem có bất kỳ trạng thái nào lớn hơn 2 không
                        var hasStatusGreaterThan2 = await dbContext.BookingLogs.AnyAsync(bl => bl.BookingId == bookingId && bl.Status > 2);
                        if (!hasStatusGreaterThan2)
                        {
                            // Tìm ReportId từ BookingDetail dựa trên BookingId
                            var bookingDetail = await dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == bookingId);
                            if (bookingDetail != null)
                            {
                                // Tìm CustomerId từ MedicalReport dựa trên ReportId
                                var medicalReport = await dbContext.MedicalReports.FirstOrDefaultAsync(mr => mr.ReportId == bookingDetail.ReportId && mr.CustomerId == customer.CustomerId);
                                if (medicalReport != null)
                                {
                                    var booking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
                                    if (booking != null)
                                    {
                                        // Kiểm tra status của dịch vụ, đối tác, và khách hàng
                                        var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
                                        if (serviceBooking != null)
                                        {
                                            var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId);
                                            if (partnerService != null)
                                            {
                                                var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerService.PartnerId);
                                                if (partner != null)
                                                {
                                                    // Kiểm tra status của dịch vụ, đối tác, và khách hàng
                                                    var serviceStatus = serviceBooking.Service.Status;
                                                    var partnerStatus = partner.Status;
                                                    var customerStatus = customer.Status;

                                                    if (serviceStatus == 1 && partnerStatus == 1 && customerStatus == 1)
                                                    {
                                                        var pendingBooking = new PendingBookingDTO
                                                        {
                                                            BookingId = booking.BookingId,
                                                            Status = (int)status,
                                                            BookingDate = (DateTime)booking.CreatedDate,
                                                            Services = await dbContext.BookingDetails
                                                                    .Where(detail => detail.BookingId == bookingId)
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
                                                                                }).Distinct()
                                                                                .ToList()
                                                                    }).GroupBy(service => service.ServiceId)
                                                        .Select(group => group.First())
                                                        .ToListAsync(),
                                                            Address = booking.Address,
                                                            PaymentMethod = await dbContext.PaymentMethods
                                                        .Where(payment => payment.PaymentId == booking.PaymentId)
                                                        .Select(payment => payment.Name)
                                                        .FirstOrDefaultAsync(),
                                                            MedicalName = medicalReport.Fullname,
                                                            bookingTime = booking.bookingTime
                                                        };

                                                        // Lấy thông tin về dịch vụ và thêm vào danh sách services của pendingBooking
                                                        if (serviceBooking != null)
                                                        {
                                                            pendingBooking.DisplayName = partner.DisplayName;

                                                            // Tìm thông tin lịch trình từ Schedule
                                                            var schedule = await dbContext.Schedules.FirstOrDefaultAsync(s => s.PartnerId == partner.PartnerId);
                                                            if (schedule != null)
                                                            {
                                                                pendingBooking.From = schedule.From.ToString();
                                                                pendingBooking.To = schedule.To.ToString();
                                                                pendingBooking.WorkShift = schedule.WorkShift;
                                                            }
                                                        }

                                                        result.Add(pendingBooking);
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Invalid status for service, partner, or customer.");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPendingBookingsByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<PendingBookingDTO>> GetBookingsByCustomerIdAsync(string email)
        {
            try
            {
                // Tìm CustomerId dựa trên customerId từ bảng Customer
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
                if (customer == null)
                {
                    // Nếu không tìm thấy khách hàng với customerId đã cung cấp, trả về danh sách trống
                    return new List<PendingBookingDTO>();
                }

                // Lấy tất cả các BookingLogs
                var allBookingLogs = await dbContext.BookingLogs.ToListAsync();

                // Lọc BookingLogs theo BookingId và nhóm chúng lại
                var pendingBookings = allBookingLogs
                    .GroupBy(bl => bl.BookingId)
                    .ToList();

                var result = new List<PendingBookingDTO>();

                foreach (var group in pendingBookings)
                {
                    var bookingId = group.Key;
                    var statuses = group.Select(bl => bl.Status).Distinct().ToList();

                    foreach (var status in statuses)
                    {
                        // Tìm ReportId từ BookingDetail dựa trên BookingId
                        var bookingDetail = await dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == bookingId);
                        if (bookingDetail != null)
                        {
                            // Tìm CustomerId từ MedicalReport dựa trên ReportId
                            var medicalReport = await dbContext.MedicalReports.FirstOrDefaultAsync(mr => mr.ReportId == bookingDetail.ReportId && mr.CustomerId == customer.CustomerId);
                            if (medicalReport != null)
                            {
                                var booking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
                                if (booking != null)
                                {
                                    // Kiểm tra status của dịch vụ, đối tác, hoặc khách hàng
                                    var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
                                    if (serviceBooking != null)
                                    {
                                        var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId);
                                        if (partnerService != null)
                                        {
                                            var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerService.PartnerId);
                                            if (partner != null)
                                            {
                                                var serviceStatus = partnerService.Status;
                                                var partnerStatus = partner.Status;
                                                var customerStatus = customer.Status;

                                                // Kiểm tra status của dịch vụ, đối tác, hoặc khách hàng
                                                if (serviceStatus == 1 && partnerStatus == 1 && customerStatus == 1)
                                                {
                                                    // Tiếp tục thêm thông tin vào danh sách kết quả
                                                    var pendingBooking = new PendingBookingDTO
                                                    {
                                                        BookingId = booking.BookingId,
                                                        Status = (int)status,
                                                        BookingDate = (DateTime)booking.CreatedDate,
                                                        Services = await dbContext.BookingDetails
                                                                .Where(detail => detail.BookingId == bookingId)
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
                                                            }).Distinct()
                                                            .ToList()
                                                                }).GroupBy(service => service.ServiceId)
                                                    .Select(group => group.First())
                                                    .ToListAsync(),
                                                        Address = booking.Address,
                                                        PaymentMethod = await dbContext.PaymentMethods
                                                            .Where(payment => payment.PaymentId == booking.PaymentId)
                                                            .Select(payment => payment.Name)
                                                            .FirstOrDefaultAsync(),
                                                        MedicalName = medicalReport.Fullname,
                                                        bookingTime = booking.bookingTime // Lấy bookingTime từ Booking
                                                    };

                                                    // Lấy thông tin về dịch vụ và thêm vào danh sách services của pendingBooking
                                                    if (serviceBooking != null)
                                                    {
                                                        pendingBooking.DisplayName = partner.DisplayName;

                                                        // Tìm thông tin lịch trình từ Schedule
                                                        var schedule = await dbContext.Schedules.FirstOrDefaultAsync(s => s.PartnerId == partner.PartnerId);
                                                        if (schedule != null)
                                                        {
                                                            pendingBooking.From = schedule.From.ToString();
                                                            pendingBooking.To = schedule.To.ToString();
                                                            pendingBooking.WorkShift = schedule.WorkShift;
                                                        }
                                                    }

                                                    result.Add(pendingBooking);
                                                }
                                                else
                                                {
                                                    throw new Exception("Invalid status for service, partner, or customer.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<List<PendingBookingDTO>> GetBookingsHaveStatus1ByEmailAsync(string partnerEmail)
        {
            try
            {
                var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.Email == partnerEmail);
                if (partner == null)
                {
                    return new List<PendingBookingDTO>();
                }

                var latestPendingLogs = await dbContext.BookingLogs
                    .Where(bl => bl.Status == 1)
                    .GroupBy(bl => bl.BookingId)
                    .Select(g => g.OrderByDescending(bl => bl.CreatedDate).FirstOrDefault())
                    .ToListAsync();

                var result = new List<PendingBookingDTO>();

                foreach (var log in latestPendingLogs)
                {
                    var booking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.BookingId == log.BookingId);
                    if (booking != null)
                    {
                        // Kiểm tra xem có bất kỳ trạng thái nào khác với 1 không
                        var hasStatusGreaterThan1 = await dbContext.BookingLogs.AnyAsync(bl => bl.BookingId == booking.BookingId && bl.Status > 1);
                        if (!hasStatusGreaterThan1)
                        {
                            var bookingDetail = await dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == booking.BookingId);
                            if (bookingDetail != null)
                            {
                                var medicalReport = await dbContext.MedicalReports.FirstOrDefaultAsync(mr => mr.ReportId == bookingDetail.ReportId);
                                if (medicalReport != null)
                                {
                                    // Lấy CustomerId từ MedicalReport
                                    int? customerId = medicalReport.CustomerId;

                                    // Lấy thông tin trạng thái của khách hàng
                                    var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.Status == 1);
                                    if (customer != null)
                                    {
                                        var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
                                        if (serviceBooking != null)
                                        {
                                            var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId && ps.PartnerId == partner.PartnerId);
                                            if (partnerService != null)
                                            {
                                                // Kiểm tra status của dịch vụ, đối tác, và khách hàng
                                                var serviceStatus = serviceBooking.Service.Status;
                                                var partnerStatus = partner.Status;
                                                var customerStatus = customer.Status;

                                                if (serviceStatus == 1 && partnerStatus == 1 && customerStatus == 1)
                                                {
                                                    var pendingBooking = new PendingBookingDTO
                                                    {
                                                        BookingId = booking.BookingId,
                                                        Status = (int)log.Status,
                                                        BookingDate = (DateTime)booking.CreatedDate,
                                                        Services = await dbContext.BookingDetails
                                                            .Where(detail => detail.BookingId == booking.BookingId)
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
                                                                    }).Distinct()
                                                                    .ToList()
                                                            })
                                                            .GroupBy(service => service.ServiceId)
                                                            .Select(group => group.First())
                                                            .ToListAsync(),
                                                        Address = booking.Address,
                                                        PaymentMethod = await dbContext.PaymentMethods
                                                            .Where(payment => payment.PaymentId == booking.PaymentId)
                                                            .Select(payment => payment.Name)
                                                            .FirstOrDefaultAsync(),
                                                        MedicalName = medicalReport.Fullname, // Lấy tên từ MedicalReport
                                                        DisplayName = partner.DisplayName,
                                                        From = "N/A", // Không có lịch làm việc cho đối tác
                                                        To = "N/A",
                                                        WorkShift = 0,
                                                        bookingTime = booking.bookingTime
                                                    };

                                                    // Truy cập thông tin lịch làm việc của đối tác từ danh sách lịch làm việc của đối tác
                                                    var schedule = await dbContext.Schedules.FirstOrDefaultAsync(s => s.PartnerId == partner.PartnerId);
                                                    if (schedule != null)
                                                    {
                                                        pendingBooking.From = schedule.From.ToString();
                                                        pendingBooking.To = schedule.To.ToString();
                                                        pendingBooking.WorkShift = schedule.WorkShift;
                                                    }

                                                    result.Add(pendingBooking);
                                                }
                                                else
                                                {
                                                    throw new Exception("Invalid status for service, partner, or customer.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookingsHaveStatus1ByPartnerEmailAsync: {ex.Message}", ex);
                throw;
            }
        }

    }
}
