
using BussinessObject;
using DataAccessLayer;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class AdminRepo : IAdminRepo
    {
        public AdminRepo(LumosDBContext context) {
        }

        public Task<bool> AddAdminAsync(Admin admin) => AdminDAO.Instance.AddAdminAsync(admin);

        public Task<bool> BanAdminAsync(int id) => AdminDAO.Instance.BanAdminAsync(id);

        public Task<Admin> GetAdminByCodeAsync(string code) => AdminDAO.Instance.GetAdminByCodeAsync(code);

        public Task<Admin> GetAdminByEmailAsync(string email) => AdminDAO.Instance.GetAdminByEmailAsync(email);

        public Task<Admin> GetAdminByIDAsync(int id) => AdminDAO.Instance.GetAdminByIDAsync(id);

        public Task<Admin> GetAdminByRefreshTokenAsync(string token) => AdminDAO.Instance.GetAdminByRefreshTokenAsync(token);

        public Task<List<Admin>> GetAdminsAsync() => AdminDAO.Instance.GetAdminsAsync();

        public Task<bool> UpdateAdminAsync(Admin admin) => AdminDAO.Instance.UpdateAdminAsync(admin);
    }
}
