using BussinessObject;
using Repository.Interface.IUnitOfWork;
using Service.InterfaceService;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Service.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> AddCustomerAsync(Customer customer)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                bool result = await _unitOfWork.CustomerRepo.AddCustomerAsync(customer);
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

        public async Task<ApiResponse<bool>> BanCustomerAsync(int id)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                bool result = await _unitOfWork.CustomerRepo.BanCustomerAsync(id);
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

        public async Task<ApiResponse<Customer>> GetCustomerByCodeAsync(string code)
        {
            ApiResponse<Customer> response = new ApiResponse<Customer>();
            try
            {
                Customer customer = await _unitOfWork.CustomerRepo.GetCustomerByCodeAsync(code);
                if (customer == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = customer;
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

        public async Task<ApiResponse<Customer>> GetCustomerByEmailAsync(string email)
        {
            ApiResponse<Customer> response = new ApiResponse<Customer>();
            try
            {
                Customer customer = await _unitOfWork.CustomerRepo.GetCustomerByEmailAsync(email);
                if (customer == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = customer;
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

        public async Task<ApiResponse<Customer>> GetCustomerByIDAsync(int id)
        {
            ApiResponse<Customer> response = new ApiResponse<Customer>();
            try
            {
                Customer customer = await _unitOfWork.CustomerRepo.GetCustomerByIDAsync(id);
                if (customer == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = customer;
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

        public async Task<ApiResponse<Customer>> GetCustomerByRefreshTokenAsync(string token)
        {
            ApiResponse<Customer> response = new ApiResponse<Customer>();
            try
            {
                Customer customer = await _unitOfWork.CustomerRepo.GetCustomerByRefreshTokenAsync(token);
                if (customer == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = customer;
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

        public async Task<ApiResponse<List<Customer>>> GetCustomersAsync()
        {
            ApiResponse<List<Customer>> response = new ApiResponse<List<Customer>>();
            try
            {
                List<Customer> customers = await _unitOfWork.CustomerRepo.GetCustomersAsync();
                if (customers == null || customers.Count == 0)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = 404;
                }
                else
                {
                    response.data = customers;
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

        public async Task<List<MedicalReport>> GetMedicalReportByCustomerIdAsync(int id)
        {
            List<MedicalReport> medReport = new List<MedicalReport>();
            try
            {
                medReport = await _unitOfWork.CustomerRepo.GetMedicalReportByCustomerIdAsync(id);

                if(medReport == null || medReport.Count == 0)
                {
                    Console.WriteLine($"No medical report found for customer with Id {id}");
                } else
                {
                    Console.WriteLine($"Found {medReport.Count} medical report for customer with Id {id}");
                }

                return medReport;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error in GetMedicalReportByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<ApiResponse<bool>> UpdateCustomerAsync(Customer customer)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                bool result = await _unitOfWork.CustomerRepo.UpdateCustomerAsync(customer);
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
