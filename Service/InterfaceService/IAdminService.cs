using BussinessObject;
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
        Task<ApiResponse<List<Admin>>> GetAdminsAsync();
        Task<ApiResponse<Admin>> GetAdminByEmailAsync(string email);
        Task<ApiResponse<Admin>> GetAdminByRefreshTokenAsync(string token);
        Task<ApiResponse<Admin>> GetAdminByIDAsync(int id);
        Task<ApiResponse<Admin>> GetAdminByCodeAsync(string code);
        Task<ApiResponse<bool>> AddAdminAsync(Admin admin);
        Task<ApiResponse<bool>> UpdateAdminAsync(Admin admin);
        Task<ApiResponse<bool>> BanAdminAsync(int id);
    }
}
