using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IServiceCategoryRepo
    {
        Task<IEnumerable<ServiceCategory>> GetCategoriesByServiceIdAsync (int serviceId);
        Task<ServiceCategory?> GetCategoryByIdAsync(int cateId);

    }
}
