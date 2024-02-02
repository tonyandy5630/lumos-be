using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class PartnerTypeDAO
    {
        private static PartnerTypeDAO instance = null;
        private LumosDBContext _context = null;

        public PartnerTypeDAO()
        {
            if (_context == null)
                _context = new LumosDBContext();
        }

        public static PartnerTypeDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PartnerTypeDAO();
                }
                return instance;
            }
        }

        public async Task<List<PartnerType>> GetPartnerTypesAsync(string? keyword)
        {
            try
            {
                List<PartnerType> partnerTypes = new List<PartnerType>();
                if (keyword != null)
                {
                    partnerTypes = _context.PartnerTypes.Where(s => s.Name.ToLower().Contains(keyword.ToLower())).ToList();
                }
                else
                {
                    partnerTypes = _context.PartnerTypes.ToList();
                }
                Console.WriteLine("GetPartnerTypesAsync: " + partnerTypes.Count);
                return partnerTypes;
            } catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerTypesAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
