using AutoMapper;
using BussinessObject;
using DataTransferObject.DTO;
using Repository.Interface.IUnitOfWork;
using Repository.Repo;
using Service.InterfaceService;
using System;
using System.Collections.Generic;
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

       

        public async Task<ApiResponse<PartnerServiceDTO?>> GetPartnerServiceDetailAsync(int serviceId)
        {
            ApiResponse<PartnerServiceDTO?> response = new ApiResponse<PartnerServiceDTO?>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = 404
            };

            try
            {
                PartnerService? service = await _unitOfWork.PartnerRepo.GetPartnerServiceDetailByIdAsync(serviceId);
                IEnumerable<ServiceCategory> serviceCategories = await _unitOfWork.ServiceCategoryRepo.GetCategoriesByServiceIdAsync(serviceId);

                if (service == null)
                    return response;

                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = 200;

                PartnerServiceDTO serviceDTO = _mapper.Map<PartnerServiceDTO>(service);
                IEnumerable<ServiceCategoryDTO> serviceCategoryDTOs = _mapper.Map<IEnumerable<ServiceCategoryDTO>>(serviceCategories);
                serviceDTO.Categories = serviceCategoryDTOs;
                response.data = serviceDTO;
            }
            catch
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = 500;
            }
           
            return response;
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

        public async Task<ApiResponse<IEnumerable<Partner>>> SearchPartnerByPartnerOrServiceName(string keyword)
        {
            ApiResponse<IEnumerable<Partner>> response = new ApiResponse<IEnumerable<Partner>>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = 500
            };
            try
            {
                IEnumerable<Partner> searchedPartner = await _unitOfWork.PartnerRepo.SearchPartnerByPartnerOrServiceNameAsync(keyword.Trim());
                response.message = MessagesResponse.Success.Completed;
                response.data = searchedPartner;
                response.StatusCode = 200;
                return response;
            }
            catch
            {
                return response;
            }
        }
    }
}
