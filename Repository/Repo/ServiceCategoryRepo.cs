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
    public class ServiceCategoryRepo:IServiceCategoryRepo
    {
        public ServiceCategoryRepo(LumosDBContext context) { }

        public Task<IEnumerable<ServiceCategory>> GetCategoriesByServiceIdAsync(int serviceId) =>  ServiceCategoryDAO.Instance.GetCategoriesOfServiceByServiceIdAsync(serviceId);

        public Task<ServiceCategory?> GetCategoryByIdAsync(int cateId) =>  ServiceCategoryDAO.Instance.GetCategoryByIdAsync(cateId);
    }
}
