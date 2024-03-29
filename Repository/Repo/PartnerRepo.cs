﻿using BussinessObject;
using DataAccessLayer;
using DataTransferObject.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Repository.Repo
{
    public class PartnerRepo : IPartnerRepo
    {
        public PartnerRepo(LumosDBContext context) { }

        public Task<List<Partner>> GetTopPartnerAsync(int top) => PartnerDAO.Instance.GetTopPartnerRatingAsync(top);
        public Task<List<ChartStatDTO>> GetNewPartnerMonthlyAsync(int year) => PartnerDAO.Instance.GetNewPartnerMonthlyAsync(year);
        public Task<Partner?> GetPartnerByBookingIdAsync(int bookingId) => PartnerDAO.Instance.GetPartnerByBookingIdAsync(bookingId);

        public Task<PartnerService?> AddPartnerServiceAsync(PartnerService service) => PartnerDAO.Instance.AddPartnerServiceAsync(service);
        
        public Task<Partner?> AddPartnerAsync(Partner partner) => PartnerDAO.Instance.AddPartnereAsync(partner);

        public Task<bool> BanPartnerAsync(int partnerId) => PartnerDAO.Instance.BanPartnerAsync(partnerId);

        public Task<List<Partner>> GetAllPartnersAsync() => PartnerDAO.Instance.GetAllPartnersAsync();

        public Task<Partner> GetPartnerByCodeAsync(string code) => PartnerDAO.Instance.GetPartnerByCodeAsync(code);

        public Task<Partner?> GetPartnerByEmailAsync(string email) => PartnerDAO.Instance.GetPartnerByEmailAsync(email);

        public Task<Partner?> GetPartnerByIDAsync(int id) => PartnerDAO.Instance.GetPartnerByIDAsync(id);

        public Task<Partner> GetPartnerByRefreshTokenAsync(string token) => PartnerDAO.Instance.GetPartnerByRefreshTokenAsync(token);

        public Task<IEnumerable<PartnerService>> GetPartnerServiceByServiceNameAsync(string serviceName, int partnerId) => PartnerDAO.Instance.GetServiceOfPartnerByServiceName(serviceName, partnerId);

        public Task<PartnerServiceDTO?> GetPartnerServiceDetailByIdAsync(int id) => PartnerDAO.Instance.GetPartnerServiceByIdAsync(id);

        public Task<IEnumerable<Partner>> SearchPartnerByPartnerOrServiceNameAsync(string keyword) => PartnerDAO.Instance.SearchPartnerByServiceOrPartnerNameAsync(keyword);

        public Task<bool> UpdatePartnerAsync(Partner partner) => PartnerDAO.Instance.UpdatePartnerAsync(partner);
        public Task<IEnumerable<Partner>> SearchPartnerByCategoryIdAsync(int categoryId) => PartnerDAO.Instance.GetPartnersByCategoryAsync(categoryId);

        public Task<int?> CalculatePartnerRevenueInMonthAsync(int month, int year) => PartnerDAO.Instance.CalculatePartnerRevenueInMonthAsync(month, year);
        public Task<List<int>> GetRevenuePerWeekInMonthAsync(string email,int month, int year) => PartnerDAO.Instance.GetRevenuePerWeekInMonthAsync(email,month, year);
        public async Task<ListDataDTO> CalculateMonthlyRevenueAsync(int year)
        {
            try
            {
                List<int?> monthlyRevenueList = new List<int?>();

                for (int month = 1; month <= 12; month++)
                {
                    var monthlyRevenue = await CalculatePartnerRevenueInMonthAsync(month, year);
                    monthlyRevenueList.Add((int)monthlyRevenue);
                }

                ListDataDTO result = new ListDataDTO
                {
                    Data = monthlyRevenueList
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CalculateMonthlyRevenueAsync in partnerrepo: {ex.Message}", ex);
                throw;
            }
        }


        public Task<List<PartnerServiceDTO>> GetPartnerServicesWithBookingCountAsync(int partnerId) => PartnerDAO.Instance.GetPartnerServicesWithBookingCountAsync(partnerId);

        public Task<Partner?> GetPartnerByBussinessLicenseAsync(string license) => PartnerDAO.Instance.GetPartnerByBussinessLicenseAsync(license);

        public Task<Partner?> GetPartnerByDisplayNameAsync(string displayName) => PartnerDAO.Instance.GetPartnerByDisplayNameAsync(displayName);

        public Task<Partner?> GetPartnerByPartnerNameAsync(string name) => PartnerDAO.Instance.GetPartnerByPartnerNameAsync(name);

        public Task<StatPartnerServiceDTO> CalculateServicesAndRevenueAsync(string? email) => PartnerDAO.Instance.CalculateServicesAndRevenueAsync(email);

        public Task<int> CountAppPartnerAsync() => PartnerDAO.Instance.CountAppPartnerAsync();

    }
}
