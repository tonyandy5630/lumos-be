using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.InterfaceService
{
    public interface ISystemConfigurationService
    {
        Task<IEnumerable<SystemConfiguration>> SearchSystemConfigByNameAsync(string name);
        Task<SystemConfiguration?> GetSystemConfigurationDetailById(int id);
    }
}
