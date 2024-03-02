using AutoMapper;
using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.EntityFrameworkCore.Storage;
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
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
                medReport = await _unitOfWork.MedicalReportRepo.GetMedicalReportByCustomerIdAsync(id);

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

        public async Task<MedicalReport> AddMedicalReportAsync(MedicalReportDTO medicalReport, string? userEmail)
        {
            try
            {
                if (string.IsNullOrEmpty(userEmail))
                { throw new Exception("Cannot find customer email"); }

                Customer customer = await _unitOfWork.CustomerRepo.GetCustomerByEmailAsync(userEmail);

                if (customer == null)
                    throw new Exception("Cannot find customer");

                MedicalReport medReport = _mapper.Map<MedicalReport>(medicalReport);

                medReport.CustomerId = customer.CustomerId;
/*                medReport.Customer = customer;*/

                MedicalReport med = await _unitOfWork.MedicalReportRepo.AddMedicalReportAsyn(medReport);

                if (med == null) { throw new Exception("Add failed"); }

                return med;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in add medical report: {ex.Message}");
            }
        }

        public async Task<Address> AddCustomerAddressAsync(Address address, string email) 
        {
            try
            {
                Address addedAddress = await _unitOfWork.CustomerRepo.AddCustomerAddressAsync(address,email);
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

        public async Task<MedicalReport> GetMedicalReportByIdAsync(int id)
        {

            try
            {
                MedicalReport med = await _unitOfWork.MedicalReportRepo.GetMedicalReportByIdAsync(id);

                if (med == null)
                {
                    Console.WriteLine($"No medical report found with ID {id}");
                }
                else
                {
                    Console.WriteLine($"Found medical report with ID {id}");
                }

                return med;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMedicalReportByIdAsync: {ex.Message}", ex);
                throw;
            }
        }
        public async Task<bool> CheckExistingAddressAsync(string address)
        {
            try
            {
                return await _unitOfWork.CustomerRepo.CheckExistingAddressAsync(address);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckExistingAddressAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckExistingMedicalReportAsync(string fullName)
        {
            try
            {
                return await _unitOfWork.CustomerRepo.CheckExistingMedicalReportAsync(fullName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckExistingMedicalReportAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

    }
}
