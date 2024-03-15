﻿using System.Security.Claims;
using AutoMapper;
using BussinessObject;
using BussinessObject.AuthenModel;
using DataTransferObject.DTO;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using RequestEntity;
using Service.InterfaceService;
using Utils;

namespace LumosSolution.Controllers
{
    [Route("api/auth/")]
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
                StatusCode = ApiStatusCode.InternalServerError
            };

            try
            {
                string accessToken = Request.Headers.FirstOrDefault(h => h.Key.Equals("Authorization")).ToString();
                bool isUserSignedOut = await _authentication.SignOutAsync(accessToken);
                if (!isUserSignedOut)
                    return BadRequest(response);
                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;
            }
            catch (Exception ex)
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
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                    response.message = string.Join(" ", errors);
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return UnprocessableEntity(response);
                }

                var existingCustomer = await _customerService.GetCustomerByEmailAsync(model.Email);
                if (existingCustomer != null)
                {
                    response.message = MessagesResponse.Error.EmailAlreadyExists;
                    response.StatusCode = ApiStatusCode.Conflict;
                    return Conflict(response);
                }

                if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    response.message = MessagesResponse.Error.InvalidInput;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return UnprocessableEntity(response);
                }
                if (model.Password != model.ConfirmPassword)
                {
                    response.message = MessagesResponse.Error.PasswordMismatch;
                    response.StatusCode = ApiStatusCode.UnprocessableEntity;
                    return UnprocessableEntity(response);
                }

                var user = _mapper.Map<Customer>(model);
                var result = await _customerService.AddCustomerAsync(user);

                if (result)
                {
                    response.message = MessagesResponse.Success.RegisterSuccess;
                    response.StatusCode = ApiStatusCode.Created;
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
                Console.WriteLine(ex.Message);
                return UnprocessableEntity(response);
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
                    var newCustomer = new AddCustomerRequest
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
                        response.message = MessagesResponse.Success.RegisterSuccess;
                        response.StatusCode = ApiStatusCode.Created;
                    }
                    else
                    {
                        response.message = MessagesResponse.Error.OperationFailed;
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
                var username = payload.Name;
                var checkcustomer = await _customerService.GetCustomerByEmailAsync(userEmail);
                if (checkcustomer == null)
                {
                    response.message = MessagesResponse.Error.BannedAccount;
                    response.StatusCode = ApiStatusCode.Forbidden;
                    return BadRequest(response);
                }
                var (roleValid, userRole, userDetails) = await _authentication.GetUserdetailsInLoginGooogle(userEmail);

                if (!roleValid)
                {
                    response.message = MessagesResponse.Error.Unauthorized;
                    response.StatusCode = ApiStatusCode.Unauthorized;
                    return Unauthorized(response);
                }
                else
                {
                    await _authentication.UpdateLastLoginTimeAsync(userEmail);
                    var (token, accessTokenTime, refreshTokentime, refreshToken) = await _authentication.GenerateToken(userEmail, userRole);
                    if (string.IsNullOrEmpty(token))
                    {
                        response.message = MessagesResponse.Error.AccessTokenNotFound;
                        response.StatusCode = ApiStatusCode.BadRequest;
                        return BadRequest(response);
                    }
                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        response.message = MessagesResponse.Error.RefreshTokenNotFound;
                        response.StatusCode = ApiStatusCode.BadRequest;
                        return BadRequest(response);
                    }

                    response.message = MessagesResponse.Success.Completed;
                    response.StatusCode = ApiStatusCode.OK;
                    response.data = new
                    {
                        Username = username,
                        Token = token,
                        Userdetails = userDetails
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
            ApiResponse<object>? response = new();
            AuthenticateResponse res = new();
            try
            {
                if (string.IsNullOrEmpty(model.Password))
                {
                    // Nếu không nhập mật khẩu
                    response.message = MessagesResponse.Validation.PasswordRequired;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }
                else if (string.IsNullOrEmpty(model.Email))
                {
                    response.message = MessagesResponse.Validation.EmailRequired;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }

                res = await _authentication.IsUserAuthenticatedAsync(model.Email, model.Password);

                if (!res.Authenticated)
                {
                    if (res.isBanned)
                    {
                        response.message = "User is banned or cannot found";
                        response.data = new
                        {
                            user = "User is banned or cannot found"
                        };
                        return Unauthorized(response);
                    }

                    if (!res.emailExists)
                    {
                        response.message = MessagesResponse.Error.NotFound;
                        response.StatusCode = ApiStatusCode.NotFound;
                        response.data = new
                        {
                            password = "Email không đúng",
                        };
                    }
                    else if (!res.passwordCorrect)
                    {
                        response.message = MessagesResponse.Error.Unauthorized;
                        response.StatusCode = ApiStatusCode.Unauthorized;
                        response.data = new
                        {
                            password = "Password không đúng",
                        };
                    }
                    return Unauthorized(response);
                }

                var (token, accessTokenTime, refreshTokentime, refreshToken) = await _authentication.GenerateToken(model.Email, res.Role);
                if (string.IsNullOrEmpty(token))
                {
                    response.message = MessagesResponse.Error.AccessTokenNotFound;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }
                if (string.IsNullOrEmpty(refreshToken))
                {
                    response.message = MessagesResponse.Error.RefreshTokenNotFound;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }
                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;
                response.data = new
                {
                    userDetail = res.UserDetails,
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
[HttpPost("refreshtoken")]
public async Task<IActionResult> RefreshToken([FromBody] RefreshModel model)
{
    ApiResponse<object>? response = new ApiResponse<object>();
    try
    {
        var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userEmail))
        {
            response.message = MessagesResponse.Error.Unauthorized;
            response.StatusCode = ApiStatusCode.Unauthorized;
            return Unauthorized(response);
        }

        var (isValid, _) = await _authentication.ValidateRefreshTokenByEmail(userEmail);

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
        if (string.IsNullOrEmpty(token))
        {
            response.message = MessagesResponse.Error.AccessTokenNotFound;
            response.StatusCode = ApiStatusCode.BadRequest;
            return BadRequest(response);
        }
        if (string.IsNullOrEmpty(newRefreshToken))
        {
            response.message = MessagesResponse.Error.RefreshTokenNotFound;
            response.StatusCode = ApiStatusCode.BadRequest;
            return BadRequest(response);
        }
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
