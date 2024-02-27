using BussinessObject;
using DataTransferObject.DTO;
using RequestEntity;
using Service.ErrorObject;
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
        Task<SearchPartnerDTO> GetPartnerByIDAsync(int id);
        Task<Partner> GetPartnerByRefreshTokenAsync(string token);
        Task<Partner> GetPartnerByEmailAsync(string email);
        Task<Partner> GetPartnerByCodeAsync(string code);
        Task<(Partner?, PartnerError?)>  AddPartnerAsync(AddPartnerRequest partner);
        Task<bool> UpdatePartnerAsync(Partner partner);
        Task<bool> BanPartnerAsync(int partnerId);
        Task<List<Schedule>> GetScheduleByPartnerIdAsyn(int id);
        Task<List<PartnerType>> GetPartnerTypesAsync(string? keyword);
        Task<Schedule> AddPartnerScheduleAsync(Schedule schedule);
        Task<IEnumerable<SearchPartnerDTO>> SearchPartnerByPartnerOrServiceNameAsync(string keyword);
        Task<PartnerService> AddPartnerServiceAsync(AddPartnerServiceResquest service, string? partnerEmail);
        Task<IEnumerable<SearchPartnerDTO>> GetPartnerByCategoryAsync(int categoryId);
        Task<IEnumerable<PartnerServiceDTO>> GetTopFiveBookedServicesAsync();
        Task<List<RevenuePerWeekDTO>> CalculatePartnerRevenueInMonthAsync(int month, int year);
        Task<List<MonthlyRevenueDTO>> CalculateMonthlyRevenueAsync(int year);
        Task<List<PartnerServiceDTO>> GetPartnerServicesWithBookingCountAsync(string email);
        Task<StatPartnerServiceDTO> GetStatPartnerServiceAsync (string email);

    }
}
