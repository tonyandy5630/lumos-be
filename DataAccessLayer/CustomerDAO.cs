using BussinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class CustomerDAO
    {
        private static CustomerDAO instance = null;
        private readonly LumosDBContext dbContext;

        public CustomerDAO(LumosDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public static CustomerDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CustomerDAO(new LumosDBContext());
                }
                return instance;
            }
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            try
            {
                return await dbContext.Customers.ToListAsync();
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
                return await dbContext.Customers.SingleOrDefaultAsync(u => u.RefreshToken.Equals(token));
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
                return await dbContext.Customers.SingleOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));
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
                return await dbContext.Customers.SingleOrDefaultAsync(u => u.CustomerId == id);
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
                return await dbContext.Customers.SingleOrDefaultAsync(u => u.Code.ToLower().Equals(code.ToLower()));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AddCustomerAsync(Customer customer)
        {
            try
            {
                bool existingAccount = (await GetCustomersAsync())
                    .Any(a => a.Code.ToLower().Equals(customer.Code.ToLower()) || a.Email.ToLower().Equals(customer.Email.ToLower()));

                if (!existingAccount)
                {
                    dbContext.Customers.Add(customer);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Add Customer: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                var existing = await dbContext.Customers.SingleOrDefaultAsync(x => x.CustomerId == customer.CustomerId);
                if (existing != null)
                {
                    existing.Code = customer.Code;
                    existing.Email = customer.Email;
                    existing.Password = customer.Password;
                    existing.Status = customer.Status;

                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    Console.WriteLine("Customer not found for updating.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Update Customer: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<bool> BanCustomerAsync(int id)
        {
            try
            {
                Customer u = await dbContext.Customers.SingleOrDefaultAsync(a => a.CustomerId == id);

                if (u != null)
                {
                    u.Status = (u.Status == 0) ? 1 : 0;

                    dbContext.Entry(u).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine("Customer status updated successfully!");
                    return true;
                }
                else
                {
                    Console.WriteLine("Customer does not exist!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BanCustomer: {ex.Message}");
                return false;
            }
        }
    }
}
