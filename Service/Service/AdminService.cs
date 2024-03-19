using BussinessObject;
using DataTransferObject.DTO;
using Enum;
using Microsoft.EntityFrameworkCore;
using Repository.Interface.IUnitOfWork;
using Service.InterfaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.Service
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<BookingDTO> _cachedBookings;
        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<BookingDTO>> GetPartnerBookingsAsync(int? page, int? pageSize)
        {
            try
            {
                if (page == null || pageSize == null)
                {
                    return await _unitOfWork.BookingLogRepo.GetAllBookingDetailsForAdminAsync();
                }
                int pageNumber = page.Value;
                int size = pageSize.Value;
                if (pageNumber < 1 || size < 1)
                {
                    throw new ArgumentException("Page number and page size must be greater than zero.");
                }
                if (_cachedBookings == null)
                {
                    _cachedBookings = await _unitOfWork.BookingLogRepo.GetAllBookingDetailsForAdminAsync();
                }
                _cachedBookings = _cachedBookings.OrderByDescending(b => b.BookingDate).ToList();
                int skipAmount = (pageNumber - 1) * size;

                // Retrieve bookings for the current page
                return (await _unitOfWork.BookingLogRepo.GetAllBookingDetailsForAdminAsync())
                    .Skip(skipAmount)
                    .Take(size)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerBookingsAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<List<Partner>> GetTopPartnerAsync(int top)
        {
            try
            {
                List<Partner> topPartner = await _unitOfWork.PartnerRepo.GetTopPartnerAsync(top);
                return topPartner;
            }catch (Exception ex)
            {
                throw new Exception();
            }
        }
        public async Task<AdminDashboardStat> GetAdminDashboardStatAsync()
        {
            try
            {
                AdminDashboardStat stats = new AdminDashboardStat
                {
                    OnGoingBookings = 0,
                    Earning = 0,
                    TotalBookings = 0,
                    TotalMembers = 0
                };

                List<Booking> bookings = await _unitOfWork.BookingRepo.GetAllAppBookingAsync();
                foreach (Booking booking in bookings)
                {
                    BookingDTO? curBooking = await _unitOfWork.BookingRepo.GetLatestBookingByBookingIdAsync(booking.BookingId);
                    if (curBooking == null)
                        continue;

                    if (curBooking.Status == (int) BookingStatusEnum.Pending)
                        stats.OnGoingBookings++;

                    if (curBooking.Status == (int)BookingStatusEnum.Completed)
                        stats.Earning += (int)curBooking?.TotalPrice;
                }
                stats.TotalMembers = await _unitOfWork.PartnerRepo.CountAppPartnerAsync();
                stats.TotalBookings = await _unitOfWork.BookingRepo.CountBookingInAppAsync();

                return await Task.FromResult(stats);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }

        public async Task<bool> AddAdminAsync(Admin admin)
        {
            try
            {
                return await _unitOfWork.AdminRepo.AddAdminAsync(admin);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> BanAdminAsync(int id)
        {
            try
            {
                return await _unitOfWork.AdminRepo.BanAdminAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Admin> GetAdminByCodeAsync(string code)
        {
            try
            {
                return await _unitOfWork.AdminRepo.GetAdminByCodeAsync(code);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Admin> GetAdminByEmailAsync(string email)
        {
            try
            {
                return await _unitOfWork.AdminRepo.GetAdminByEmailAsync(email);               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Admin> GetAdminByIDAsync(int id)
        {
            try
            {
                return await _unitOfWork.AdminRepo.GetAdminByIDAsync(id);               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Admin> GetAdminByRefreshTokenAsync(string token)
        {
            try
            {
                return await _unitOfWork.AdminRepo.GetAdminByRefreshTokenAsync(token);               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<Admin>> GetAdminsAsync()
        {
            try
            {
                return await _unitOfWork.AdminRepo.GetAdminsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateAdminAsync(Admin admin)
        {
           
            try
            {
                return await _unitOfWork.AdminRepo.UpdateAdminAsync(admin);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<NewUserMonthlyChartDTO> GetAppNewUserMonthlyAsync(int year)
        {
            try
            {
                NewUserMonthlyChartDTO res = new NewUserMonthlyChartDTO
                {
                    newCustomerMonthly = new (),
                    newPartnerMonthly = new ()
                };
                List<ChartStatDTO> customerStats = await _unitOfWork.CustomerRepo.GetNewCustomerMonthlyAsync(year);
                List<ChartStatDTO> partnerStats = await _unitOfWork.PartnerRepo.GetNewPartnerMonthlyAsync(year);

                if (customerStats == null && partnerStats == null)
                {
                    return res;
                }

                for (int month = 1; month <= 12; month++)
                {
                    int? cusMonthStatInDb = customerStats?.FirstOrDefault(t => t.StatUnit == month)?.StatValue;
                    int? partnerMonthStatInDB = partnerStats?.FirstOrDefault(t => t.StatUnit == month)?.StatValue;

                    int? stat = 0;
                    if (cusMonthStatInDb != null)
                    {
                        stat = cusMonthStatInDb;
                    }

                    res.newCustomerMonthly.Add(stat);
                    stat = 0;
                    if (partnerMonthStatInDB != null)
                    {
                        stat = partnerMonthStatInDB;
                    }
                    res.newPartnerMonthly.Add(stat);
                }
                return res;
            }catch(Exception ex)
            {
                throw new Exception();
            }
        }
        public async Task<ListDataDTO> GetAppMonthlyRevenueAsync(int year)
        {
            try
            {

                var res = await _unitOfWork.PartnerRepo.CalculateMonthlyRevenueAsync(year);

                return res;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

    }
}
