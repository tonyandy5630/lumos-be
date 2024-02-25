using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class PartnerServiceDAO
    {
        private static PartnerServiceDAO instance = null;
        private readonly LumosDBContext _context = null;

        public PartnerServiceDAO()
        {
            if (_context == null)
                _context = new LumosDBContext();
        }

        public static PartnerServiceDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PartnerServiceDAO();
                }
                return instance;
            }
        }

        public async Task<PartnerService?> GetServiceOfPartnerByServiceNameAsync(string serviceName, int partnerId)
        {
            return await _context.PartnerServices.FirstOrDefaultAsync(s => s.PartnerId == partnerId && s.Name.ToLower().Contains(serviceName.ToLower()));
        }
        public async Task<IEnumerable<PartnerServiceDTO>> GetTopFiveBookedServicesAsync()
        {
            var result = from sb in _context.ServiceBookings
                         group sb by sb.ServiceId into grouped
                         orderby grouped.Count() descending
                         select new PartnerServiceDTO
                         {
                             ServiceId = (int)grouped.Key,
                             BookedQuantity = grouped.Count(),
                         };

            var topFiveServices = await result.Take(5).ToListAsync();

            var serviceDetails = new List<PartnerServiceDTO>();
            foreach (var service in topFiveServices)
            {
                var serviceDetail = await GetPartnerServiceByIdAsync(service.ServiceId);
                if (serviceDetail != null)
                {
                    serviceDetail.BookedQuantity = service.BookedQuantity;
                    serviceDetails.Add(serviceDetail);
                }
            }

            return serviceDetails;
        }
        public async Task<PartnerServiceDTO?> GetPartnerServiceByIdAsync(int serviceId)
        {
            try
            {
                var result = from ps in _context.PartnerServices
                             join sb in _context.ServiceBookings
                             on ps.ServiceId equals sb.ServiceId
                             group new { ps, sb } by new { ps.ServiceId, ps.Name, ps.Description, ps.Price, ps.Code, ps.Status, ps.CreatedDate, ps.UpdatedBy,ps.Rating, ps.Duration, ps.LastUpdate } into grouped
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
                                 Rating = grouped.Key.Rating,
                                 BookedQuantity = grouped.Count()
                             };

                return await result.FirstOrDefaultAsync(s => s.ServiceId == serviceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerServiceByIdAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

    }
}
