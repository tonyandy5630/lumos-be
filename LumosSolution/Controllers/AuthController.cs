using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BussinessObject;
using BussinessObject.AuthenModel;
using DataTransferObject.DTO;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.InterfaceService;
using Utils;
using static Google.Apis.Requests.BatchRequest;

namespace LumosSolution.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAdminService _adminService;
        private readonly ICustomerService _customerService;
        private readonly IPartnerService _partnerService;
        private readonly IAuthentication _authentication;
        private readonly IMapper _mapper;
        public AuthController(IConfiguration configuration, IAdminService adminService,
        ICustomerService customerService, IPartnerService partnerService
        , IAuthentication authentication, IMapper mapper)
        {
            _configuration = configuration;
            _adminService = adminService;
            _customerService = customerService;
            _partnerService = partnerService;
            _authentication = authentication;
            _mapper = mapper;
        }

        [HttpPost("sign-out")]
        public async Task<ActionResult<ApiResponse<string>>> SignOut()
        {
            ApiResponse<string> response = new ApiResponse<string>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = 500
            };

            try
            {
                string accessToken = Request.Headers.FirstOrDefault(h => h.Key.Equals("Authorization")).ToString();
                bool isUserSignedOut = await _authentication.SignOutAsync(accessToken);
                if (!isUserSignedOut)
                    return BadRequest(response);
                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = 200;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel model)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    response.message = MessagesResponse.Error.InvalidInput;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }
                if (model.Password != model.ConfirmPassword)
                {

                    response.message = MessagesResponse.Error.OperationFailed;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }

                var user = _mapper.Map<Customer>(model);
                var result = await _customerService.AddCustomerAsync(user);

                if (result)
                {
                    response.message = MessagesResponse.Success.Completed;
                    response.StatusCode = ApiStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    response.message = MessagesResponse.Error.OperationFailed;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.message = MessagesResponse.Error.RegisterFailed;
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }

        }

        [HttpPost("google/register")]
        public async Task<IActionResult> RegisterGoogle([FromBody] string credential)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var setting = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _configuration["Authentication:Google:clientId"] }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(credential, setting);

                var userEmail = payload.Email;

                var (roleValid, userRole) = await _authentication.CheckRole(userEmail);

                if (!roleValid)
                {
                    var newCustomer = new CustomerServiceDTO
                    {
                        Email = userEmail,
                        Fullname = payload.Name,
                        ImgUrl = payload.Picture,
                        Password = "Passw0rd!"
                    };
                    var user = _mapper.Map<Customer>(newCustomer);
                    var result = await _customerService.AddCustomerAsync(user);

                    if (result)
                    {
                        response.message = MessagesResponse.Success.Completed;
                        response.StatusCode = ApiStatusCode.OK;
                    }
                    else
                    {
                        response.message = MessagesResponse.Error.RegisterFailed;
                        response.StatusCode = ApiStatusCode.BadRequest;
                    }
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = MessagesResponse.Error.RegisterFailed;
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }

        [HttpPost("google/login")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] string credential)
        {
            ApiResponse<object>? response = new ApiResponse<object>();
            try
            {
                var setting = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _configuration["Authentication:Google:clientId"] }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(credential, setting);

                var userEmail = payload.Email;

                var (roleValid, userRole) = await _authentication.CheckRole(userEmail);

                if (!roleValid)
                {
                    response.message = MessagesResponse.Error.Unauthorized;
                    response.StatusCode = ApiStatusCode.Unauthorized;
                    return Unauthorized(response);
                }
                else
                {
                    await _authentication.UpdateLastLoginTime(userEmail);
                    var (token, accessTokenTime, refreshTokentime, refreshToken) = await _authentication.GenerateToken(userEmail, userRole);

                    var accessTokenRemainingTime = accessTokenTime - DateTime.UtcNow;
                    var refreshTokenRemainingTime = refreshTokentime - DateTime.UtcNow;
                    
                    response.message = MessagesResponse.Success.Completed;
                    response.StatusCode = ApiStatusCode.OK;
                    response.data = new
                    {
                        Token = token,
                        AccessTokenExpiration = accessTokenRemainingTime,
                        RefreshToken = refreshToken,
                        RefreshTokenExpiration = refreshTokenRemainingTime
                    };
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = MessagesResponse.Error.LoginFailed;
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            ApiResponse<object>? response = new ApiResponse<object>();
            try
            {
                var (authenticated, role) = await _authentication.IsUserAuthenticatedAsync(model.Email, model.Password);

                if (authenticated)
                {
                    await _authentication.UpdateLastLoginTime(model.Email);
                    var (token, accessTokenTime, refreshTokentime, refreshToken) = await _authentication.GenerateToken(model.Email, role);

                    var accessTokenRemainingTime = accessTokenTime - DateTime.UtcNow;
                    var refreshTokenRemainingTime = refreshTokentime - DateTime.UtcNow;

                    response.message = MessagesResponse.Success.Completed;
                    response.StatusCode = ApiStatusCode.OK;
                    response.data = new
                    {
                        Token = token,
                        AccessTokenExpiration = accessTokenRemainingTime,
                        RefreshToken = refreshToken,
                        RefreshTokenExpiration = refreshTokenRemainingTime
                    };
                    return Ok(response);
                }
                else
                {
                    response.message = MessagesResponse.Error.Unauthorized;
                    response.StatusCode = ApiStatusCode.Unauthorized;
                    return Unauthorized(response);
                }

            }
            catch (Exception ex)
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }
        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshModel model)
        {
            ApiResponse<object>? response = new ApiResponse<object>();
            try
            {
                if (string.IsNullOrEmpty(model.RefreshToken))
                {
                    response.message = MessagesResponse.Error.InvalidInput;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }

                var (isValid, userEmail) = await _authentication.ValidateRefreshToken(model.RefreshToken);

                if (!isValid)
                {
                    response.message = MessagesResponse.Error.OperationFailed;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }

                var (roleValid, userRole) = await _authentication.CheckRole(userEmail);

                if (!roleValid)
                {
                    response.message = MessagesResponse.Error.OperationFailed;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }

                var (token, accessTokenTime, refreshTokentime, newRefreshToken) = await _authentication.GenerateToken(userEmail, userRole);

                var accessTokenRemainingTime = accessTokenTime - DateTime.UtcNow;
                var refreshTokenRemainingTime = refreshTokentime - DateTime.UtcNow;

                await _authentication.SaveRefreshTokenToDatabase(userEmail, newRefreshToken);

                if (accessTokenTime < DateTime.UtcNow)
                {
                    response.message = MessagesResponse.Error.Unauthorized;
                    response.StatusCode = ApiStatusCode.Unauthorized;
                    return Unauthorized(response);
                }

                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;
                response.data = new
                {
                    Token = token,
                    AccessTokenExpiration = accessTokenRemainingTime,
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiration = refreshTokenRemainingTime
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }
    }
}
