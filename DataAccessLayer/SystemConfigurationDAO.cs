using BussinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class SystemConfigurationDAO
    {
        private static SystemConfigurationDAO instance = null;
        private LumosDBContext _context = null;

        public SystemConfigurationDAO()
        {
            if (_context == null)
                _context = new LumosDBContext();
        }

        public static SystemConfigurationDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SystemConfigurationDAO();
                }
                return instance;
            }
        }

        public async Task<IEnumerable<SystemConfiguration>> SearchSystemConfigByNameAsync(string config)
        {
            return await _context.SystemConfigurations.Where(s => s.Field.ToLower().Contains(config.ToLower())).ToListAsync();
        }
    }
}
