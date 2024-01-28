using BussinessObject;
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
        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> AddAdminAsync(Admin admin)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                bool result = await _unitOfWork.AdminRepo.AddAdminAsync(admin);
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

        public async Task<ApiResponse<bool>> BanAdminAsync(int id)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                bool result = await _unitOfWork.AdminRepo.BanAdminAsync(id);
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

        public async Task<ApiResponse<Admin>> GetAdminByCodeAsync(string code)
        {
            ApiResponse<Admin> response = new ApiResponse<Admin>();
            try
            {
                Admin admin = await _unitOfWork.AdminRepo.GetAdminByCodeAsync(code);
                if (admin == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = admin;
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

        public async Task<ApiResponse<Admin>> GetAdminByEmailAsync(string email)
        {
            ApiResponse<Admin> response = new ApiResponse<Admin>();
            try
            {
                Admin admin = await _unitOfWork.AdminRepo.GetAdminByEmailAsync(email);
                if (admin == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = admin;
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

        public async Task<ApiResponse<Admin>> GetAdminByIDAsync(int id)
        {
            ApiResponse<Admin> response = new ApiResponse<Admin>();
            try
            {
                Admin admin = await _unitOfWork.AdminRepo.GetAdminByIDAsync(id);
                if (admin == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = admin;
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

        public async Task<ApiResponse<Admin>> GetAdminByRefreshTokenAsync(string token)
        {
            ApiResponse<Admin> response = new ApiResponse<Admin>();
            try
            {
                Admin admin = await _unitOfWork.AdminRepo.GetAdminByRefreshTokenAsync(token);
                if (admin == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = admin;
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

        public async Task<ApiResponse<List<Admin>>> GetAdminsAsync()
        {
            ApiResponse<List<Admin>> response = new ApiResponse<List<Admin>>();
            try
            {
                List<Admin> admins = await _unitOfWork.AdminRepo.GetAdminsAsync();
                if (admins == null || admins.Count == 0)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = admins;
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

        public async Task<ApiResponse<bool>> UpdateAdminAsync(Admin admin)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                bool result = await _unitOfWork.AdminRepo.UpdateAdminAsync(admin);
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
    }
}
