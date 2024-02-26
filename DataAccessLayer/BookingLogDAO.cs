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
                // Tìm CustomerId dựa trên email từ bảng Customer
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
                if (customer == null)
                {
                    // Nếu không tìm thấy khách hàng với email đã cung cấp, trả về danh sách trống
                    return new List<PendingBookingDTO>();
                }

                // Lấy tất cả các BookingLogs có trạng thái 1 hoặc 2
                var allBookingLogs = await dbContext.BookingLogs
                    .Where(bl => bl.Status == 1 || bl.Status == 2)
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
                                                })
                                                .ToList()
                                    }).GroupBy(service => service.ServiceId)
                                    .Select(group => group.First())
                                    .ToListAsync(),
                                        Address = booking.Address,
                                        PaymentMethod = await dbContext.PaymentMethods
                                    .Where(payment => payment.PaymentId == booking.PaymentId)
                                    .Select(payment => payment.Name)
                                    .FirstOrDefaultAsync(),
                                        MedicalName = medicalReport.Fullname
                                    };

                                    // Lấy thông tin về dịch vụ và thêm vào danh sách services của pendingBooking
                                    var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
                                    if (serviceBooking != null)
                                    {
                                        var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId);
                                        if (partnerService != null)
                                        {
                                            var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerService.PartnerId);
                                            if (partner != null)
                                            {
                                                pendingBooking.DisplayName = partner.DisplayName;

                                                // Tìm thông tin lịch trình từ Schedule
                                                var schedule = await dbContext.Schedules.FirstOrDefaultAsync(s => s.PartnerId == partner.PartnerId);
                                                if (schedule != null)
                                                {
                                                    pendingBooking.From = schedule.From.ToString();
                                                    pendingBooking.To = schedule.To.ToString();
                                                    pendingBooking.DayOfWeek = schedule.DayOfWeek;
                                                    pendingBooking.WorkShift = schedule.WorkShift;
                                                }
                                            }
                                        }
                                    }

                                    result.Add(pendingBooking);
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
                    // Nếu không tìm thấy khách hàng với email đã cung cấp, trả về danh sách trống
                    return new List<PendingBookingDTO>();
                }

                // Lấy tất cả các BookingLogs có trạng thái 1 hoặc 2
                var allBookingLogs = await dbContext.BookingLogs
                    .Where(bl => bl.Status == 1 || bl.Status == 2)
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
                                                })
                                                .ToList()
                                                }).GroupBy(service => service.ServiceId)
                                    .Select(group => group.First())
                                    .ToListAsync(),
                                        Address = booking.Address,
                                        PaymentMethod = await dbContext.PaymentMethods
                                    .Where(payment => payment.PaymentId == booking.PaymentId)
                                    .Select(payment => payment.Name)
                                    .FirstOrDefaultAsync(),
                                        MedicalName = medicalReport.Fullname
                                    };

                                    // Lấy thông tin về dịch vụ và thêm vào danh sách services của pendingBooking
                                    var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
                                    if (serviceBooking != null)
                                    {
                                        var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId);
                                        if (partnerService != null)
                                        {
                                            var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerService.PartnerId);
                                            if (partner != null)
                                            {
                                                pendingBooking.DisplayName = partner.DisplayName;

                                                // Tìm thông tin lịch trình từ Schedule
                                                var schedule = await dbContext.Schedules.FirstOrDefaultAsync(s => s.PartnerId == partner.PartnerId);
                                                if (schedule != null)
                                                {
                                                    pendingBooking.From = schedule.From.ToString();
                                                    pendingBooking.To = schedule.To.ToString();
                                                    pendingBooking.DayOfWeek = schedule.DayOfWeek;
                                                    pendingBooking.WorkShift = schedule.WorkShift;
                                                }
                                            }
                                        }
                                    }

                                    result.Add(pendingBooking);
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
                                                    })
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
                                        MedicalName = medicalReport.Fullname
                                    };
                                    // Lấy thông tin về dịch vụ và thêm vào danh sách services của pendingBooking
                                    var serviceBooking = await dbContext.ServiceBookings.FirstOrDefaultAsync(sb => sb.DetailId == bookingDetail.DetailId);
                                    if (serviceBooking != null)
                                    {
                                        var partnerService = await dbContext.PartnerServices.FirstOrDefaultAsync(ps => ps.ServiceId == serviceBooking.ServiceId);
                                        if (partnerService != null)
                                        {
                                            var partner = await dbContext.Partners.FirstOrDefaultAsync(p => p.PartnerId == partnerService.PartnerId);
                                            if (partner != null)
                                            {
                                                pendingBooking.DisplayName = partner.DisplayName;

                                                // Tìm thông tin lịch trình từ Schedule
                                                var schedule = await dbContext.Schedules.FirstOrDefaultAsync(s => s.PartnerId == partner.PartnerId);
                                                if (schedule != null)
                                                {
                                                    pendingBooking.From = schedule.From.ToString();
                                                    pendingBooking.To = schedule.To.ToString();
                                                    pendingBooking.DayOfWeek = schedule.DayOfWeek;
                                                    pendingBooking.WorkShift = schedule.WorkShift;
                                                }
                                            }
                                        }
                                    }

                                    result.Add(pendingBooking);
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

    }
}
