using AutoMapper;
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

namespace Service.Service
{
    public class PartnerServices : IPartnerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PartnerServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PartnerService?> AddPartnerServiceAsync(AddPartnerServiceResquest service, string? partnerEmail)
        {
            const string TRANSACTION = "add-parner-service";
            IDbContextTransaction commit = await _unitOfWork.StartTransactionAsync(TRANSACTION);
            try
            {
                List<Task<ServiceCategory>> categoryTasks = new(); // parallel category validation
                List<Task<ServiceDetail?>> serviceDetailTasks = new(); // add service detail parallel
                List<ServiceCategory> categories = new();

                if (partnerEmail == null)
                    throw new Exception("Not found partner email");


                Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(partnerEmail);

                if (partner == null)
                    throw new Exception("Not found partner");

                PartnerService? existedService = await _unitOfWork.PartnerServiceRepo.GetPartnerServiceByServiceNameAsync(service.Name, partner.PartnerId);
                if (existedService != null)
                    throw new Exception("Service with same name has existed");

                PartnerService partnerService = _mapper.Map<PartnerService>(service);
                partnerService.PartnerId = partner.PartnerId;
                partnerService.Partner = partner;
                partnerService.Status = 1;
                partnerService.CreatedDate = DateTime.Now;
                partnerService.LastUpdate = DateTime.Now;
                partnerService.Rating = 0;
                partner.UpdatedBy = partner.PartnerName;
                // Validate categories
                // better if can do concurrently
                foreach (int cateId in service.Categories)
                {
                    ServiceCategory? cate = await _unitOfWork.ServiceCategoryRepo.GetCategoryByIdAsync(cateId);
                    categories.Add(cate);
                }

                if (categories.Count != service.Categories.ToList().Count)
                    throw new Exception("Categories is not matched");

                PartnerService? newService = await _unitOfWork.PartnerRepo.AddPartnerServiceAsync(partnerService);

                if (newService == null)
                    throw new Exception("Added faield");


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
                        throw new Exception("Error when adding 1 of the details");
                }

                await _unitOfWork.CommitTransactionAsync(commit);
                return newService;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync(commit, TRANSACTION);
                throw new Exception(ex.Message);
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

        public async Task<Partner?> AddPartnereAsync(AddPartnerRequest partner)
        {
            try
            {

                PartnerType? partnerType = await _unitOfWork.PartnerTypeRepo.GetPartnerTypeByIdAsync(partner.TypeId);
                if(partnerType == null)
                    throw new NullReferenceException("Partner Type is not existed");

                Partner addPartner = _mapper.Map<Partner>(partner);
                // Hash password
                IUserManagerRepo<AddPartnerRequest> userManager = new UserManagerRepo<AddPartnerRequest>();
                addPartner.Password = userManager.HashPassword(partner, partner.Email);
                
                Partner? part = await _unitOfWork.PartnerRepo.AddPartnereAsync(addPartner);

                if(part == null)
                {
                    Console.WriteLine("Failed to add partner!");
                    throw new Exception("Something went wrong when adding partner");
                } 
                return part;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex is NullReferenceException)
                    throw new NullReferenceException(ex.Message);
                throw new Exception(ex.Message);
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
                IEnumerable<SearchPartnerDTO> results = _mapper.Map<IEnumerable<SearchPartnerDTO>>(searchedPartner);
                List<PartnerServiceDTO?> serviceDetails = new List<PartnerServiceDTO?>();
                foreach (var partner in results)
                {
                    // empty old  partner servicelist for new partner
                    if(serviceDetails.Count > 0)
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
                Console.WriteLine($"Error in SearchPartnerByPartnerOrServiceName: { ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PartnerType>> GetPartnerTypesAsync(string? keyword)
        {
            try
            {   List<PartnerType> partnerTypes =  await _unitOfWork.PartnerTypeRepo.GetPartnerTypesAsync(keyword);
                Console.WriteLine("GetPartnerTypesAsync: " + partnerTypes.Count);
                return partnerTypes;
            } catch (Exception ex)
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

        public async Task<Schedule> AddPartnerScheduleAsync(Schedule schedule)
        {
            try
            {
                Schedule addedSchedule = await _unitOfWork.ScheduleRepo.AddPartnerScheduleAsync(schedule);
                if (addedSchedule == null)
                {
                    throw new Exception("Something wrong, Schedule not added");
                }
                else
                {
                    Console.WriteLine("Schedule added successfully");
                }
                return addedSchedule;
            } catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPartnerScheduleAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
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
            Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(email);

            if (partner == null)
            {
                throw new Exception("Partner not found");
            }
            int revenue = 0;

            foreach (var service in partner.PartnerServices)
            {
                revenue += service.Price;
            }
            stat.totalServices = partner.PartnerServices.Count;
            stat.revenue = revenue;

            return  await Task.FromResult(stat);
        }

      
        public async Task<List<RevenuePerWeekDTO>> CalculatePartnerRevenueInMonthAsync(int month, int year)
        {
            try
            {
                return await _unitOfWork.PartnerRepo.CalculatePartnerRevenueInMonthAsync(month,year);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CalculatePartnerRevenueInMonthAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<MonthlyRevenueDTO>> CalculateMonthlyRevenueAsync(int year)
        {
            try
            {
                return await _unitOfWork.PartnerRepo.CalculateMonthlyRevenueAsync(year);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CalculateMonthlyRevenueAsync: {ex.Message}", ex);
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
    }
}
