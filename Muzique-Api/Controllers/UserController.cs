using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Muzique_Api.Helpers;
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
        [HttpPost("/signUp")]
        public ActionResult SignUp(User model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.email) || string.IsNullOrEmpty(model.password)) return StatusCode(500, "Email hoặc mật khẩu rỗng!");
                if (model.password.Length < 8) return StatusCode(500, "weak-password");
                UserService userService = new UserService();
                User userCheck = userService.GetUserByEmail(model.email);
                if (userCheck != null)
                {
                    return StatusCode(500, "Email already exists");
                }
                else
                {
                    User user = new User();
                    user.email = model.email;
                    user.password = model.password;
                    user.name = model.name;
                    user.nameSearch = Helper.RemoveUnicode(model.name);
                    user.createdAt = DateTime.Now;
                    user.coverImageUrl = model.coverImageUrl;

                    if (!userService.InsertUser(user)) return StatusCode(500, "Create User Error");
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

        [HttpPost("/userUpdate")]
        [Authorize(Roles = "User")]
        public ActionResult UserUpdate(UserUpdate model)
        {
            try
            {
                var currentUser = GetCurrentUser();
                int userId = currentUser.userId;
                UserService userService = new UserService();
                UserUpdate userUpdate = new UserUpdate();
                userUpdate.userId = userId;
                userUpdate.updatedAt = DateTime.Now;
                if (!string.IsNullOrEmpty(model.coverImageUrl))
                {
                    userUpdate.coverImageUrl = model.coverImageUrl;
                    if (!userService.UserUpdateImage(userUpdate)) return StatusCode(500, "Lỗi khi sửa ảnh");
                }
                if (!string.IsNullOrEmpty(model.name))
                {
                    userUpdate.name = model.name;
                    userUpdate.nameSearch = Helper.RemoveUnicode(model.name);
                    if (!userService.UserUpdateName(userUpdate)) return StatusCode(500, "Lỗi khi sửa tên");
                }
                return Ok();
            }
            catch (Exception ex) { return BadRequest(ex); }
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

                return Ok(new { user, likeAlbumIdList, likeArtistIdList, likePlaylistIdList, likeSongIdList, customizePlaylistIdList, recentSongIdList, recentPlaylistIdList, recentArtistIdList, recentAlbumIdList });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/insertUserHistory")]
        [Authorize(Roles = "User")]
        public ActionResult InsertUserHistory(HistoryModel model)
        {
            try
            {
                var currentUser = GetCurrentUser();
                int userId = currentUser.userId;
                UserService userService = new UserService();
                if (model.type == "Song")
                {
                    HistorySong history = new HistorySong();
                    history.songId = model.objectId;
                    history.userId = userId;
                    history.createdAt = DateTime.Now;

                    if (!userService.InsertHistorySong(history)) return StatusCode(500, "Lỗi khi thêm vào lịch sử xem");

                }
                if (model.type == "Album")
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
                    var playlistUser = userService.GetHistoryPlaylistUser(history);

                    if (!userService.InsertHistoryPlaylist(history)) return StatusCode(500, "Lỗi khi thêm vào lịch sử xem");

                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/deleteUserHistory")]
        [Authorize(Roles = "User")]
        public ActionResult DeleteUserHistory(HistoryModel model)
        {
            try
            {
                var currentUser = GetCurrentUser();
                int userId = currentUser.userId;
                UserService userService = new UserService();
                if (model.type == "Song")
                {
                    HistorySong history = new HistorySong();
                    history.songId = model.objectId;
                    history.userId = userId;

                    var songUser = userService.GetHistorySongUser(history);
                    if (songUser != null)
                    {
                        if (!userService.DeleteHistorySong(history)) return StatusCode(500, "Lỗi khi thêm vào lịch sử xem");
                    }
                }
                if (model.type == "Album")
                {
                    HistoryAlbum history = new HistoryAlbum();
                    history.albumId = model.objectId;
                    history.userId = userId;

                    var albumUser = userService.GetHistoryAlbumUser(history);
                    if (albumUser != null)
                    {
                        if (!userService.DeleteHistoryAlbum(history)) return StatusCode(500, "Lỗi khi thêm vào lịch sử xem");
                    }
                }
                if (model.type == "Artist")
                {
                    HistoryArtist history = new HistoryArtist();
                    history.artistId = model.objectId;
                    history.userId = userId;

                    var artistUser = userService.GetHistoryArtistUser(history);
                    if (artistUser != null)
                    {
                        if (!userService.DeleteHistoryArtist(history)) return StatusCode(500, "Lỗi khi thêm vào lịch sử xem");
                    }
                }
                if (model.type == "Playlist")
                {
                    HistoryPlaylist history = new HistoryPlaylist();
                    history.playlistId = model.objectId;
                    history.userId = userId;
                    var playlistUser = userService.GetHistoryPlaylistUser(history);
                    if (playlistUser != null)
                    {
                        if (!userService.DeleteHistoryPlaylist(history)) return StatusCode(500, "Lỗi khi thêm vào lịch sử xem");
                    }
                }

                return Ok();
            }
            catch (Exception ex)
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

        [HttpPost("/deleteUserLike")]
        [Authorize(Roles = "User")]
        public ActionResult DeleteUserLike(HistoryModel model)
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

                    if (!userService.DeleteUserLikeSong(history)) return StatusCode(500, "Lỗi khi xoá khỏi danh sách yêu thích");
                }
                if (model.type == "Album")
                {
                    LikeAlbum history = new LikeAlbum();
                    history.albumId = model.objectId;
                    history.userId = userId;
                    history.createdAt = DateTime.Now;

                    if (!userService.DeleteUserLikeAlbum(history)) return StatusCode(500, "Lỗi khi xoá khỏi danh sách yêu thích");
                }
                if (model.type == "Artist")
                {
                    LikeArtist history = new LikeArtist();
                    history.artistId = model.objectId;
                    history.userId = userId;
                    history.createdAt = DateTime.Now;

                    if (!userService.DeleteUserLikeArtist(history)) return StatusCode(500, "Lỗi khi xoá khỏi danh sách yêu thích");
                }
                if (model.type == "Playlist")
                {
                    LikePlaylist history = new LikePlaylist();
                    history.playlistId = model.objectId;
                    history.userId = userId;
                    history.createdAt = DateTime.Now;

                    if (!userService.DeleteUserLikePlaylist(history)) return StatusCode(500, "Lỗi khi xoá khỏi danh sách yêu thích");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("/updateUserPlaylist")]
        [Authorize(Roles = "User")]
        public ActionResult UpdateUserPlaylist(Playlist model)
        {
            try
            {
                var currentUser = GetCurrentUser();
                int userId = currentUser.userId;
                PlaylistService playlistService = new PlaylistService();
                Playlist playlist = playlistService.GetPlaylistById(model.playlistId);
                if (playlist == null) return StatusCode(500, "Playlist không tồn tại");
                playlist.userId = userId;
                playlist.updatedAt = DateTime.Now;
                if (!string.IsNullOrEmpty(model.name))
                {
                    playlist.name = model.name;
                    if (!playlistService.UpdatePlaylistName(playlist)) return StatusCode(500, "Lỗi khi sửa tên Playlist");
                }

                if (!string.IsNullOrEmpty(model.coverImageUrl))
                {
                    playlist.coverImageUrl = model.coverImageUrl;
                    if (!playlistService.UpdatePlaylistImage(playlist)) return StatusCode(500, "Lỗi khi sửa ảnh Playlist");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/createUserPlaylist")]
        [Authorize(Roles = "User")]
        public IActionResult CreateUserPlaylist(Playlist model)
        {
            try
            {
                var currentUser = GetCurrentUser();
                int userId = currentUser.userId;
                PlaylistService playlistService = new PlaylistService();
                Playlist playlist = new Playlist();
                playlist.name = model.name;
                playlist.nameSearch = Helper.RemoveUnicode(model.name);
                playlist.description = model.description;
                playlist.coverImageUrl = model.coverImageUrl;
                playlist.createdAt = DateTime.Now;
                playlist.type = model.type;
                playlist.userId = userId;

                if (!playlistService.InsertPlaylist(playlist)) return StatusCode(500, "Lỗi khi thêm Playlist");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("/userSearch")]
        public IActionResult UserSearch(string keyword)
        {
            try
            {
                //Song
                SongService songService = new SongService();
                SongViewModel song =  songService.GetListSong(1, 10, keyword);
                List<Song> listSong = song.ListSong;

                //Playlist
                PlaylistService playlistService = new PlaylistService();
                PlaylistViewModel playlistViewModel = playlistService.GetListPlaylist(1, 10, keyword);
                for (int i = 0; i < playlistViewModel.ListData.Count; i++)
                {
                    int playlistId = playlistViewModel.ListData[i].playlistId;
                    List<int>? listSongIds = playlistService.GetListSongByPlaylistId(playlistId);

                    playlistViewModel.ListData[i].listSongId = listSongIds;
                }
                List<Playlist> listPlaylist = playlistViewModel.ListData;

                //Artist
                ArtistService artistService = new ArtistService();
                ArtistViewModel artistViewModel = artistService.GetListArtist(1, 10, keyword);
                for (int i = 0; i < artistViewModel.ListData.Count; i++)
                {
                    int playlistId = artistViewModel.ListData[i].artistId;
                    List<int>? listSongIds = artistService.GetListSongByArtistId(playlistId);

                    artistViewModel.ListData[i].listSongId = listSongIds;
                }
                List<Artist> listArtist = artistViewModel.ListData;

                //Album
                AlbumService albumService = new AlbumService();
                AlbumViewModel albumViewModel = albumService.GetListAlbum(1, 10, keyword);
                for (int i = 0; i < albumViewModel.ListData.Count; i++)
                {
                    int playlistId = albumViewModel.ListData[i].albumId;
                    List<int>? listSongIds = albumService.GetListSongByAlbumId(playlistId);

                    albumViewModel.ListData[i].listSongId = listSongIds;
                }
                List<Album> listAlbum = albumViewModel.ListData;

                return Ok(new {listSong,listAlbum,listArtist,listPlaylist });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
