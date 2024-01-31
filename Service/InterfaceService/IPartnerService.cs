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
        Task<ApiResponse<List<Partner>>> GetAllPartnersAsync();
        Task<ApiResponse<Partner>> GetPartnerByIDAsync(int id);
        Task<ApiResponse<Partner>> GetPartnerByRefreshTokenAsync(string token);
        Task<ApiResponse<Partner>> GetPartnerByEmailAsync(string email);
        Task<ApiResponse<Partner>> GetPartnerByCodeAsync(string code);
        Task<ApiResponse<bool>> AddPartnerAsync(Partner partner);
        Task<ApiResponse<bool>> UpdatePartnerAsync(Partner partner);
        Task<ApiResponse<bool>> BanPartnerAsync(int partnerId);
        Task<IEnumerable<SearchPartnerDTO>> SearchPartnerByPartnerOrServiceName(string keyword);
    }
}
