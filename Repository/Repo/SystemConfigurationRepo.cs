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
    public class SystemConfigurationRepo:ISystemConfigurationRepo
    {
        public SystemConfigurationRepo(LumosDBContext context) { }

        public Task<IEnumerable<SystemConfiguration>> SearchSystemConfigByNameAsync(string name) => SystemConfigurationDAO.Instance.SearchSystemConfigByNameAsync(name);
    }
}
