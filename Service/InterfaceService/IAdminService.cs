using BussinessObject;
using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.InterfaceService
{
    public interface IAdminService
    {
        Task<List<Admin>> GetAdminsAsync();
        Task<Admin> GetAdminByEmailAsync(string email);
        Task<Admin> GetAdminByRefreshTokenAsync(string token);
        Task<Admin> GetAdminByIDAsync(int id);
        Task<Admin> GetAdminByCodeAsync(string code);
        Task<bool> AddAdminAsync(Admin admin);
        Task<bool> UpdateAdminAsync(Admin admin);
        Task<bool> BanAdminAsync(int id);
        Task<AdminDashboardStat> GetAdminDashboardStatAsync();
    }
}
