using BussinessObject;
using Repository.Interface.IUnitOfWork;
using Service.InterfaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class SystemConfigurationService:ISystemConfigurationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SystemConfigurationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SystemConfiguration>> SearchSystemConfigByNameAsync(string name)
        {
            try
            {
                List<SystemConfiguration> searchedConfig = (List<SystemConfiguration>) await _unitOfWork.SystemConfigurationRepo.SearchSystemConfigByNameAsync(name.Trim());
                return searchedConfig;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
