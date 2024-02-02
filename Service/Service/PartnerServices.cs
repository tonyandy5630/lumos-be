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

namespace Service.Service
{
    public class PartnerServices : IPartnerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        public PartnerServices(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
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
                partnerService.CreatedDate = DateTime.Now;
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
                PartnerService? service = await _unitOfWork.PartnerRepo.GetPartnerServiceDetailByIdAsync(serviceId);
                IEnumerable<ServiceCategory> serviceCategories = await _unitOfWork.ServiceCategoryRepo.GetCategoriesByServiceIdAsync(serviceId);

                if (service == null)
                    return null;

                PartnerServiceDTO? serviceDTO = _mapper.Map<PartnerServiceDTO>(service);
                IEnumerable<ServiceCategoryDTO> serviceCategoryDTOs = _mapper.Map<IEnumerable<ServiceCategoryDTO>>(serviceCategories);
                serviceDTO.Categories = serviceCategoryDTOs;
                return serviceDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<ApiResponse<bool>> AddPartnerAsync(Partner partner)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                bool result = await _unitOfWork.PartnerRepo.AddPartnerAsync(partner);
                response.data = result;
                response.message = result ? MessagesResponse.Success.Created : MessagesResponse.Error.OperationFailed;
                response.StatusCode = result ? 201 : 400;
            }
            catch
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = 500;
            }

            return response;
        }

        public async Task<ApiResponse<bool>> BanPartnerAsync(int partnerId)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                bool result = await _unitOfWork.PartnerRepo.BanPartnerAsync(partnerId);
                response.data = result;
                response.message = result ? MessagesResponse.Success.Updated : MessagesResponse.Error.OperationFailed;
                response.StatusCode = result ? 200 : 400;
            }
            catch
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = 500;
            }

            return response;
        }

        public async Task<ApiResponse<Partner>> GetPartnerByCodeAsync(string code)
        {
            ApiResponse<Partner> response = new ApiResponse<Partner>();
            try
            {
                Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByCodeAsync(code);
                if (partner == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = partner;
                    response.message = MessagesResponse.Success.Completed;
                    response.StatusCode = 200;
                }
            }
            catch
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = 500;
            }

            return response;
        }

        public async Task<ApiResponse<Partner>> GetPartnerByEmailAsync(string email)
        {
            ApiResponse<Partner> response = new ApiResponse<Partner>();
            try
            {
                Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(email);
                if (partner == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = partner;
                    response.message = MessagesResponse.Success.Completed;
                    response.StatusCode = 200;
                }
            }
            catch
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = 500;
            }

            return response;
        }

        public async Task<ApiResponse<Partner>> GetPartnerByIDAsync(int id)
        {
            ApiResponse<Partner> response = new ApiResponse<Partner>();
            try
            {
                Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByIDAsync(id);
                if (partner == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = partner;
                    response.message = MessagesResponse.Success.Completed;
                    response.StatusCode = 200;
                }
            }
            catch
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = 500;
            }

            return response;
        }

        public async Task<ApiResponse<List<Partner>>> GetAllPartnersAsync()
        {
            ApiResponse<List<Partner>> response = new ApiResponse<List<Partner>>();
            try
            {
                List<Partner> partners = await _unitOfWork.PartnerRepo.GetAllPartnersAsync();
                if (partners == null || partners.Count == 0)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = partners;
                    response.message = MessagesResponse.Success.Completed;
                    response.StatusCode = 200;
                }
            }
            catch
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = 500;
            }

            return response;
        }

        public async Task<ApiResponse<bool>> UpdatePartnerAsync(Partner partner)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                bool result = await _unitOfWork.PartnerRepo.UpdatePartnerAsync(partner);
                response.data = result;
                response.message = result ? MessagesResponse.Success.Updated : MessagesResponse.Error.OperationFailed;
                response.StatusCode = result ? 200 : 400;
            }
            catch
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = 500;
            }

            return response;
        }

        public async Task<ApiResponse<Partner>> GetPartnerByRefreshTokenAsync(string token)
        {
            ApiResponse<Partner> response = new ApiResponse<Partner>();
            try
            {
                Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByRefreshTokenAsync(token);
                if (partner == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = partner;
                    response.message = MessagesResponse.Success.Completed;
                    response.StatusCode = 200;
                }
            }
            catch
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = 500;
            }

            return response;
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
            catch
            {
                return null;
            }
        }


    }
}
