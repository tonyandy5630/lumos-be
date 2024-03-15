using BussinessObject;
using BussinessObject.AuthenModel;
using DataTransferObject.DTO;
using Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Repository.Interface.IUnitOfWork;
using RequestEntity;
using Service.InterfaceService;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.Service
{
    public class Authentication : IAuthentication
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public Authentication(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthenticateResponse> IsUserAuthenticatedAsync(string email, string password)
        {
            AuthenticateResponse res = new();
            try
            {
                var adminResponse = await _unitOfWork.AdminRepo.GetAdminByEmailAsync(email);
                if (adminResponse != null)
                {
                    return await GetAuthResponseFromUser(adminResponse, password);
                }

                var partnerResponse = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(email);
                if (partnerResponse != null)
                {
                    return await GetAuthResponseFromUser(partnerResponse, password);
                }

                var customerResponse = await _unitOfWork.CustomerRepo.GetCustomerByEmailAsync(email);
                if (customerResponse != null)
                {
                    return await GetAuthResponseFromUser(customerResponse, password);
                }

                res.isBanned = true;
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in IsUserAuthenticatedAsync: {ex.Message}");
                return res;
            }
        }

        private async Task<AuthenticateResponse> GetAuthResponseFromUser(object user, string password)
        {
            AuthenticateResponse res = new();
            try
            {
                if (user is Admin)
                {
                    Admin castUser = (Admin)user;
                    if (castUser.Password.Length > 60)
                    {
                        res.passwordCorrect = PasswordHelper.VerifyPassword(castUser, password, castUser.Password);
                    }
                    else
                    {
                        res.passwordCorrect = castUser.Password == password;
                    }
                    res.Role = nameof(RolesEnum.Admin);
                }
                else if (user is Partner)
                {
                    Partner castUser = (Partner)user;
                    if (castUser.Password.Length > 60)
                    {
                        res.passwordCorrect = PasswordHelper.VerifyPassword(castUser, password, castUser.Password);
                    }
                    else
                    {
                        res.passwordCorrect = castUser.Password == password;
                    }
                    res.Role = nameof(RolesEnum.Partner);
                }
                else
                {
                    Customer castUser = (Customer)user;
                    if (castUser.Password.Length > 60)
                    {
                        res.passwordCorrect = PasswordHelper.VerifyPassword(castUser, password, castUser.Password);
                    }
                    else
                    {
                        res.passwordCorrect = castUser.Password == password;
                    }
                    res.Role = nameof(RolesEnum.Customer);
                }

                if (!res.passwordCorrect)
                {
                    return res;
                }
                res.Authenticated = true;
                res.UserDetails = user;
                return await Task.FromResult(res);
            }
            catch
            {
                return res;
            }
        }


        public async Task<(string,DateTime, DateTime, string)> GenerateToken(string email, string role)
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
                var accessTokenexpiration = accessTokenOptions.ValidTo;
                var refreshTokenexpires = refreshTokenOptions.ValidTo;
                await SaveRefreshTokenToDatabase(email, refreshTokenString);
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
                var adminResponse = await _unitOfWork.AdminRepo.GetAdminByEmailAsync(email);
                if(adminResponse != null) {
                    adminResponse.RefreshToken = refreshToken;
                    await UpdateLastLoginTimeAsync(adminResponse);
                }

                var customerResponse = await _unitOfWork.CustomerRepo.GetCustomerByEmailAsync(email);
                if(customerResponse != null)
                {
                    customerResponse.RefreshToken = refreshToken;
                    await UpdateLastLoginTimeAsync(customerResponse);
                }

                var partnerResponse = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(email);
                if (partnerResponse != null)
                {
                    partnerResponse.RefreshToken = refreshToken;
                    await UpdateLastLoginTimeAsync(partnerResponse);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in SaveRefreshTokenToDatabase: {ex.Message}");
                throw;
            }
        }

        public async Task<(bool, string)> ValidateRefreshTokenByEmail(string email)
        {
            try
            {
                var adminResponse = await _unitOfWork.AdminRepo.GetAdminByEmailAsync(email);
                if (adminResponse != null && adminResponse.RefreshToken != null)
                {
                    return (true, adminResponse.RefreshToken);
                }

                var customerResponse = await _unitOfWork.CustomerRepo.GetCustomerByEmailAsync(email);
                if (customerResponse != null && customerResponse.RefreshToken != null)
                {
                    return (true, customerResponse.RefreshToken);
                }

                var partnerResponse = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(email);
                if (partnerResponse != null && partnerResponse.RefreshToken != null)
                {
                    return (true, partnerResponse.RefreshToken);
                }

                return (false, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in ValidateRefreshTokenByEmail: {ex.Message}");
                return (false, null);
            }
        }

        public async Task<(bool, string, object)> GetUserdetailsInLoginGooogle(string email)
        {
            try
            {
                object userDetails = null;
                var adminResponse = await _unitOfWork.AdminRepo.GetAdminByEmailAsync(email);
                var customerResponse = await _unitOfWork.CustomerRepo.GetCustomerByEmailAsync(email);
                var partnerResponse = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(email);

                if (adminResponse != null && adminResponse.Status == 1)
                {
                    userDetails = new
                    {
                        ID = adminResponse.AdminId,
                        Email = adminResponse.Email,
                        Code = adminResponse.Code,
                        Role = adminResponse.Role,
                        Status = adminResponse.Status,
                        CreatedDate = adminResponse.CreatedDate,
                        CreatedBy = adminResponse.CreatedBy,
                        LastUpdate = adminResponse.LastUpdate,
                        UpdatedBy = adminResponse.UpdatedBy,
                        ImgUrl = adminResponse.ImgUrl,
                    };
                    return (true, nameof(RolesEnum.Admin), userDetails);
                }
                else if (customerResponse != null && customerResponse.Status == 1)
                {
                    userDetails = new
                    {
                        ID = customerResponse.CustomerId,
                        Email = customerResponse.Email,
                        Code = customerResponse.Code,
                        Role = customerResponse.Role,
                        Phone = customerResponse.Phone,
                        Pronounce = customerResponse.Pronounce,
                        Status = customerResponse.Status,
                        CreatedDate = customerResponse.CreatedDate,
                        LastUpdate = customerResponse.LastUpdate,
                        UpdatedBy = customerResponse.UpdateBy,
                        ImgUrl = customerResponse.ImgUrl,
                    };
                    return (true, nameof(RolesEnum.Customer), userDetails);
                }
                else if (partnerResponse != null && partnerResponse.Status == 1)
                {
                    userDetails = new
                    {
                        ID = partnerResponse.PartnerId,
                        Email = partnerResponse.Email,
                        Code = partnerResponse.Code,
                        Role = partnerResponse.Role,
                        TypeId = partnerResponse.TypeId,
                        DisplayName = partnerResponse.DisplayName,
                        Phone = partnerResponse.Phone,
                        Address = partnerResponse.Address,
                        Status = partnerResponse.Status,
                        CreatedDate = partnerResponse.CreatedDate,
                        CreatedBy = partnerResponse.CreatedBy,
                        LastUpdate = partnerResponse.LastUpdate,
                        UpdatedBy = partnerResponse.UpdatedBy,
                        ImgUrl = partnerResponse.ImgUrl,
                        BusinessLicenseNumber = partnerResponse.BusinessLicenseNumber
                    };
                    return (true, nameof(RolesEnum.Partner), userDetails);
                }
                else
                {
                    return (false, null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetUserdetailsInLoginGooogle: {ex.Message}");
                return (false, null, null);
            }
        }
        public async Task<(bool, string)> CheckRole(string email)
        {
            try
            {
                var adminResponse = await _unitOfWork.AdminRepo.GetAdminByEmailAsync(email);
                var customerResponse = await _unitOfWork.CustomerRepo.GetCustomerByEmailAsync(email);
                var partnerResponse = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(email);

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

        public async Task UpdateLastLoginTimeAsync(object user)
        {
            try
            {
                DateTime now = DateConverter.GetUTCTime();
                
                switch (user)
                {
                    case Customer:
                        Customer customer = (Customer)user;
                        customer.LastLogin = now;
                        await _unitOfWork.CustomerRepo.UpdateCustomerAsync(customer);
                        break;

                    case Partner:
                        Partner partner = (Partner)user;
                        partner.LastLogin = now;
                        await _unitOfWork.PartnerRepo.UpdatePartnerAsync(partner);
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


        public async Task<bool> SignOutAsync(string accessToken)
        {
            try
            {
                var (email, roles) = await GetEmailAndRolesFromToken(accessToken);
                if (email.IsNullOrEmpty() || roles.IsNullOrEmpty())
                {
                    throw new Exception("cannot get email or roles");
                }

                string refreshToken = "";
                switch (roles)
                {
                    case nameof(RolesEnum.Admin):
                        Admin admin = await _unitOfWork.AdminRepo.GetAdminByEmailAsync(email);
                        if (admin == null)
                            throw new Exception("cannot find email");

                        refreshToken = await GetRefreshToken(admin.RefreshToken);
                        break;
                    case nameof(RolesEnum.Partner):
                        Partner partner = await _unitOfWork.PartnerRepo.GetPartnerByEmailAsync(email);
                        if (partner == null) throw new Exception("cannot find partner");
                        refreshToken = await GetRefreshToken(partner.RefreshToken);
                        break;
                    case nameof(RolesEnum.Customer):
                        Customer customer = await _unitOfWork.CustomerRepo.GetCustomerByEmailAsync(email);
                        if (customer == null) throw new Exception("cannot find customer");
                        refreshToken = await GetRefreshToken(customer.RefreshToken);
                        break;
                }

                return await Task.FromResult(false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<string> GetRefreshToken(string? refreshToken)
        {
            try
            {
                if (refreshToken == null || refreshToken.Length == 0)
                    throw new Exception();
                return await Task.FromResult(refreshToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot find user refresh token");
            }
        }

        private async Task<(string?, string?)> GetEmailAndRolesFromToken(string token)
        {
            string aToken = token.Split(" ")[1];
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken = handler.ReadJwtToken(aToken);

            string? email = decodedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string? role = decodedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return await Task.FromResult((email, role));
        }
        public static class PasswordHelper
        {
            private static readonly PasswordHasher<object> _passwordHasher = new PasswordHasher<object>();

            public static string HashPassword<TUser>(TUser user, string password) where TUser : class
            {
                return _passwordHasher.HashPassword(user, password);
            }

            public static bool VerifyPassword<TUser>(TUser user, string verifyPassword, string providedPassword) where TUser : class
            {
                PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, providedPassword, verifyPassword);
                return result == PasswordVerificationResult.Success;
            }
        }
    }
}
