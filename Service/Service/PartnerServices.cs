using AutoMapper;
using BussinessObject;
using DataTransferObject.DTO;
using Repository.Interface.IUnitOfWork;
using Repository.Repo;
using Service.InterfaceService;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

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

        public async Task<bool> AddPartnerAsync(Partner partner)
        {
            try
            {
                return await _unitOfWork.PartnerRepo.AddPartnerAsync(partner);
            }
            catch (Exception ex)
            {
                Console.WriteLine(MessagesResponse.Error.OperationFailed);
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

        public async Task<Partner> GetPartnerByIDAsync(int id)
        {
            try
            {
                Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByIDAsync(id);
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

        public async Task<IEnumerable<SearchPartnerDTO>> SearchPartnerByPartnerOrServiceName(string keyword)
        {
            try
            {
                IEnumerable<Partner> searchedPartner = await _unitOfWork.PartnerRepo.SearchPartnerByPartnerOrServiceNameAsync(keyword.Trim());
                IEnumerable<SearchPartnerDTO> results = _mapper.Map<IEnumerable<SearchPartnerDTO>>(searchedPartner);
                // Task for getting details parrallel
                List<Task<PartnerServiceDTO?>> tasks = new();
                foreach (var partner in results)
                {
                    foreach (var service in partner.PartnerServices)
                    {
                        if (service != null)
                            tasks.Add(GetPartnerServiceDetailAsync(service.ServiceId));
                    }
                    // Wait for all the tasks to be done
                    var resultsForPartner = await Task.WhenAll(tasks);
                    //* Empty for mutable service detail
                    partner.PartnerServices = Enumerable.Empty<PartnerServiceDTO>();
                    partner.PartnerServices.ToList().AddRange(resultsForPartner);
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
    }
}
