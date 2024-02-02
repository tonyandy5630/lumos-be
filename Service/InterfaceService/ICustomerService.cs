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

        Task<List<Customer>> GetCustomersAsync(string keyword);
        Task<Customer> GetCustomerByEmailAsync(string email);
        Task<Customer> GetCustomerByRefreshTokenAsync(string token);
        Task<Customer> GetCustomerByIDAsync(int id);
        Task<Customer> GetCustomerByCodeAsync(string code);
        Task<bool> AddCustomerAsync(Customer customer);
        Task<bool> UpdateCustomerAsync(Customer customer);
        Task<bool> BanCustomerAsync(int id);
        Task<List<Address>> GetCustomerAddressByCustomerIdAsync(int id);
    }
}
