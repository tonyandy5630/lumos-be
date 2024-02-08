using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.InterfaceService
{
    public interface IServiceCategorySer
    {
        Task<List<ServiceCategory>> GetCategorysAsync(string? keyword);
    }
}
