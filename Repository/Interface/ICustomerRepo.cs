using BussinessObject;
using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ICustomerRepo
    {
        Task<List<Customer>> GetCustomersAsync(string keyword);
        Task<Customer> GetCustomerByEmailAsync(string email);
        Task<Customer> GetCustomerByRefreshTokenAsync(string token);
        Task<Customer> GetCustomerByIDAsync(int id);
        Task<Customer> GetCustomerByCodeAsync(string code);
        Task<bool> AddCustomerAsync(Customer customer);
        Task<bool> UpdateCustomerAsync(Customer customer);
        Task<bool> BanCustomerAsync(int id);
        Task<List<Address>> GetCustomersAddressByCustomerIdAsync(int id);
        Task<Address> AddCustomerAddressAsync(Address address, string email);
        Task<bool> CheckExistingAddressAsync(string address);
        Task<bool> CheckExistingMedicalReportAsync(string fullName);
        Task<Customer?> GetCustomerByBookingIdAsync(int bookingId);
        Task<List<ChartStatDTO>> GetNewCustomerMonthlyAsync(int year);
        Task<List<Customer>> GetAllCustomersAsync();

    }
}
