using BussinessObject;
using Microsoft.EntityFrameworkCore;
using Utils;

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

        public async Task<List<Customer>> GetCustomersAsync(string keyword)
        {
            try
            {
                List<Customer> customers;

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    customers = await dbContext.Customers.ToListAsync();
                }
                else
                {
                    customers = await dbContext.Customers
                        .Where(c => c.Fullname.Contains(keyword) || c.Email.Contains(keyword))
                        .ToListAsync();
                }

                if (customers == null || customers.Count == 0)
                {
                    Console.WriteLine("No customers were found!");
                    return null;
                }

                return customers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCustomersAsync: {ex.Message}", ex);
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
                Console.WriteLine($"Error in GetCustomerByRefreshTokenAsync: {ex.Message}", ex);
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
                Console.WriteLine($"Error in GetCustomerByEmailAsync: {ex.Message}", ex);
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
                Console.WriteLine($"Error in GetCustomerByIDAsync: {ex.Message}", ex);
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
                Console.WriteLine($"Error in GetCustomerByCodeAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AddCustomerAsync(Customer customer)
        {
            try
            {
                bool existingAccount = dbContext.Customers
                    .Any(a => a.Email.ToLower().Equals(customer.Email.ToLower()));

                if (!existingAccount)
                {
                    customer.Code = GenerateCode.GenerateRoleCode("customer");
                    customer.Status = 1;
                    customer.Role = 2;
                    customer.Fullname = ExtractNameFromEmail(customer.Email);
                    DateTime currentDate = DateTime.UtcNow;
                    customer.LastUpdate = currentDate;
                    customer.UpdateBy = customer.Email;
                    customer.CreatedDate = currentDate;

                    dbContext.Customers.Add(customer);
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine("Add customer successfully!");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddCustomerAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }


        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                var existing = await dbContext.Customers.SingleOrDefaultAsync(x => x.CustomerId == customer.CustomerId);
                if (existing != null)
                {
                    existing.Email = customer.Email;
                    existing.Password = customer.Password;
                    existing.Status = customer.Status;

                    await dbContext.SaveChangesAsync();
                    Console.WriteLine("Customer updated successfully!");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateCustomerAsync: {ex.Message}", ex);
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
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BanCustomerAsync: {ex.Message}");
                return false;
            }
        }

        private string ExtractNameFromEmail(string email)
        {
            string[] parts = email.Split('@');
            if (parts.Length > 0)
            {
                return parts[0];
            }
            return string.Empty;
        }

        public async Task<List<Address>> GetCustomerAddressByCustomerIdAsync(int customerId)
        {
            try
            {
                return await dbContext.Addresses.Where(x => x.CustomerId == customerId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Address> AddCustomerAddressAsync(Address address)
        {
            try
            {
                //check if address existed via displayname and Address
                bool existedAddress = dbContext.Addresses
                    .Where(x => x.CustomerId == address.CustomerId)
                    .Any(x => x.DisplayName.ToLower().Equals(address.DisplayName.ToLower()) || 
                              x.Address1.ToLower().Equals(address.Address1.ToLower()));

                if (!existedAddress)
                {
                    address.Code = GenerateCode.GenerateTableCode("address");
                    address.Status = 1;
                    address.CreatedDate = DateTime.UtcNow;
                    address.LastUpdate = DateTime.UtcNow;
                    //address.UpdatedBy = "admin";
                    //address.CreatedBy = "admin";

                    dbContext.Addresses.Add(address);
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine("Add address successfully!");
                    return await dbContext.Addresses.SingleOrDefaultAsync(x => x.Code.Equals(address.Code));
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
