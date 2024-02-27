using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IUserManagerRepo<T> where T : class
    {
        public string HashPassword(T user, string password);

        public bool VerifyPassword(T user, string verifyPassword, string providedPassword);
    }
}
