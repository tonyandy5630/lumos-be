// Add missing using directives
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BussinessObject.AuthenModel;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.InterfaceService;
using Utils;

namespace LumosSolution.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IAdminService _adminService;
        private readonly ICustomerService _customerService;
        private readonly IPartnerService _partnerService;

        public AuthController(IConfiguration configuration, IAdminService adminService, ICustomerService customerService, IPartnerService partnerService)
        {
            _configuration = configuration;
            _adminService = adminService;
            _customerService = customerService;
            _partnerService = partnerService;
        }
        //Chưa hoàn thiện 
        [HttpPost("loginwithgoogle")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] string credential)
        {
            var setting = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["Authentication:Google:clientId"] }
            };

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(credential, setting);
                // Handle the case when the user is not found in the database after Google authentication
                if (User != null)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception accordingly
                return BadRequest("Google authentication failed.");
            }

            return BadRequest("User not found after Google authentication.");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var (authenticated, role) = await IsUserAuthenticatedAsync(model.Email, model.Password);

            if (authenticated)
            {
                var (token, expiration, refreshToken) = await GenerateToken(model.Email, role);


                var response = new ApiResponse<object>
                {
                    message = $"Logged in with role: {role}",
                    StatusCode = 200,
                    data = new { Token = token, Expiration = expiration, RefreshToken = refreshToken }
                };

                return Ok(response);
            }

            var unauthorizedResponse = new ApiResponse<object>
            {
                message = "Login failure",
                StatusCode = 401,
                data = null
            };

            return Unauthorized(unauthorizedResponse);
        }
        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshModel model)
        {
            if (string.IsNullOrEmpty(model.RefreshToken))
            {
                return BadRequest("Refresh token is missing.");
            }

            var (isValid, userEmail) = await ValidateRefreshToken(model.RefreshToken);

            if (!isValid)
            {
                return BadRequest("Invalid refresh token.");
            }
            var (roleValid, userRole) = await CheckRole(userEmail);

            if (!roleValid)
            {
                return BadRequest("Invalid user role.");
            }
            var (token, expiration, newRefreshToken) = await GenerateToken(userEmail, userRole);

            await SaveRefreshTokenToDatabase(userEmail, newRefreshToken);

            var response = new ApiResponse<object>
            {
                message = "Token refreshed successfully.",
                StatusCode = 200,
                data = new { Token = token, Expiration = expiration, RefreshToken = newRefreshToken }
            };

            return Ok(response);
        }
        //Function
        private async Task<(bool, string)> IsUserAuthenticatedAsync(string email, string password)
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

        private async Task<(string, DateTime, string)> GenerateToken(string email, string role)
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
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            var expiration = tokenOptions.ValidTo;
            await SaveRefreshTokenToDatabase(email, refreshToken);
            return (token, expiration,refreshToken);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
        private async Task SaveRefreshTokenToDatabase(string email, string refreshToken)
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
        private async Task<(bool, string)> ValidateRefreshToken(string refreshToken)
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
        private async Task<(bool, string)> CheckRole(string email)
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
