using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.InterfaceService
{
    public interface IAuthentication
    {
        Task<(bool, string)> IsUserAuthenticatedAsync(string email, string password);
        Task<(string, DateTime, string)> GenerateToken(string email, string role);
        string GenerateRefreshToken();
        Task SaveRefreshTokenToDatabase(string email, string refreshToken);
        Task<(bool, string)> ValidateRefreshToken(string refreshToken);
        Task<(bool, string)> CheckRole(string email);
    }
}
