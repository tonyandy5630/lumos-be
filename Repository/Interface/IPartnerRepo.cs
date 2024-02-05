using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IPartnerRepo
    {
        Task<PartnerServiceDTO?> GetPartnerServiceDetailByIdAsync(int id);
        Task<List<Partner>> GetAllPartnersAsync();
        Task<Partner> GetPartnerByIDAsync(int id);
        Task<Partner> GetPartnerByRefreshTokenAsync(string token);
        Task<Partner> GetPartnerByEmailAsync(string email);
        Task<Partner> GetPartnerByCodeAsync(string code);
        Task<Partner> AddPartnereAsync(Partner partner);
        Task<bool> UpdatePartnerAsync(Partner partner);
        Task<bool> BanPartnerAsync(int partnerId);

        Task<IEnumerable<Partner>> SearchPartnerByPartnerOrServiceNameAsync(string keyword);

        Task<IEnumerable<PartnerService>> GetPartnerServiceByServiceNameAsync(string serviceName, int partnerId);
        Task<PartnerService?> AddPartnerServiceAsync(PartnerService service);
    }
}
