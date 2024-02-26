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
        private readonly LumosDBContext _context = null;

        public PartnerDAO()
        {
            if (_context == null)
                _context = new LumosDBContext();
        }

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

        public async Task<PartnerServiceDTO?> GetPartnerServiceByIdAsync(int serviceId)
        {
            try
            {

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
            await _context.PartnerServices.AddAsync(service);

            if (await _context.SaveChangesAsync() == 1)
                return service;
            return null;
        }

        public async Task<IEnumerable<Partner>> SearchPartnerByServiceOrPartnerNameAsync(string keyword)
        {
            var result = _context.Partners
                        .Where(p => p.PartnerName.Contains(keyword) || p.PartnerServices.Any(s => s.Name.Contains(keyword)))
                        .Include(s => s.PartnerServices
                        .Where(s => s.Name.Contains(keyword)))
                        .AsNoTracking()
                        .ToListAsync();
            return await result;
        }

        public async Task<IEnumerable<PartnerService>> GetServiceOfPartnerByServiceName(string keyword, int partnerId)
        {
            return await _context.PartnerServices.Where(s => s.PartnerId == partnerId && s.Name.Contains(keyword)).ToListAsync();
        }
        public async Task<List<Partner>> GetAllPartnersAsync()
        {
            List<Partner> partners = new List<Partner>();
            try
            {
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
                return await _context.Partners.SingleOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerByEmailAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<Partner> AddPartnereAsync(Partner partner)
        {
            try
            {
                bool existing = (await GetAllPartnersAsync())
                    .Any(p => p.PartnerName.ToLower().Equals(partner.PartnerName.ToLower())
                        || p.DisplayName.ToLower().Equals(partner.DisplayName.ToLower())
                        || p.Email.ToLower().Equals(partner.Email.ToLower())
                        || p.BusinessLicenseNumber.ToLower().Equals(partner.BusinessLicenseNumber.ToLower()));

                if (!existing)
                {
                    partner.Code = GenerateCode.GenerateRoleCode("partner");
                    partner.CreatedDate = DateTime.Now;
                    partner.LastUpdate = DateTime.Now;
                    _context.Partners.Add(partner);
                    await _context.SaveChangesAsync();

                    return await _context.Partners.SingleOrDefaultAsync(p => p.Code.Equals(partner.Code));
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPartnereAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdatePartnerAsync(Partner partner)
        {
            try
            {
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
                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                var revenuePerWeek = await _context.ServiceBookings
                    .Where(sb => sb.CreatedDate >= startDate && sb.CreatedDate <= endDate)
                    .GroupBy(sb => ((sb.CreatedDate.Value.Day - 1) / 7) + 1)
                    .OrderBy(g => g.Key)
                    .Select(g => new RevenuePerWeekDTO
                    {
                        WeekNumber = g.Key,
                        Revenue = g.Sum(sb => sb.Price ?? 0),
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

    }
}
