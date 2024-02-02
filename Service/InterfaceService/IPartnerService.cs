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
    public interface IPartnerService
    {
        Task<PartnerServiceDTO?> GetPartnerServiceDetailAsync(int serviceId);
        Task<List<Partner>> GetAllPartnersAsync();
        Task<Partner> GetPartnerByIDAsync(int id);
        Task<Partner> GetPartnerByRefreshTokenAsync(string token);
        Task<Partner> GetPartnerByEmailAsync(string email);
        Task<Partner> GetPartnerByCodeAsync(string code);
        Task<bool> AddPartnerAsync(Partner partner);
        Task<bool> UpdatePartnerAsync(Partner partner);
        Task<bool> BanPartnerAsync(int partnerId);
        Task<IEnumerable<SearchPartnerDTO>> SearchPartnerByPartnerOrServiceName(string keyword);
        Task<List<Schedule>> GetScheduleByPartnerIdAsyn(int id);
        Task<List<PartnerType>> GetPartnerTypesAsync(string? keyword);
    }
}
