using BussinessObject;
using DataAccessLayer;
using DataTransferObject.DTO;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class PartnerRepo : IPartnerRepo
    {
        public PartnerRepo(LumosDBContext context) { }

        public Task<PartnerService?> AddPartnerServiceAsync(PartnerService service) => PartnerDAO.Instance.AddPartnerServiceAsync(service);
        
        public Task<Partner> AddPartnereAsync(Partner partner) => PartnerDAO.Instance.AddPartnereAsync(partner);

        public Task<bool> BanPartnerAsync(int partnerId) => PartnerDAO.Instance.BanPartnerAsync(partnerId);

        public Task<List<Partner>> GetAllPartnersAsync() => PartnerDAO.Instance.GetAllPartnersAsync();

        public Task<Partner> GetPartnerByCodeAsync(string code) => PartnerDAO.Instance.GetPartnerByCodeAsync(code);

        public Task<Partner> GetPartnerByEmailAsync(string email) => PartnerDAO.Instance.GetPartnerByEmailAsync(email);

        public Task<Partner> GetPartnerByIDAsync(int id) => PartnerDAO.Instance.GetPartnerByIDAsync(id);

        public Task<Partner> GetPartnerByRefreshTokenAsync(string token) => PartnerDAO.Instance.GetPartnerByRefreshTokenAsync(token);

        public Task<IEnumerable<PartnerService>> GetPartnerServiceByServiceNameAsync(string serviceName, int partnerId) => PartnerDAO.Instance.GetServiceOfPartnerByServiceName(serviceName, partnerId);

        public Task<PartnerServiceDTO?> GetPartnerServiceDetailByIdAsync(int id) => PartnerDAO.Instance.GetPartnerServiceByIdAsync(id);

        public Task<IEnumerable<Partner>> SearchPartnerByPartnerOrServiceNameAsync(string keyword) => PartnerDAO.Instance.SearchPartnerByServiceOrPartnerNameAsync(keyword);

        public Task<bool> UpdatePartnerAsync(Partner partner) => PartnerDAO.Instance.UpdatePartnerAsync(partner);
        public Task<IEnumerable<Partner>> SearchPartnerByCategoryIdAsync(int categoryId) => PartnerDAO.Instance.GetPartnersByCategoryAsync(categoryId);

        public Task<List<RevenuePerWeekDTO>> CalculatePartnerRevenueInMonthAsync(int month,int year) => PartnerDAO.Instance.CalculatePartnerRevenueInMonthAsync(month, year);
        public Task<List<MonthlyRevenueDTO>> CalculateMonthlyRevenueAsync(int year) => PartnerDAO.Instance.CalculateMonthlyRevenueAsync(year);

        public Task<List<PartnerServiceDTO>> GetPartnerServicesWithBookingCountAsync(int partnerId) => PartnerDAO.Instance.GetPartnerServicesWithBookingCountAsync(partnerId);  
    }
}
