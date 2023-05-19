using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Muzique_Api.Models;
using Muzique_Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        public UserController(IConfiguration config)
        {
            _config = config;
        }
        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,user.email),
                new Claim(ClaimTypes.NameIdentifier,user.userId.ToString()),
                new Claim(ClaimTypes.Role,"User")
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMonths(12),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        private User GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new User
                {
                    email = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                    userId = Int32.Parse(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value),
                };
            }
            return null;
        }

        [AllowAnonymous]
        [HttpPost("/Login")]
        public ActionResult Login(UserLogin model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.email) || string.IsNullOrEmpty(model.password)) return StatusCode(500, "Email hoặc mật khẩu rỗng!");

                UserService userService = new UserService();
                User user = userService.GetUserByEmail(model.email);
                if (user != null)
                {
                    if (!user.password.Equals(model.password)) return StatusCode(500, "Mật khẩu không đúng!");
                    var token = GenerateToken(user);
                    return Ok(token);
                }
                else
                {
                    return NotFound("Không tìm thấy tài khoản");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult AdminEndPoint()
        {
            var currentUser = GetCurrentUser();
            string email = currentUser.email;
            int userId = currentUser.userId;
            return Ok(new { email, userId });
        }

    }
}
