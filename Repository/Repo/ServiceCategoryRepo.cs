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
        private readonly ServiceCategoryDAO serviceCategoryDAO;
        public ServiceCategoryRepo(LumosDBContext context) {
            serviceCategoryDAO = new ServiceCategoryDAO(context);
        }

        public Task<IEnumerable<ServiceCategory>> GetCategoriesByServiceIdAsync(int serviceId) => serviceCategoryDAO.GetCategoriesOfServiceByServiceIdAsync(serviceId);

        public Task<ServiceCategory?> GetCategoryByIdAsync(int cateId) =>  serviceCategoryDAO.GetCategoryByIdAsync(cateId);
    }
}
