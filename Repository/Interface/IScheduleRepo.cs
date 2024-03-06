using BussinessObject;
using DataTransferObject.DTO;
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
        Task<bool> AddPartnerScheduleAsync(List<Schedule> schedule);
        Task<List<ScheduleDTO>> GetSchedulesByPartnerIdAsync(int partnerId);
    }
}
