using BussinessObject;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ServiceDetailDAO
    {
        private static ServiceDetailDAO instance = null;
        private LumosDBContext _context = null;

        public ServiceDetailDAO()
        {
            if (_context == null)
                _context = new LumosDBContext();
        }

        public static ServiceDetailDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceDetailDAO();
                }
                return instance;
            }
        }

        public async Task<ServiceDetail?> AddServiceDetailAsync(ServiceDetail serviceDetail)
        {
             await _context.ServiceDetails.AddAsync(serviceDetail);
            if (await _context.SaveChangesAsync() == 1)
                return serviceDetail;
            return null;
        }
    }
}
