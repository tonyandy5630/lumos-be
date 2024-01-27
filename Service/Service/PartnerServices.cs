using BussinessObject;
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
    public class PartnerServices:IPartnerService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PartnerServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       

        public async Task<ApiResponse<PartnerService?>> GetPartnerServiceDetailAsync(int serviceId)
        {
            ApiResponse<PartnerService?> response = new ApiResponse<PartnerService?>();
            response.message = MessagesResponse.Error.NotFound;
            response.StatusCode = 404;
            PartnerService? service =  await _unitOfWork.PartnerRepo.GetPartnerServiceDetailByIdAsync(serviceId);

            if(service == null)
                return response;

            response.data = service;
            return response;
        }
    }
}
