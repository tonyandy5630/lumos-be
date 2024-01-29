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
        private readonly IAuthentication _authentication;
        public AuthController(IConfiguration configuration, IAdminService adminService,
            ICustomerService customerService, IPartnerService partnerService
            , IAuthentication authentication)
        {
            _configuration = configuration;
            _adminService = adminService;
            _customerService = customerService;
            _partnerService = partnerService;
            _authentication = authentication;
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

                var userEmail = payload.Email;

                // Kiểm tra xem người dùng đã tồn tại trong hệ thống hay chưa
                var (roleValid, userRole) = await _authentication.CheckRole(userEmail);

                if (!roleValid)
                {
                    return BadRequest("Invalid user role.");
                }

                var (token, accessTokenTime, refreshTokentime, refreshToken) = await _authentication.GenerateToken(userEmail, userRole);

                var accessTokenRemainingTime = accessTokenTime - DateTime.UtcNow;
                var refreshTokenRemainingTime = refreshTokentime - DateTime.UtcNow;

                var response = new ApiResponse<object>
                {
                    message = $"Logged in with role: {userRole}",
                    StatusCode = 200,
                    data = new
                    {
                        Token = token,
                        AccessTokenExpiration = accessTokenRemainingTime,
                        RefreshToken = refreshToken,
                        RefreshTokenExpiration = refreshTokenRemainingTime
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest("Google authentication failed.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var (authenticated, role) = await _authentication.IsUserAuthenticatedAsync(model.Email, model.Password);

            if (authenticated)
            {
                var (token, accessTokenTime, refreshTokentime, refreshToken) = await _authentication.GenerateToken(model.Email, role);
                
                var accessTokenRemainingTime = accessTokenTime - DateTime.UtcNow;
                var refreshTokenRemainingTime = refreshTokentime - DateTime.UtcNow;


                var response = new ApiResponse<object>
                {
                    message = $"Logged in with role: {role}",
                    StatusCode = 200,
                    data = new
                    {
                        Token = token,
                        AccessTokenExpiration = accessTokenRemainingTime,
                        RefreshToken = refreshToken,
                        RefreshTokenExpiration = refreshTokenRemainingTime
                    }
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

            var (isValid, userEmail) = await _authentication.ValidateRefreshToken(model.RefreshToken);

            if (!isValid)
            {
                return BadRequest("Invalid refresh token.");
            }

            var (roleValid, userRole) = await _authentication.CheckRole(userEmail);

            if (!roleValid)
            {
                return BadRequest("Invalid user role.");
            }

            var (token, accessTokenTime, refreshTokentime, newRefreshToken) = await _authentication.GenerateToken(userEmail, userRole);
            
            var accessTokenRemainingTime = accessTokenTime - DateTime.UtcNow;
            var refreshTokenRemainingTime = refreshTokentime - DateTime.UtcNow;
            
            await _authentication.SaveRefreshTokenToDatabase(userEmail, newRefreshToken);

            if (accessTokenTime < DateTime.UtcNow)
            {
                var expiredTokenResponse = new ApiResponse<object>
                {
                    message = "Access Token has expired.",
                    StatusCode = 401,
                    data = null
                };
                return Unauthorized(expiredTokenResponse);
            }

            var response = new ApiResponse<object>
            {
                message = "Token refreshed successfully.",
                StatusCode = 200,
                data = new
                {
                    Token = token,
                    AccessTokenExpiration = accessTokenRemainingTime,
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiration = refreshTokenRemainingTime
                }
            };

            return Ok(response);
        }
    }
}
