﻿using BussinessObject;
using DataAccessLayer;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class CustomerRepo : ICustomerRepo
    {
        public CustomerRepo(LumosDBContext context) { }

        public Task<bool> AddCustomerAsync(Customer customer) =>CustomerDAO.Instance.AddCustomerAsync(customer);

        public Task<bool> BanCustomerAsync(int id) => CustomerDAO.Instance.BanCustomerAsync(id);

        public Task<Customer> GetCustomerByCodeAsync(string code) => CustomerDAO.Instance.GetCustomerByCodeAsync(code);

        public Task<Customer> GetCustomerByEmailAsync(string email) => CustomerDAO.Instance.GetCustomerByEmailAsync(email);

        public Task<Customer> GetCustomerByIDAsync(int id) => CustomerDAO.Instance.GetCustomerByIDAsync(id);

        public Task<Customer> GetCustomerByRefreshTokenAsync(string token) => CustomerDAO.Instance.GetCustomerByRefreshTokenAsync(token);

        public Task<List<Customer>> GetCustomersAsync() => CustomerDAO.Instance.GetCustomersAsync();

        public Task<bool> UpdateCustomerAsync(Customer customer) => CustomerDAO.Instance.UpdateCustomerAsync(customer);

        public Task<List<Address>> GetCustomersAddressByCustomerIdAsync(int id) => CustomerDAO.Instance.GetCustomersAddressByCustomerIdAsync(id);
    }
}
