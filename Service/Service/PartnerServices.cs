﻿using AutoMapper;
using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Repository.Interface.IUnitOfWork;
using Repository.Repo;
using Service.InterfaceService;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestEntity;
using Utils;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interface;
using Service.ErrorObject;
using static Utils.MessagesResponse;
using DataAccessLayer;
using Enum;
using System.Transactions;

namespace Service.Service
{
    public class PartnerServices : IPartnerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private List<BookingDTO> _filteredBookings;
        public PartnerServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(PartnerService?, PartnerServiceError?)> AddPartnerServiceAsync(AddPartnerServiceResquest service, string? partnerEmail)
        {
            try
            {
                    List<Task<ServiceCategory>> categoryTasks = new(); // parallel category validation
                    List<Task<ServiceDetail?>> serviceDetailTasks = new(); // add service detail parallel
                    List<ServiceCategory> categories = new();

                    if (partnerEmail == null)
                        throw new Exception("Not found partner email");

                    Partner? partner = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(partnerEmail);

                    if (partner == null)
                        throw new Exception("Not found partner");

                    PartnerService? existedService = await _unitOfWork.PartnerServiceRepo.GetPartnerServiceByServiceNameAsync(service.Name, partner.PartnerId);
                    if (existedService != null)
                    {
                        PartnerServiceError error = new()
                        {
                            Name = "Service with same name has existed"
                        };
                        return (null, error);
                    };

                    PartnerService partnerService = _mapper.Map<PartnerService>(service);
                    partnerService.PartnerId = partner.PartnerId;
                    partnerService.Code = GenerateCode.GenerateTableCode("partnerservice");
                    partnerService.Status = 1;
                    partnerService.CreatedDate = DateTime.Now;
                    partnerService.LastUpdate = DateTime.Now;
                    partnerService.Rating = 0;
                    partnerService.UpdatedBy = partner.PartnerName;

                    // Validate categories
                    // Better if can be done concurrently
                    foreach (int cateId in service.Categories)
                    {
                        ServiceCategory? cate = await _unitOfWork.ServiceCategoryRepo.GetCategoryByIdAsync(cateId);
                        categories.Add(cate);
                    }

                    if (categories.Count != service.Categories.ToList().Count)
                        throw new Exception("Categories do not match");
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    PartnerService? newService = await _unitOfWork.PartnerRepo.AddPartnerServiceAsync(partnerService);

                    if (newService == null)
                        throw new Exception("Failed to add service");

                    foreach (var cate in categories)
                    {
                        if (cate == null)
                            throw new Exception("One of the categories is not available");

                        ServiceDetail serviceDetail = new ServiceDetail
                        {
                            CreatedDate = DateTime.Now,
                            ServiceId = newService.ServiceId,
                            CategoryId = cate.CategoryId,
                            LastUpdate = DateTime.Now,
                            CreatedBy = partner.PartnerName,
                        };

                        ServiceDetail? detail = await _unitOfWork.ServiceDetailRepo.AddServiceDetailAsync(serviceDetail);
                        if (detail == null)
                            throw new Exception("Error when adding one of the details");
                    }

                    scope.Complete();
                    return (newService, null);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<bool> AddPartnerScheduleAsync(List<ScheduleRequest> schedules, int partnerId)
        {

            try
            {
                if (schedules.Count == 0)
                    return false;
                Partner? partner = await _unitOfWork.PartnerRepo.GetPartnerByIDAsync(partnerId);

                if (partner == null)
                    throw new NullReferenceException("Partner is banned or deleted");

                List<Schedule> addSchedules = _mapper.Map<List<Schedule>>(schedules);

                HashSet<Schedule> uniqueSet = new HashSet<Schedule>();
                HashSet<Schedule> duplicatesSet = new HashSet<Schedule>();

                foreach (Schedule schedule in addSchedules)
                {
                    if (schedule.WorkShift < 1 || schedule.WorkShift > 3)
                        throw new NotSupportedException("Work shift is not supported yet");

                    if (schedule.DayOfWeek < 2 || schedule.DayOfWeek > 8)
                        throw new NotSupportedException("Day of week is invalid");

                    // uniqueSet return false on duplicates
                    if (!uniqueSet.Add(schedule))
                    {
                        continue;
                    }

                    schedule.PartnerId = partner.PartnerId;
                    schedule.Code = GenerateCode.GenerateTableCode("schedule");
                    schedule.CreatedDate = DateTime.Now;
                    schedule.LastUpdate = DateTime.Now;
                }

                bool addedSchedule = await _unitOfWork.ScheduleRepo.AddPartnerScheduleAsync(uniqueSet.ToList());
                return addedSchedule;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPartnerScheduleAsync: {ex.Message}", ex);
                return false;
            }
            
        }

        public async Task<PartnerServiceDTO?> GetPartnerServiceDetailAsync(int serviceId)
        {
            try
            {
                PartnerServiceDTO? service = await _unitOfWork.PartnerRepo.GetPartnerServiceDetailByIdAsync(serviceId);
                IEnumerable<ServiceCategory> serviceCategories = await _unitOfWork.ServiceCategoryRepo.GetCategoriesByServiceIdAsync(serviceId);

                if (service == null)
                    return null;

                IEnumerable<ServiceCategoryDTO> serviceCategoryDTOs = _mapper.Map<IEnumerable<ServiceCategoryDTO>>(serviceCategories);
                service.Categories = serviceCategoryDTOs;
                return service;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<(Partner?, PartnerError?)> AddPartnerAsync(AddPartnerRequest partnerRequest)
        {
            try
            {
                bool hasError = false;
                    PartnerError? errorPartner = null;

                    Task<Partner?> existedPartnerName = _unitOfWork.PartnerRepo.GetPartnerByPartnerNameAsync(partnerRequest.Partner.PartnerName.Trim());
                    Task<Partner?> existedLicense = _unitOfWork.PartnerRepo.GetPartnerByBussinessLicenseAsync(partnerRequest.Partner.BusinessLicenseNumber.Trim());
                    Task<Partner?> existedDisplayName = _unitOfWork.PartnerRepo.GetPartnerByDisplayNameAsync(partnerRequest.Partner.DisplayName.Trim());
                    Task<Partner?> existedEmail = _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(partnerRequest.Partner.Email.Trim());

                    await Task.WhenAll(existedPartnerName, existedLicense, existedDisplayName, existedEmail);

                    bool partnerNameError = existedPartnerName.Result != null;
                    bool licenseError = existedLicense.Result != null;
                    bool displayNameError = existedDisplayName.Result != null;
                    bool emailError = existedEmail.Result != null;
                    bool scheduleNotExist = partnerRequest.Schedules == null;

                    hasError = partnerNameError || licenseError || displayNameError || emailError || scheduleNotExist;
                    if (hasError)
                    {
                        errorPartner = new PartnerError();
                        if (partnerNameError)
                            errorPartner.PartnerName = "Existed Partner Name";

                        if (licenseError)
                            errorPartner.BusinessLicenseNumber = "Existed License";

                        if (displayNameError)
                            errorPartner.DisplayName = "Existed Display Name";

                        if (emailError)
                            errorPartner.Email = "Existed Email";

                        if (scheduleNotExist)
                            errorPartner.Schedule = "Schedule is missing";

                        return (null, errorPartner);
                    }

                    PartnerType? partnerType = await _unitOfWork.PartnerTypeRepo.GetPartnerTypeByIdAsync(partnerRequest.Partner.TypeId);

                    if (partnerType == null)
                        throw new NullReferenceException("Partner Type is not existed");

                    Partner addPartner = _mapper.Map<Partner>(partnerRequest.Partner);
                    addPartner.Code = GenerateCode.GenerateRoleCode("partner");
                    addPartner.CreatedDate = DateTime.Now;
                    // Hash password
                    IUserManagerRepo<PartnerRequest> userManager = new UserManagerRepo<PartnerRequest>();
                    addPartner.Password = userManager.HashPassword(partnerRequest.Partner, partnerRequest.Partner.Email.Trim());
                    addPartner.Role = (int)RolesEnum.Partner;
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Partner? part = await _unitOfWork.PartnerRepo.AddPartnerAsync(addPartner);

                    if (part == null)
                    {
                        Console.WriteLine("Failed to add partner!");
                        throw new Exception("Something went wrong when adding partner");
                    }

                    bool schedules = await AddPartnerScheduleAsync(partnerRequest.Schedules, part.PartnerId);
                    if (!schedules)
                    {
                        throw new NullReferenceException("Cannot add schedules");
                    }
                    if (!hasError)
                    {
                        scope.Complete();
                    }
                    return (part, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> BanPartnerAsync(int partnerId)
        {
            try
            {
                return await _unitOfWork.PartnerRepo.BanPartnerAsync(partnerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(MessagesResponse.Error.OperationFailed);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Partner> GetPartnerByCodeAsync(string code)
        {
            try
            {
                Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByCodeAsync(code);
                if (partner == null)
                {
                    Console.WriteLine(MessagesResponse.Error.NotFound);
                }
                return partner;
            }
            catch (Exception ex)
            {
                Console.WriteLine(MessagesResponse.Error.OperationFailed);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Partner> GetPartnerByEmailAsync(string email)
        {
            try
            {
                Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(email);
                if (partner == null)
                {
                    Console.WriteLine(MessagesResponse.Error.NotFound);
                }
                return partner;
            }
            catch (Exception ex)
            {
                Console.WriteLine(MessagesResponse.Error.OperationFailed);
                throw new Exception(ex.Message);
            }
        }

        public async Task<SearchPartnerDTO> GetPartnerByIDAsync(int id)
        {
            try
            {
                Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByIDAsync(id);
                if (partner == null)
                {
                    Console.WriteLine(MessagesResponse.Error.NotFound);
                }
                SearchPartnerDTO partnerDTO = _mapper.Map<SearchPartnerDTO>(partner);

                List<PartnerServiceDTO> partnerServices = new List<PartnerServiceDTO>();
                foreach (var service in partner.PartnerServices)
                {
                    PartnerServiceDTO serviceDTO = await GetPartnerServiceDetailAsync(service.ServiceId);
                    partnerServices.Add(serviceDTO);

                }

                partnerDTO.PartnerServices = partnerServices;
                List<ScheduleDTO> schedules = await _unitOfWork.ScheduleRepo.GetSchedulesByPartnerIdAsync(id);

                partnerDTO.Schedules = schedules;

                return partnerDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(MessagesResponse.Error.OperationFailed);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Partner>> GetAllPartnersAsync()
        {
            try
            {
                List<Partner> partners = await _unitOfWork.PartnerRepo.GetAllPartnersAsync();
                if (partners == null || partners.Count == 0)
                {
                    Console.WriteLine(MessagesResponse.Error.NotFound);
                }
                return partners;
            }
            catch (Exception ex)
            {
                Console.WriteLine(MessagesResponse.Error.OperationFailed);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdatePartnerAsync(Partner partner)
        {
            try
            {
                return await _unitOfWork.PartnerRepo.UpdatePartnerAsync(partner);
            }
            catch (Exception ex)
            {
                Console.WriteLine(MessagesResponse.Error.OperationFailed);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Partner> GetPartnerByRefreshTokenAsync(string token)
        {
            try
            {
                Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByRefreshTokenAsync(token);
                if (partner == null)
                {
                    Console.WriteLine(MessagesResponse.Error.NotFound);
                }
                return partner;
            }
            catch (Exception ex)
            {
                Console.WriteLine(MessagesResponse.Error.OperationFailed);
                throw new Exception(ex.Message);
            }
        }
        public async Task<IEnumerable<SearchPartnerDTO>> GetPartnerByCategoryAsync(int categoryId)
        {
            try
            {

                IEnumerable<Partner> searchedPartner = await _unitOfWork.PartnerRepo.SearchPartnerByCategoryIdAsync(categoryId);
                IEnumerable<SearchPartnerDTO> results = _mapper.Map<IEnumerable<SearchPartnerDTO>>(searchedPartner);

                foreach (var partner in results)
                {
                    List<PartnerServiceDTO?> serviceDetails = new List<PartnerServiceDTO?>();
                    foreach (var service in partner.PartnerServices)
                    {
                        if (service != null)
                        {
                            var sd = await GetPartnerServiceDetailAsync(service.ServiceId);
                            serviceDetails.Add(sd);
                        }
                    }
                    partner.PartnerServices = serviceDetails.Where(result => result != null).ToList();
                }
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerByCategoryAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<IEnumerable<SearchPartnerDTO>> SearchPartnerByPartnerOrServiceNameAsync(string keyword)
        {
            try
            {
                IEnumerable<Partner> searchedPartner = await _unitOfWork.PartnerRepo.SearchPartnerByPartnerOrServiceNameAsync(keyword.Trim());
                if (searchedPartner == null)
                {
                    throw new Exception("Partner not found.");
                }
                IEnumerable<SearchPartnerDTO> results = _mapper.Map<IEnumerable<SearchPartnerDTO>>(searchedPartner);
                List<PartnerServiceDTO?> serviceDetails = new List<PartnerServiceDTO?>();
                foreach (var partner in results)
                {
                    // empty old  partner servicelist for new partner
                    if (serviceDetails.Count > 0)
                        serviceDetails.Clear();
                    foreach (var service in partner.PartnerServices)
                    {
                        if (service != null)
                        {
                            var sd = await GetPartnerServiceDetailAsync(service.ServiceId);
                            serviceDetails.Add(sd);
                        }
                    }
                    partner.PartnerServices = serviceDetails.Where(result => result != null).ToList();
                }
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchPartnerByPartnerOrServiceName: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PartnerType>> GetPartnerTypesAsync(string? keyword)
        {
            try
            {
                List<PartnerType> partnerTypes = await _unitOfWork.PartnerTypeRepo.GetPartnerTypesAsync(keyword);
                Console.WriteLine("GetPartnerTypesAsync: " + partnerTypes.Count);
                return partnerTypes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerTypesAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Schedule>> GetScheduleByPartnerIdAsyn(int id)
        {
            List<Schedule> schedules = new List<Schedule>();
            try
            {
                schedules = await _unitOfWork.ScheduleRepo.GetScheduleByPartnerIdAsyn(id);

                if (schedules == null || schedules.Count == 0)
                {
                    Console.WriteLine($"No schedule found for partner with Id {id}");
                }
                else
                {
                    Console.WriteLine($"Found {schedules.Count} schedules for partner with Id {id}");
                }

                return schedules;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetScheduleByPartnerIdAsyn: {ex.Message}", ex);
                throw;
            }
        }

       
        public async Task<IEnumerable<PartnerServiceDTO>> GetTopFiveBookedServicesAsync()
        {
            try
            {
                var result = await _unitOfWork.PartnerServiceRepo.GetTopFiveBookedServicesAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTopFiveBookedServicesAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }


        public async Task<StatPartnerServiceDTO> GetStatPartnerServiceAsync(string email)
        {
            StatPartnerServiceDTO stat = new StatPartnerServiceDTO();
            if (email == null)

                throw new Exception("Partner not found");
            stat = await CalculateServicesAndRevenueAsync(email);

            return await Task.FromResult(stat);
        }


        public async Task<ListRevenueDTO> CalculatePartnerRevenueInMonthAsync(string email, int month, int year)
        {
            try
            {
                int total = 0;
                var revenuePerWeek = await _unitOfWork.PartnerRepo.GetRevenuePerWeekInMonthAsync(email, month, year);

                List<int?> result = revenuePerWeek.Select(revenue => (int?)revenue).ToList();
                foreach (var item in result)
                {
                    if (item.HasValue)
                    {
                        total += item.Value;
                    }

                }
                var bill15pt = total * 0.15;
                ListRevenueDTO listData = new ListRevenueDTO
                {
                    Data = result,
                    Bill15pt = (int)bill15pt,
                };

                return listData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CalculatePartnerRevenueInMonthAsync Services: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<PartnerServiceDTO>> GetPartnerServicesWithBookingCountAsync(string email)
        {
            try
            {
                Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(email);
                if (partner == null)
                    throw new Exception();
                return await _unitOfWork.PartnerRepo.GetPartnerServicesWithBookingCountAsync(partner.PartnerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerServicesWithBookingCountAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<StatPartnerServiceDTO> CalculateServicesAndRevenueAsync(string? email)
        {
            try
            {
                if (email == null)
                    throw new ArgumentNullException(nameof(email), "Partner email is null");

                return await _unitOfWork.PartnerRepo.CalculateServicesAndRevenueAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CalculateServicesAndRevenueAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<BookingDTO>> GetPartnerBookingsAsync(string partnerEmail, int page, int pageSize)
        {
            try
            {
                if (partnerEmail == null)
                {
                    return new List<BookingDTO>();
                }

                if (_filteredBookings == null)
                {
                    _filteredBookings = await _unitOfWork.BookingLogRepo.GetAllBookingDetailsByCustomerIdForPartnertAsync(partnerEmail);
                    foreach (var booking in _filteredBookings)
                    {
                        booking.MedicalServices = await _unitOfWork.BookingLogRepo.GetMedicalServiceDTOsAsync(booking.BookingId);
                    }
                }
                var skipAmount = (page - 1) * pageSize;
                var bookingsForPage = _filteredBookings.Skip(skipAmount).Take(pageSize).ToList();


                return bookingsForPage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerBookingsAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<bool> DeletePartnerServiceAsync(int id)
        {
            try
            {
                return await _unitOfWork.PartnerServiceRepo.DeletePartnerServiceAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeletePartnerServiceAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<(bool, string)> UpdatePartnerServiceAsync(UpdatePartnerServiceRequest request, int id)
        {
            try
            {
                var existingService = await _unitOfWork.PartnerServiceRepo.GetPartnerServiceByIdAsync(id);
                if (existingService == null)
                {
                    return (false, $"Không tìm thấy dịch vụ đối tác với ID {id}.");
                }
                if (!string.IsNullOrEmpty(request.Name))
                {
                    existingService.Name = request.Name;
                }

                if (request.Duration != default)
                {
                    existingService.Duration = request.Duration;
                }

                if (!string.IsNullOrEmpty(request.Description))
                {
                    existingService.Description = request.Description;
                }

                if (request.Price != default)
                {
                    existingService.Price = (int)request.Price;
                }

                await _unitOfWork.PartnerServiceRepo.UpdatePartnerServiceAsync(existingService);
                return (true, "Cập nhật dịch vụ đối tác thành công.");
            }
            catch (Exception ex)
            {
                return (false, "Có lỗi xảy ra khi cập nhật dịch vụ đối tác: " + ex.Message);
            }
        }

    }
}
