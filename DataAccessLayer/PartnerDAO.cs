using BussinessObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<PartnerService?> GetPartnerServiceByIdAsync(int serviceId)
        {
            return await _context.PartnerServices.FirstOrDefaultAsync(s => s.ServiceId == serviceId);
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
            return await _context.Partners.ToListAsync();
        }
        public async Task<Partner> GetPartnerByIDAsync(int id)
        {
            try
            {
                return await _context.Partners.SingleOrDefaultAsync(u => u.PartnerId == id);
            }
            catch (Exception ex)
            {
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
                throw new Exception(ex.Message);
            }
        }
        public async Task<Partner> GetPartnerByCodeAsync(string code)
        {
            try
            {
                return await _context.Partners.SingleOrDefaultAsync(u => u.Code.ToLower().Equals(code.ToLower()));
            }
            catch (Exception ex)
            {
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
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> AddPartnereAsync(Partner partner)
        {
            try
            {
                bool existing = (await GetAllPartnersAsync())
                    .Any(s => s.Code.ToLower().Equals(partner.Code.ToLower()));

                if (!existing)
                {
                    _context.Partners.Add(partner);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Add Partner: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdatePartnerAsync(Partner partner)
        {
            try
            {
                var existing = await _context.Partners.SingleOrDefaultAsync(s => s.PartnerId == partner.PartnerId);
                if (existing != null)
                {
                    existing.Code = partner.Code;
                    existing.PartnerName = partner.PartnerName;
                    existing.Description = partner.Description;
                    existing.Status = partner.Status;

                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    Console.WriteLine("Partner not found for updating.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Update Partner: {ex.Message}", ex);
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
                else
                {
                    Console.WriteLine("Partner does not exist!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Ban Partner: {ex.Message}");
                return false;
            }
        }
    }
}
