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
                List<int> likeAlbumIdList = userService.GetListLikeAlbumId(userId);
                List<int> likePlaylistIdList = userService.GetListLikePlaylistId(userId);
                List<int> likeArtistIdList = userService.GetListLikeArtistId(userId);
                List<int> likeSongIdList = userService.GetListLikeSongId(userId);
                List<int> customizePlaylistIdList = userService.GetListCustomizePlaylistId(userId);
                List<object> recentAlbumIdList = userService.GetListHistoryAlbumId(userId);
                List<object> recentPlaylistIdList = userService.GetListHistoryPlaylistId(userId);
                List<object> recentArtistIdList = userService.GetListHistoryArtistId(userId);
                List<object> recentSongIdList = userService.GetListHistorySongId(userId);

                return Ok(new {user,likeAlbumIdList,likeArtistIdList,likePlaylistIdList,likeSongIdList,customizePlaylistIdList,recentSongIdList,recentPlaylistIdList,recentArtistIdList,recentAlbumIdList});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("/insertUserHistory")]
        [Authorize(Roles ="User")]
        public ActionResult InsertUserHistory(HistoryModel model)
        {
            try {
                var currentUser = GetCurrentUser();
                int userId = currentUser.userId;
                UserService userService = new UserService();
                if(model.type == "Song")
                {
                    HistorySong history = new HistorySong();
                    history.songId = model.objectId;
                    history.userId = userId;
                    history.createdAt = DateTime.Now;

                    if (!userService.InsertHistorySong(history)) return StatusCode(500, "Lỗi khi thêm vào lịch sử xem");
                }
                if(model.type == "Album")
                {
                    HistoryAlbum history = new HistoryAlbum();
                    history.albumId = model.objectId;
                    history.userId = userId;
                    history.createdAt = DateTime.Now;

                    if (!userService.InsertHistoryAlbum(history)) return StatusCode(500, "Lỗi khi thêm vào lịch sử xem");
                }
                if (model.type == "Artist")
                {
                    HistoryArtist history = new HistoryArtist();
                    history.artistId = model.objectId;
                    history.userId = userId;
                    history.createdAt = DateTime.Now;

                    if (!userService.InsertHistoryArtist(history)) return StatusCode(500, "Lỗi khi thêm vào lịch sử xem");
                }
                if (model.type == "Playlist")
                {
                    HistoryPlaylist history = new HistoryPlaylist();
                    history.playlistId = model.objectId;
                    history.userId = userId;
                    history.createdAt = DateTime.Now;

                    if (!userService.InsertHistoryPlaylist(history)) return StatusCode(500, "Lỗi khi thêm vào lịch sử xem");
                }

                return Ok();
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("/insertUserLike")]
        [Authorize(Roles = "User")]
        public ActionResult InsertUserLike(HistoryModel model)
        {
            try
            {
                var currentUser = GetCurrentUser();
                int userId = currentUser.userId;
                UserService userService = new UserService();
                if (model.type == "Song")
                {
                    LikeSong history = new LikeSong();
                    history.songId = model.objectId;
                    history.userId = userId;
                    history.createdAt = DateTime.Now;

                    if (!userService.InsertLikeSong(history)) return StatusCode(500, "Lỗi khi thêm vào danh sách yêu thích");
                }
                if (model.type == "Album")
                {
                    LikeAlbum history = new LikeAlbum();
                    history.albumId = model.objectId;
                    history.userId = userId;
                    history.createdAt = DateTime.Now;

                    if (!userService.InsertLikeAlbum(history)) return StatusCode(500, "Lỗi khi thêm vào danh sách yêu thích");
                }
                if (model.type == "Artist")
                {
                    LikeArtist history = new LikeArtist();
                    history.artistId = model.objectId;
                    history.userId = userId;
                    history.createdAt = DateTime.Now;

                    if (!userService.InsertLikeArtist(history)) return StatusCode(500, "Lỗi khi thêm vào danh sách yêu thích");
                }
                if (model.type == "Playlist")
                {
                    LikePlaylist history = new LikePlaylist();
                    history.playlistId = model.objectId;
                    history.userId = userId;
                    history.createdAt = DateTime.Now;

                    if (!userService.InsertLikePlaylist(history)) return StatusCode(500, "Lỗi khi thêm vào danh sách yêu thích");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
