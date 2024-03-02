using BussinessObject;
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


        public Task<List<ChartStatDTO>> GetNewPartnerMonthlyAsync(int year) => PartnerDAO.Instance.GetNewPartnerMonthlyAsync(year);
        public Task<Partner?> GetPartnerByBookingIdAsync(int bookingId) => PartnerDAO.Instance.GetPartnerByBookingIdAsync(bookingId);

        public Task<PartnerService?> AddPartnerServiceAsync(PartnerService service) => PartnerDAO.Instance.AddPartnerServiceAsync(service);
        
        public Task<Partner?> AddPartnereAsync(Partner partner) => PartnerDAO.Instance.AddPartnereAsync(partner);

        public Task<bool> BanPartnerAsync(int partnerId) => PartnerDAO.Instance.BanPartnerAsync(partnerId);

        public Task<List<Partner>> GetAllPartnersAsync() => PartnerDAO.Instance.GetAllPartnersAsync();

        public Task<Partner> GetPartnerByCodeAsync(string code) => PartnerDAO.Instance.GetPartnerByCodeAsync(code);

        public Task<Partner?> GetPartnerByEmailAsync(string email) => PartnerDAO.Instance.GetPartnerByEmailAsync(email);

        public Task<Partner> GetPartnerByIDAsync(int id) => PartnerDAO.Instance.GetPartnerByIDAsync(id);

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

        public async Task<List<BookingDTO>> GetPartnerBookingsAsync(string partnerEmail, int page, int pageSize)
        {
            try
            {
                var partner = await GetPartnerByEmailAsync(partnerEmail);
                if (partner == null)
                {
                    return new List<BookingDTO>();
                }

                var allBookingLogs = await BookingLogDAO.Instance.GetAllPendingBookingLogsAsync();
                var pendingBookings = BookingLogDAO.Instance.GroupPendingBookings(allBookingLogs);
                var skipAmount = (page - 1) * pageSize;
                var pendingBookingsForPage = pendingBookings.Skip(skipAmount).Take(pageSize).ToList();
                var result = await FilterAndMapBookingsAsync(pendingBookingsForPage, partner);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerBookingsAsync: {ex.Message}", ex);
                throw;
            }
        }

        public Task<int> CountAppPartnerAsync() => PartnerDAO.Instance.CountAppPartnerAsync();

        public async Task<List<BookingDTO>> FilterAndMapBookingsAsync(List<IGrouping<int, BookingLog>> pendingBookings, Partner partner)
        {
            var result = new List<BookingDTO>();

            foreach (var group in pendingBookings)
            {
                var bookingId = group.Key;
                var statuses = group.Select(bl => bl.Status).Distinct().ToList();
                var bookingDetail = await BookingDAO.Instance.GetBookingDetailByIdAsync(bookingId);
                if (bookingDetail == null)
                {
                    continue;
                }
                var reportId = bookingDetail.ReportId;
                foreach (var status in statuses)
                {
                    var medicalServiceDTOs = await BookingLogDAO.Instance.GetMedicalServiceDTOsAsync(bookingId);

                    result.Add(new BookingDTO
                    {
                        BookingId = bookingId,
                        Status = EnumUtils.GetBookingEnumByStatus(status),
                        Partner = partner.DisplayName,
                        BookingDate = await BookingLogDAO.Instance.GetBookingDateAsync(bookingId),
                        bookingTime = (int)await BookingLogDAO.Instance.GetBookingTimeAsync(bookingId),
                        Address = await BookingLogDAO.Instance.GetBookingAddressAsync(bookingId),
                        PaymentMethod = await BookingLogDAO.Instance.GetPaymentMethodAsync(bookingId),
                        Customer = await BookingDAO.Instance.GetCustomerByReportIdAsync(reportId),
                        MedicalServices = medicalServiceDTOs
                    });
                }
            }

            return result;
        }
    }
}
