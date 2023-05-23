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
        [HttpPost("/login")]
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

                    Response.Cookies.Append("jwt", token);
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

        [HttpGet("/getUser")]
        [Authorize(Roles = "User")]
        public ActionResult GetUser()
        {
            try
            {
                var currentUser = GetCurrentUser();
                int userId = currentUser.userId;
                UserService userService = new UserService();
                User user = userService.GetUserById(userId);
                List<string> likeAlbumIdList = userService.GetListLikeAlbumId(userId);
                List<string> likePlaylistIdList = userService.GetListLikePlaylistId(userId);
                List<string> likeArtistIdList = userService.GetListLikeArtistId(userId);
                List<string> likeSongIdList = userService.GetListLikeSongId(userId);
                List<string> customizePlaylistIdList = userService.GetListCustomizePlaylistId(userId);
                List<string> recentAlbumIdList = userService.GetListHistoryAlbumId(userId);
                List<string> recentPlaylistIdList = userService.GetListHistoryPlaylistId(userId);
                List<string> recentArtistIdList = userService.GetListHistoryArtistId(userId);
                List<string> recentSongIdList = userService.GetListHistorySongId(userId);

                return Ok(new {user,likeAlbumIdList,likeArtistIdList,likePlaylistIdList,likeSongIdList,customizePlaylistIdList,recentAlbumIdList,recentArtistIdList,recentPlaylistIdList,recentSongIdList});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
