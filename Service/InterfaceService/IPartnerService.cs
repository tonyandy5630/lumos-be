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
        Task<(bool, string)> UpdatePartnerServiceAsync(UpdatePartnerServiceRequest request, int id);
        Task<bool> DeletePartnerServiceAsync(int id);
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
        Task<IEnumerable<SearchPartnerDTO>> SearchPartnerByPartnerOrServiceNameAsync(string keyword);
        Task<(PartnerService?, PartnerServiceError?)> AddPartnerServiceAsync(AddPartnerServiceResquest service, string? partnerEmail);
        Task<IEnumerable<SearchPartnerDTO>> GetPartnerByCategoryAsync(int categoryId);
        Task<IEnumerable<PartnerServiceDTO>> GetTopFiveBookedServicesAsync();
        Task<ListRevenueDTO> CalculatePartnerRevenueInMonthAsync(string email, int month, int year);
        Task<List<PartnerServiceDTO>> GetPartnerServicesWithBookingCountAsync(string email);
        Task<StatPartnerServiceDTO> GetStatPartnerServiceAsync (string email);
        Task<StatPartnerServiceDTO> CalculateServicesAndRevenueAsync(string? email);
        Task<List<BookingDTO>> GetPartnerBookingsAsync(string partnerEmail, int page, int pageSize);
    }
}
