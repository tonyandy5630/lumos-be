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
        Task<List<BookingforAdminDTO>> GetBookingsAsync(int? page, int? pageSize);
        Task<List<PartnerDTO>> GetAllPartnersAsync(int? page, int? pageSize);
        Task<List<Partner>> GetTopPartnerAsync(int top);
        Task<List<Admin>> GetAdminsAsync();
        Task<Admin> GetAdminByEmailAsync(string email);
        Task<Admin> GetAdminByRefreshTokenAsync(string token);
        Task<Admin> GetAdminByIDAsync(int id);
        Task<Admin> GetAdminByCodeAsync(string code);
        Task<bool> AddAdminAsync(Admin admin);
        Task<bool> UpdateAdminAsync(Admin admin);
        Task<bool> BanAdminAsync(int id);
        Task<AdminDashboardStat> GetAdminDashboardStatAsync();
        Task<NewUserMonthlyChartDTO> GetAppNewUserMonthlyAsync(int year);
        Task<ListDataDTO> GetAppMonthlyRevenueAsync(int year);
    }
}
