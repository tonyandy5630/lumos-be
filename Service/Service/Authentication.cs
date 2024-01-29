using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.InterfaceService;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class Authentication :IAuthentication
    {
        private readonly IConfiguration _configuration;
        private readonly IAdminService _adminService;
        private readonly ICustomerService _customerService;
        private readonly IPartnerService _partnerService;

        public Authentication(IConfiguration configuration, IAdminService adminService, ICustomerService customerService, IPartnerService partnerService)
        {
            _configuration = configuration;
            _adminService = adminService;
            _customerService = customerService;
            _partnerService = partnerService;
        }
        public async Task<(bool, string)> IsUserAuthenticatedAsync(string email, string password)
        {
            string role = null;
            bool authenticated = false;

            var adminResponse = await _adminService.GetAdminByEmailAsync(email);
            if (adminResponse != null && adminResponse.data != null && adminResponse.data.Password == password)
            {
                authenticated = true;
                role = "Admin";
            }

            if (!authenticated)
            {
                var partnerResponse = await _partnerService.GetPartnerByEmailAsync(email);
                if (partnerResponse != null && partnerResponse.data != null && partnerResponse.data.Password == password)
                {
                    authenticated = true;
                    role = "Partner";
                }
            }

            if (!authenticated)
            {
                var customerResponse = await _customerService.GetCustomerByEmailAsync(email);
                if (customerResponse != null && customerResponse.data != null && customerResponse.data.Password == password)
                {
                    authenticated = true;
                    role = "Customer";
                }
            }

            return (authenticated, role);
        }

        public async Task<(string, DateTime, string)> GenerateToken(string email, string role)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var refreshToken = GenerateRefreshToken();

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("RefreshToken", refreshToken)
            };

            var tokenOptions = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            var expiration = tokenOptions.ValidTo;
            SaveRefreshTokenToDatabase(email, refreshToken);
            return (token, expiration, refreshToken);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        public async Task SaveRefreshTokenToDatabase(string email, string refreshToken)
        {
            var adminResponse = await _adminService.GetAdminByEmailAsync(email);
            var customerResponse = await _customerService.GetCustomerByEmailAsync(email);
            var partnerResponse = await _partnerService.GetPartnerByEmailAsync(email);

            switch (adminResponse?.data, customerResponse?.data, partnerResponse?.data)
            {
                case (BussinessObject.Admin admin, _, _):
                    admin.RefreshToken = refreshToken;
                    await _adminService.UpdateAdminAsync(admin);
                    break;

                case (_, BussinessObject.Customer customer, _):
                    customer.RefreshToken = refreshToken;
                    await _customerService.UpdateCustomerAsync(customer);
                    break;

                case (_, _, BussinessObject.Partner partner):
                    partner.RefreshToken = refreshToken;
                    await _partnerService.UpdatePartnerAsync(partner);
                    break;

                default:
                    break;
            }
        }

        public async Task<(bool, string)> ValidateRefreshToken(string refreshToken)
        {
            var adminResponse = await _adminService.GetAdminByRefreshTokenAsync(refreshToken);
            var customerResponse = await _customerService.GetCustomerByRefreshTokenAsync(refreshToken);
            var partnerResponse = await _partnerService.GetPartnerByRefreshTokenAsync(refreshToken);

            if (adminResponse?.data != null)
            {
                return (true, adminResponse.data.Email);
            }
            else if (customerResponse?.data != null)
            {
                return (true, customerResponse.data.Email);
            }
            else if (partnerResponse?.data != null)
            {
                return (true, partnerResponse.data.Email);
            }
            else
            {
                return (false, null);
            }
        }

        public async Task<(bool, string)> CheckRole(string email)
        {
            var adminResponse = await _adminService.GetAdminByEmailAsync(email);
            var customerResponse = await _customerService.GetCustomerByEmailAsync(email);
            var partnerResponse = await _partnerService.GetPartnerByEmailAsync(email);

            if (adminResponse?.data != null)
            {
                return (true, "Admin");
            }
            else if (customerResponse?.data != null)
            {
                return (true, "Customer");
            }
            else if (partnerResponse?.data != null)
            {
                return (true, "Partner");
            }
            else
            {
                return (false, null);
            }
        }
    }
}
