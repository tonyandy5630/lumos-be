using Enum;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.InterfaceService;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class Authentication : IAuthentication
    {
        private readonly IConfiguration _configuration;
        private readonly IAdminService _adminService;
        private readonly ICustomerService _customerService;
        private readonly IPartnerService _partnerService;

        public Authentication(IConfiguration configuration, IAdminService adminService, ICustomerService customerService, IPartnerService partnerService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
        }

        public async Task<(bool, string)> IsUserAuthenticatedAsync(string email, string password)
        {
            try
            {
                string role = null;
                bool authenticated = false;

                var adminResponse = await _adminService.GetAdminByEmailAsync(email);
                if (adminResponse != null && adminResponse.Password == password)
                {
                    authenticated = true;
                    role = nameof(RolesEnum.Admin);
                }

                if (!authenticated)
                {
                    var partnerResponse = await _partnerService.GetPartnerByEmailAsync(email);
                    if (partnerResponse != null && partnerResponse.Password == password)
                    {
                        authenticated = true;
                        role = nameof(RolesEnum.Partner);

                    }
                }

                if (!authenticated)
                {
                    var customerResponse = await _customerService.GetCustomerByEmailAsync(email);
                    if (customerResponse != null && customerResponse.Password == password)
                    {
                        authenticated = true;
                        role = nameof(RolesEnum.Customer);

                    }
                }

                if (authenticated && role != "Admin")
                {
                    await UpdateLastLoginTime(email);
                }

                return (authenticated, role);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in IsUserAuthenticatedAsync: {ex.Message}");
                return (false, null);
            }
        }

        public async Task<(string, DateTime, DateTime, string)> GenerateToken(string email, string role)
        {
            try
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var refreshTokenSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var refreshTokenSigningCredentials = new SigningCredentials(refreshTokenSigningKey, SecurityAlgorithms.HmacSha256);
                var accessTokenExpiration = DateTime.UtcNow.AddHours(3);
                var refreshTokenExpiration = DateTime.UtcNow.AddHours(24);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role),
                };

                var accessTokenOptions = new JwtSecurityToken(
                    claims: claims,
                    expires: accessTokenExpiration,
                    signingCredentials: signingCredentials
                );

                var refreshTokenOptions = new JwtSecurityToken(
                    claims: claims,
                    expires: refreshTokenExpiration,
                    signingCredentials: refreshTokenSigningCredentials
                );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(accessTokenOptions);
                var refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshTokenOptions);

                await SaveRefreshTokenToDatabase(email, refreshTokenString);
                var accessTokenexpiration = accessTokenOptions.ValidTo;
                var refreshTokenexpires = refreshTokenOptions.ValidTo;
                return (accessToken, accessTokenexpiration, refreshTokenexpires, refreshTokenString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GenerateToken: {ex.Message}");
                throw;
            }
        }

        public async Task SaveRefreshTokenToDatabase(string email, string refreshToken)
        {
            try
            {
                var adminResponse = await _adminService.GetAdminByEmailAsync(email);
                var customerResponse = await _customerService.GetCustomerByEmailAsync(email);
                var partnerResponse = await _partnerService.GetPartnerByEmailAsync(email);

                switch (adminResponse, customerResponse, partnerResponse)
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
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in SaveRefreshTokenToDatabase: {ex.Message}");
                throw;
            }
        }

        public async Task<(bool, string)> ValidateRefreshToken(string refreshToken)
        {
            try
            {
                var adminResponse = await _adminService.GetAdminByRefreshTokenAsync(refreshToken);
                var customerResponse = await _customerService.GetCustomerByRefreshTokenAsync(refreshToken);
                var partnerResponse = await _partnerService.GetPartnerByRefreshTokenAsync(refreshToken);

                if (adminResponse != null)
                {
                    return (true, adminResponse.Email);
                }
                else if (customerResponse != null)
                {
                    return (true, customerResponse.Email);
                }
                else if (partnerResponse != null)
                {
                    return (true, partnerResponse.Email);
                }
                else
                {
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in ValidateRefreshToken: {ex.Message}");
                return (false, null);
            }
        }

        public async Task<(bool, string)> CheckRole(string email)
        {
            try
            {
                var adminResponse = await _adminService.GetAdminByEmailAsync(email);
                var customerResponse = await _customerService.GetCustomerByEmailAsync(email);
                var partnerResponse = await _partnerService.GetPartnerByEmailAsync(email);

                if (adminResponse != null)
                {
                    return (true, nameof(RolesEnum.Admin));
                }
                else if (customerResponse != null)
                {
                    return (true, nameof(RolesEnum.Customer));
                }
                else if (partnerResponse != null)
                {
                    return (true, nameof(RolesEnum.Partner));
                }
                else
                {
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CheckRole: {ex.Message}");
                return (false, null);
            }
        }

        public async Task UpdateLastLoginTime(string email)
        {
            try
            {
                var customerResponse = await _customerService.GetCustomerByEmailAsync(email);
                var partnerResponse = await _partnerService.GetPartnerByEmailAsync(email);

                DateTime now = DateTime.UtcNow;

                switch (customerResponse, partnerResponse)
                {
                    case (BussinessObject.Customer customer, _):
                        customer.LastLogin = now;
                        await _customerService.UpdateCustomerAsync(customer);
                        break;

                    case (_, BussinessObject.Partner partner):
                        partner.LastLogin = now;
                        await _partnerService.UpdatePartnerAsync(partner);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UpdateLastLoginTime: {ex.Message}");
            }
        }

        public Task<bool> SignOutAsync(string email, string roles)
        {
            try
            {

                return Task.FromResult(false);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
