using BussinessObject;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class ServiceDetailRepo:IServiceDetailRepo
    {
        public ServiceDetailRepo(LumosDBContext context) { }

        public Task<EntityEntry<ServiceDetail>> AddServiceDetailAsync(ServiceDetail serviceDetail) => ServiceDetailDAO.Instance.AddServiceDetailAsync(serviceDetail);
    }
}
