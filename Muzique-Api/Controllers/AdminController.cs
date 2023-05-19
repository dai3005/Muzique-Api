using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Muzique_Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AdminController(IConfiguration config)
        {
            _config = config;
        }
        private string GenerateAdminToken(Admin admin)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,admin.name),
                new Claim(ClaimTypes.Role,"Admin")
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

        [HttpPost("/adminLogin")]
        public ActionResult Login(Admin model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.name) || string.IsNullOrEmpty(model.password)) return StatusCode(500, "Tài khoản hoặc mật khẩu rỗng!");


                if (model.name == "admin" && model.password == "admin")
                {
                    var token = GenerateAdminToken(model);
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
    }
}
