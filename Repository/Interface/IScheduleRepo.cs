using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IScheduleRepo
    {
        Task<List<Schedule>> GetScheduleByPartnerIdAsyn(int id);
        Task<Schedule> AddPartnerScheduleAsync(Schedule schedule);
    }
}
