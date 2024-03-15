using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.InterfaceService
{
    public interface IAuthentication
    {
        Task<AuthenticateResponse> IsUserAuthenticatedAsync(string email, string password);
        Task<(string,DateTime,DateTime, string)> GenerateToken(string email, string role);
        Task SaveRefreshTokenToDatabase(string email, string refreshToken);
        Task<(bool, string)> ValidateRefreshTokenByEmail(string email);
        Task<(bool, string, object)> GetUserdetailsInLoginGooogle(string email);
        Task<(bool, string)> CheckRole(string email);
        Task UpdateLastLoginTimeAsync(object user);
        Task<bool> SignOutAsync(string accessToken);
    }
}
