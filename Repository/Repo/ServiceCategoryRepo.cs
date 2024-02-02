using BussinessObject;
using DataAccessLayer;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class ServiceCategoryRepo : IServiceCategoryRepo
    {
        public ServiceCategoryRepo(LumosDBContext context) { }
        public async Task<IEnumerable<ServiceCategory>> GetCategoriesByServiceIdAsync(int serviceId) => await ServiceCategoryDAO.Instance.GetCategoriesOfServiceByServiceIdAsync(serviceId);
        public Task<List<ServiceCategory>> GetCategorysAsync(string? keyword) => ServiceCategoryDAO.Instance.GetCategorysAsync(keyword);
    }
}
