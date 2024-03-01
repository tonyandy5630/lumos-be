using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DataAccessLayer
{
    public class PartnerDAO
    {
        private static PartnerDAO instance = null;

        public static PartnerDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PartnerDAO();
                }
                return instance;
            }
        }

        public async Task<Partner?> GetPartnerByBookingIdAsync(int bookingId)
        {
            try
            {
                using var _context = new LumosDBContext();
                Partner? partner = await (from sb in _context.ServiceBookings
                                            join bd in _context.BookingDetails on sb.DetailId equals bd.DetailId
                                            join mr in _context.MedicalReports on bd.ReportId equals mr.ReportId
                                            join b in _context.Bookings on bd.BookingId equals b.BookingId
                                            join ps in _context.PartnerServices on sb.ServiceId equals ps.ServiceId
                                            join p in _context.Partners on ps.PartnerId equals p.PartnerId
                                            where b.BookingId == bookingId
                                            select p).FirstOrDefaultAsync();
                return partner;
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<int> CountAppPartnerAsync()
        {
            try
            {
                using LumosDBContext _context = new LumosDBContext();
                return await _context.Partners.CountAsync();
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<PartnerServiceDTO?> GetPartnerServiceByIdAsync(int serviceId)
        {
            try
            {
                using var _context = new LumosDBContext();
                var result = from ps in _context.PartnerServices
                             join sb in _context.ServiceBookings
                             on ps.ServiceId equals sb.ServiceId into serviceBookings
                             from sb in serviceBookings.DefaultIfEmpty()
                             where ps.ServiceId == serviceId
                             group new { ps, sb } by
                             new { ps.ServiceId, ps.Name, ps.Description, ps.Price, ps.Code, ps.Status, ps.CreatedDate, ps.Rating, ps.UpdatedBy, ps.Duration, ps.LastUpdate }
                              into grouped
                             select new PartnerServiceDTO
                             {
                                 ServiceId = grouped.Key.ServiceId,
                                 Name = grouped.Key.Name,
                                 Description = grouped.Key.Description,
                                 Price = grouped.Key.Price,
                                 Code = grouped.Key.Code,
                                 Status = grouped.Key.Status,
                                 CreatedDate = grouped.Key.CreatedDate,
                                 UpdatedBy = grouped.Key.UpdatedBy,
                                 LastUpdate = grouped.Key.LastUpdate,
                                 Duration = grouped.Key.Duration,
                                 BookedQuantity = grouped.Count(entry => entry.sb != null),
                                 Rating = grouped.Key.Rating
                             };

                return await result.FirstOrDefaultAsync(s => s.ServiceId == serviceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerServiceByIdAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<PartnerService?> AddPartnerServiceAsync(PartnerService service)
        {
            using var _context = new LumosDBContext();
            await _context.PartnerServices.AddAsync(service);

            if (await _context.SaveChangesAsync() == 1)
                return service;
            return null;
        }

        public async Task<IEnumerable<Partner>> SearchPartnerByServiceOrPartnerNameAsync(string keyword)
        {
            using var _context = new LumosDBContext();
            var result = _context.Partners
                        .Where(p => p.Status == 1 && (p.PartnerName.Contains(keyword) || p.PartnerServices.Any(s => s.Name.Contains(keyword))))
                        .Include(s => s.PartnerServices
                        .Where(s => s.Name.Contains(keyword)))
                        .AsNoTracking()
                        .ToListAsync();
            return await result;
        }

        public async Task<IEnumerable<PartnerService>> GetServiceOfPartnerByServiceName(string keyword, int partnerId)
        {
            using var _context = new LumosDBContext();
            return await _context.PartnerServices.Where(s => s.PartnerId == partnerId && s.Name.Contains(keyword)).ToListAsync();
        }
        public async Task<List<Partner>> GetAllPartnersAsync()
        {
            List<Partner> partners = new List<Partner>();
            try
            {
                using var _context = new LumosDBContext();
                partners = await _context.Partners.ToListAsync();
                if (partners == null || partners.Count == 0)
                {
                    Console.WriteLine("No partners was found!");
                    return null;
                }
                return partners;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllPartnersAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<Partner> GetPartnerByIDAsync(int id)
        {
            try
            {
                using var _context = new LumosDBContext();
                Partner partner = await _context.Partners.SingleOrDefaultAsync(u => u.PartnerId == id);
                if (partner != null)
                {
                    partner.PartnerServices = await _context.PartnerServices.Where(ps => ps.PartnerId == id).ToListAsync();
                }
                return partner;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerByIDAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<Partner> GetPartnerByRefreshTokenAsync(string token)
        {
            try
            {
                using var _context = new LumosDBContext();
                return await _context.Partners.SingleOrDefaultAsync(u => u.RefreshToken.Equals(token));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerByRefreshTokenAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<Partner> GetPartnerByCodeAsync(string code)
        {
            try
            {
                using var _context = new LumosDBContext();
                Partner? partner = await _context.Partners.SingleOrDefaultAsync(u => u.Code.ToLower().Equals(code.ToLower()));

                if (partner != null)
                {
                    partner.PartnerServices = await _context.PartnerServices.Where(ps => ps.PartnerId == partner.PartnerId).ToListAsync();
                }
                return partner;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerByCodeAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<Partner?> GetPartnerByEmailAsync(string email)
        {
            try
            {
                using var _context = new LumosDBContext();
                Partner partner = await _context.Partners.SingleOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()) && u.Status == 1);
                if (partner != null)
                {
                    partner.PartnerServices = await _context.PartnerServices.Where(ps => ps.PartnerId == partner.PartnerId).ToListAsync();
                }
                return partner;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerByEmailAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<Partner?> AddPartnereAsync(Partner partner)
        {
            try
            {
                using var _context = new LumosDBContext();
                await _context.Partners.AddAsync(partner);
                int success = await _context.SaveChangesAsync();
                if (success != 1)
                    return null;
                return partner;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPartnereAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Partner?> GetPartnerByDisplayNameAsync(string displayName)
        {
            try
            {
                using var _context = new LumosDBContext();
                return await _context.Partners.FirstOrDefaultAsync(p => p.DisplayName.Equals(displayName));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Partner?> GetPartnerByBussinessLicenseAsync(string bussinessLisence)
        {
            try
            {
                using var _context = new LumosDBContext();
                return await _context.Partners.FirstOrDefaultAsync(p => p.BusinessLicenseNumber.Equals(bussinessLisence));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Partner?> GetPartnerByPartnerNameAsync(string name)
        {
            try
            {
                using var _context = new LumosDBContext();
                return await _context.Partners.FirstOrDefaultAsync(p => p.PartnerName.Equals(name));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Partner?> GetPartnerByPartnerEmailAsync(string email)
        {
            try
            {
                using var _context = new LumosDBContext();
                return await _context.Partners.FirstOrDefaultAsync(p => p.Email.Equals(email));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdatePartnerAsync(Partner partner)
        {
            try
            {
                using var _context = new LumosDBContext();
                var existing = await _context.Partners.SingleOrDefaultAsync(s => s.PartnerId == partner.PartnerId);
                if (existing != null)
                {
                    existing.PartnerName = partner.PartnerName;
                    existing.Description = partner.Description;
                    existing.Status = partner.Status;

                    await _context.SaveChangesAsync();
                    Console.WriteLine("Partner updated successfully!");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdatePartnerAsync: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<bool> BanPartnerAsync(int partnerId)
        {
            try
            {
                using var _context = new LumosDBContext();
                Partner partner = await _context.Partners.SingleOrDefaultAsync(s => s.PartnerId == partnerId);

                if (partner != null)
                {
                    partner.Status = (partner.Status == 0) ? 1 : 0;

                    _context.Entry(partner).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Partner status updated successfully!");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Ban Partner: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Partner>> GetPartnersByCategoryAsync(int categoryId)
        {
            try
            {
                using var _context = new LumosDBContext();
                var result = _context.Partners
                .Where(p => p.TypeId == categoryId)
                .AsNoTracking()
                .ToListAsync();
                return await result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnersByCategoryAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<RevenuePerWeekDTO>> CalculatePartnerRevenueInMonthAsync(int month, int year)
        {
            try
            {
                using var _context = new LumosDBContext();
                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                var revenuePerWeek = await _context.BookingLogs
                    .Where(bl => bl.Status == 4 && bl.CreatedDate >= startDate && bl.CreatedDate <= endDate)
                    .Join(_context.ServiceBookings,
                        bl => bl.BookingId,
                        sb => sb.Detail.BookingId,
                        (bl, sb) => new { sb.Price, bl.CreatedDate })
                    .GroupBy(x => ((x.CreatedDate.Value.Day - 1) / 7) + 1)
                    .OrderBy(g => g.Key)
                    .Select(g => new RevenuePerWeekDTO
                    {
                        WeekNumber = g.Key,
                        Revenue = (decimal)g.Sum(x => x.Price),
                        StartDate = startDate.AddDays((g.Key - 1) * 7), // Ngày bắt đầu của tuần
                        EndDate = startDate.AddDays(g.Key * 7).AddDays(-1) // Ngày kết thúc của tuần
                    })
                    .ToListAsync();

                return revenuePerWeek;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CalculatePartnerRevenueInMonthAsync: {ex.Message}", ex);
                throw;
            }
        }


        public async Task<List<MonthlyRevenueDTO>> CalculateMonthlyRevenueAsync(int year)
        {
            try
            {
                using var _context = new LumosDBContext();
                List<MonthlyRevenueDTO> monthlyRevenueList = new List<MonthlyRevenueDTO>();

                for (int month = 1; month <= 12; month++)
                {
                    var monthlyRevenue = await CalculatePartnerRevenueInMonthAsync(month, year);
                    int totalRevenue = (int)monthlyRevenue.Sum(r => r.Revenue);

                    if (monthlyRevenue.Count == 0)
                    {
                        monthlyRevenueList.Add(new MonthlyRevenueDTO
                        {
                            Month = month,
                            Revenue = 0,
                            Details = new List<RevenuePerWeekDTO>()
                        });
                    }
                    else
                    {
                        monthlyRevenueList.Add(new MonthlyRevenueDTO
                        {
                            Month = month,
                            Revenue = totalRevenue,
                            Details = monthlyRevenue
                        });
                    }
                }

                return monthlyRevenueList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CalculateMonthlyRevenueAsync: {ex.Message}", ex);
                throw;
            }
        }


        public async Task<List<PartnerServiceDTO>> GetPartnerServicesWithBookingCountAsync(int partnerId)
        {
            try
            {
                using var _context = new LumosDBContext();
                var result = await (from ps in _context.PartnerServices
                                    join sd in _context.ServiceDetails on ps.ServiceId equals sd.ServiceId
                                    join sc in _context.ServiceCategories on sd.CategoryId equals sc.CategoryId
                                    where ps.PartnerId == partnerId
                                    select new
                                    {
                                        PartnerService = ps,
                                        ServiceBookings = _context.ServiceBookings.Where(sb => sb.ServiceId == ps.ServiceId).ToList(), // Convert to List
                                        Category = sc // Select category
                                    })
                                    .GroupBy(x => x.PartnerService.ServiceId) // Group by ServiceId
                                    .Select(g => g.First()) // Select the first item from each group
                                    .ToListAsync(); // Execute the query and return the result as a list

                var partnerServiceDTOs = result.Select(x => new PartnerServiceDTO
                {
                    ServiceId = x.PartnerService.ServiceId,
                    Name = x.PartnerService.Name,
                    Description = x.PartnerService.Description,
                    Price = x.PartnerService.Price,
                    Code = x.PartnerService.Code,
                    Status = x.PartnerService.Status,
                    CreatedDate = x.PartnerService.CreatedDate,
                    UpdatedBy = x.PartnerService.UpdatedBy,
                    LastUpdate = x.PartnerService.LastUpdate,
                    Duration = x.PartnerService.Duration,
                    BookedQuantity = x.ServiceBookings.Count(),
                    Categories = new List<ServiceCategoryDTO>
                    {
                        new ServiceCategoryDTO
                        {
                             CategoryId = x.Category.CategoryId,
                             Category = x.Category.Category,
                             Code = x.Category.Code
                        }
                    }
                }).ToList();

                return partnerServiceDTOs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerServicesWithBookingCountAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<StatPartnerServiceDTO> CalculateServicesAndRevenueAsync(string? email)
        {
            using var _context = new LumosDBContext();
            if (email == null)
                throw new ArgumentNullException(nameof(email), "Partner email is null");

            var partner = await _context.Partners.Include(p => p.PartnerServices).SingleOrDefaultAsync(p => p.Email == email);
            if (partner == null)
                throw new Exception("Partner not found");

            int totalServices = partner.PartnerServices.Count;

            var completedBookings = await _context.Bookings
                .Where(b => b.BookingLogs.Any(bl => bl.Status == 4))
                .ToListAsync();

            int revenue = 0;
            foreach (var booking in completedBookings)
            {
                var serviceBookings = await _context.ServiceBookings
                    .Where(sb => sb.Detail.BookingId == booking.BookingId)
                    .ToListAsync();

                foreach (var sb in serviceBookings)
                {
                    revenue += sb.Price ?? 0;
                }
            }

            var statDTO = new StatPartnerServiceDTO
            {
                totalServices = totalServices,
                revenue = revenue
            };

            return statDTO;
        }
        private async Task<int> GetPartnerIdAsync(string partnerEmail)
        {
            using var _context = new LumosDBContext();
            return await _context.Partners
                .Where(p => p.Email == partnerEmail)
                .Select(p => p.PartnerId)
                .FirstOrDefaultAsync();
        }

    }
}
