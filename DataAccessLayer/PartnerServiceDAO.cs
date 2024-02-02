using BussinessObject;
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
    }
}
