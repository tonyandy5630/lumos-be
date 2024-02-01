using BussinessObject;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IServiceDetailRepo
    {
        Task<EntityEntry<ServiceDetail>> AddServiceDetailAsync(ServiceDetail serviceDetail);
    }
}
