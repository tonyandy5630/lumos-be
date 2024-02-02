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

        public async Task<List<Customer>> GetCustomersAsync(string keyword)
        {
            try
            {
                return await _unitOfWork.CustomerRepo.GetCustomersAsync(keyword);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

        public async Task<List<Address>> GetCustomerAddressByCustomerIdAsync(int id)
        {
            List<Address> addresses;

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

        public async Task<Address> AddCustomerAddressAsync(Address address) 
        {
            try
            {
                Address addedAddress = await _unitOfWork.CustomerRepo.AddCustomerAddressAsync(address);
                if (addedAddress != null)
                {
                       Console.WriteLine("Address added successfully!");
                } else
                {
                    Console.WriteLine("Failed to add address!");    
                }

                return addedAddress;
            } catch (Exception ex)
            {
                Console.WriteLine($"Error in AddCustomerAddressAsync: {ex.Message}", ex);
                throw;
            }
        }
    }
}
