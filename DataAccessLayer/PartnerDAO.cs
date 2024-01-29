using BussinessObject;
using Microsoft.EntityFrameworkCore;
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
        private LumosDBContext _context = null;
        public static PartnerDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PartnerDAO();
                    instance._context = new LumosDBContext();
                }
                return instance;
            }
        }

        public async Task<PartnerService?> GetPartnerServiceByIdAsync(int serviceId)
        {
            return await _context.PartnerServices.Include(s => s.ServiceDetails).SingleOrDefaultAsync(s => s.ServiceId == serviceId );
        }

    }
}
