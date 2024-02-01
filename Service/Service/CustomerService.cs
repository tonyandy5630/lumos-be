using BussinessObject;
using Microsoft.Extensions.Logging;
using Repository.Interface.IUnitOfWork;
using Service.InterfaceService;
using System;
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

        public async Task<bool> AddCustomerAsync(Customer customer)
        {
            try
            {
                return await _unitOfWork.CustomerRepo.AddCustomerAsync(customer);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> BanCustomerAsync(int id)
        {
            try
            {
                return await _unitOfWork.CustomerRepo.BanCustomerAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Customer> GetCustomerByCodeAsync(string code)
        {
            try
            {
                return await _unitOfWork.CustomerRepo.GetCustomerByCodeAsync(code);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            try
            {
                return await _unitOfWork.CustomerRepo.GetCustomerByEmailAsync(email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Customer> GetCustomerByIDAsync(int id)
        {
            try
            {
                return await _unitOfWork.CustomerRepo.GetCustomerByIDAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Customer> GetCustomerByRefreshTokenAsync(string token)
        {
            try
            {
                return await _unitOfWork.CustomerRepo.GetCustomerByRefreshTokenAsync(token);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            try
            {
                return await _unitOfWork.CustomerRepo.GetCustomersAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                return await _unitOfWork.CustomerRepo.UpdateCustomerAsync(customer);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Address>?> GetCustomerAddressByCustomerIdAsync(int id)
        {
            List<Address> addresses = new List<Address>();

            try
            {
                addresses = await _unitOfWork.CustomerRepo.GetCustomersAddressByCustomerIdAsync(id);

                if (addresses == null || addresses.Count == 0)
                {
                    Console.WriteLine($"No addresses found for customer with ID {id}");
                } else
                {
                    Console.WriteLine($"Found {addresses.Count} addresses for customer with ID {id}");
                }

                return addresses;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCustomersAddressByCustomerIdAsync: {ex.Message}", ex);
                throw;
            }
        }
    }
}
