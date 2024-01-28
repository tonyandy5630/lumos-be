using BussinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class AdminDAO
    {
        private static AdminDAO instance = null;
        private readonly LumosDBContext dbContext;

        public AdminDAO(LumosDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public static AdminDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdminDAO(new LumosDBContext());
                }
                return instance;
            }
        }

        public async Task<List<Admin>> GetAdminsAsync()
        {
            try
            {
                return await dbContext.Admins.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Admin> GetAdminByEmailAsync(string email)
        {
            try
            {
                return await dbContext.Admins.SingleOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Admin> GetAdminByRefreshTokenAsync(string token)
        {
            try
            {
                return await dbContext.Admins.SingleOrDefaultAsync(u => u.RefreshToken.Equals(token));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Admin> GetAdminByIDAsync(int id)
        {
            try
            {
                return await dbContext.Admins.SingleOrDefaultAsync(u => u.AdminId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Admin> GetAdminByCodeAsync(string code)
        {
            try
            {
                return await dbContext.Admins.SingleOrDefaultAsync(u => u.Code.ToLower().Equals(code.ToLower()));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AddAdminAsync(Admin admin)
        {
            try
            {
                List<Admin> existingAccounts = await GetAdminsAsync();
                bool isExistingAccount = existingAccounts
                    .Any(a => a.Code.ToLower().Equals(admin.Code.ToLower()) || a.Email.ToLower().Equals(admin.Email.ToLower()));

                if (!isExistingAccount)
                {
                    admin.Role = 1;
                    dbContext.Admins.Add(admin);
                    await dbContext.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Add Admin: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAdminAsync(Admin admin)
        {
            try
            {
                var existing = await dbContext.Admins.SingleOrDefaultAsync(x => x.AdminId == admin.AdminId);
                if (existing != null)
                {
                    existing.Code = admin.Code;
                    existing.Email = admin.Email;
                    existing.Password = admin.Password;
                    existing.Status = admin.Status;

                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    Console.WriteLine("Admin not found for updating.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Update Admin: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<bool> BanAdminAsync(int id)
        {
            try
            {
                Admin u = await dbContext.Admins.SingleOrDefaultAsync(a => a.AdminId == id);

                if (u != null)
                {
                    u.Status = (u.Status == 0) ? 1 : 0;

                    dbContext.Entry(u).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine("Admin status updated successfully!");
                    return true;
                }
                else
                {
                    Console.WriteLine("Account does not exist!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BanAdmin: {ex.Message}");
                return false;
            }
        }
    }
}
