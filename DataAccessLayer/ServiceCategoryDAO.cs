using BussinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ServiceCategoryDAO
    {
        private static ServiceCategoryDAO instance = null;
        private LumosDBContext _context;



        public ServiceCategoryDAO()
        {
            _context = new LumosDBContext();
        }

        public static ServiceCategoryDAO Instance
        {
            _context = context;
        }
        //public static ServiceCategoryDAO Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = new ServiceCategoryDAO();
        //        }
        //        return instance;
        //    }
        //}

        public async Task<IEnumerable<ServiceCategory>> GetCategoriesOfServiceByServiceIdAsync(int serviceId)
        {
            var query = from cat in _context.ServiceCategories
                        where cat.ServiceDetails.Any(c => c.ServiceId == serviceId)
                        select cat;
            return await query.ToListAsync();
        }


        public async Task<ServiceCategory?> GetCategoryByIdAsync(int cateId)
        {
            return await _context.ServiceCategories.FirstOrDefaultAsync(c => c.CategoryId == cateId);
        }


        public async Task<List<ServiceCategory>> GetCategorysAsync(string? keyword)
        {
            List<ServiceCategory> categories = new List<ServiceCategory>();
            try
            {
                if (keyword != null)
                {
                    categories = await _context.ServiceCategories.Where(s => s.Category.ToLower().Contains(keyword.ToLower())).ToListAsync();
                }
                else
                {
                    categories = await _context.ServiceCategories.ToListAsync();
                }
                Console.WriteLine("GetCategorysAsync: " + categories.Count);
                return categories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCategorysAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
