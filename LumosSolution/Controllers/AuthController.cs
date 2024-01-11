using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LumosSolution.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var (authenticated, role) = IsUserAuthenticated(model.Username, model.Password);

            if (authenticated)
            {
                // Nếu xác thực thành công, sinh ra token với role tương ứng
                var token = GenerateToken(model.Username, role);
                return Ok(new { Message = $"Logged in with role: {role}", Token = token });
            }

            return Unauthorized("login failure");
        }
        private (bool, string) IsUserAuthenticated(string username, string password)
        {
            if (username == "demo" && password == "password")
            {
                // User authenticated as "demo" with the role "admin"
                return (true, "admin");
            }
            else if (username == "demo1" && password == "password")
            {
                // User authenticated as "demo1" with the role "user"
                return (true, "user");
            }
            else
            {
                // User not authenticated
                return (false, null);
            }
        }
        [AllowAnonymous]
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleLoginCallback)),
                Items = { { "scheme", "Google" } }
            };

            return Challenge(authenticationProperties, "Google");
        }

        [AllowAnonymous]
        [HttpGet("google-login-callback")]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("External");
            if (!authenticateResult.Succeeded)
            {
                return Unauthorized("Google authentication failed.");
            }

            var username = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = "user"; // Set the role as needed

            var token = GenerateToken(username, role);
            return Ok(new { Message = $"Logged in with role: {role}", Token = token });
        }

        private string GenerateToken(string username, string role)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role) // Set the role based on the authenticated user
                // Các thông tin khác có thể được thêm vào đây
            };

            var tokenOptions = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return token;
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
