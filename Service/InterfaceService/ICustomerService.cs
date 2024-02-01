using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.InterfaceService
{
    public interface ICustomerService
    {
        Task<ApiResponse<List<Customer>>> GetCustomersAsync();
        Task<ApiResponse<Customer>> GetCustomerByEmailAsync(string email);
        Task<ApiResponse<Customer>> GetCustomerByRefreshTokenAsync(string token);
        Task<ApiResponse<Customer>> GetCustomerByIDAsync(int id);
        Task<ApiResponse<Customer>> GetCustomerByCodeAsync(string code);
        Task<ApiResponse<bool>> AddCustomerAsync(Customer customer);
        Task<ApiResponse<bool>> UpdateCustomerAsync(Customer customer);
        Task<ApiResponse<bool>> BanCustomerAsync(int id);
        Task<List<Address>> GetCustomerAddressByCustomerIdAsync(int id);
    }
}
