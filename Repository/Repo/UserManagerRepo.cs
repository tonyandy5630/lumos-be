using BussinessObject;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using RequestEntity;

namespace Repository.Repo
{
    public class UserManagerRepo<TUser>: IUserManagerRepo<TUser> where TUser : class
    {
        private readonly PasswordHasher<TUser> _passwordHasher;
        public UserManagerRepo() {
            _passwordHasher = new PasswordHasher<TUser>();
        }
        public string HashPassword(TUser user, string password)
        {
            CheckUserType(user);
            string hashedPassword = _passwordHasher.HashPassword(user, password);
            return hashedPassword;
        }

        public bool VerifyPassword(TUser user, string verifyPassword, string providedPassword)
        {
            CheckUserType(user);
            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user , providedPassword, providedPassword);
            return  result == PasswordVerificationResult.Success;
        }

        private void CheckUserType(TUser user)
        {
            bool isPartner = user is AddPartnerRequest;
            bool isAdmin = user is Admin;
            bool isCustomer = user is Customer;
            if ( !isPartner && !isAdmin && !isCustomer)
            {
                throw new NotSupportedException("Not supported User type");
            }
        }
    }
}
