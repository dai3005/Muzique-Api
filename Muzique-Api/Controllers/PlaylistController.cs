using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Muzique_Api.Helpers;
using Muzique_Api.Models;
using Muzique_Api.Services;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private IWebHostEnvironment _env;
        private DeleteFile _deleteFile;

        public PlaylistController(IWebHostEnvironment env)
        {
            _env = env;
            _deleteFile = new DeleteFile(_env);
        }
        [HttpGet("/getListPlaylist")]
        public IActionResult Get(int page, int rowperpage, string? keyword = "")
        {
            try
            {
                PlaylistService playlistService = new PlaylistService();
                PlaylistViewModel playlistViewModel = playlistService.GetListPlaylist(page, rowperpage, keyword);
                for(int i = 0; i < playlistViewModel.ListData.Count; i++)
                {
                    int playlistId = playlistViewModel.ListData[i].playlistId;
                    List<int>? listSongIds = playlistService.GetListSongByPlaylistId(playlistId);

                    playlistViewModel.ListData[i].listSongId = listSongIds;
                }

                return Ok(playlistViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("/getPlaylistById")]
        public IActionResult GetPlaylistById(int id)
        {
            try
            {
                PlaylistService playlistService = new PlaylistService();

                return Ok(playlistService.GetPlaylistById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("/getPlaylistDetail")]
        public IActionResult GetPlaylistDetail(int id)
        {
            try
            {
                PlaylistService playlistService = new PlaylistService();

                return Ok(playlistService.GetPlaylistDetail(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("/createPlaylist")]
        public IActionResult CreatePlaylist(Playlist model)
        {
            try
            {
                PlaylistService playlistService = new PlaylistService();
                Playlist playlist = new Playlist();
                playlist.name = model.name;
                playlist.nameSearch = model.nameSearch;
                playlist.description = model.description;
                playlist.coverImageUrl = model.coverImageUrl;
                playlist.createdAt = DateTime.Now;

                if (!playlistService.InsertPlaylist(playlist)) return StatusCode(500, "Lỗi khi thêm Playlist");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("/updatePlaylist")]
        public async Task<IActionResult> UpdatePlaylist(Playlist model)
        {
            try
            {
                PlaylistService playlistService = new PlaylistService();
                Playlist playlist = playlistService.GetPlaylistById(model.playlistId);
                if (playlist == null) return StatusCode(500, "Playlist không tồn tại");

                playlist.name = model.name;
                playlist.nameSearch = model.nameSearch;
                playlist.description = model.description;
                playlist.updatedAt = DateTime.Now;
                playlist.coverImageUrl = model.coverImageUrl;

                if (!string.IsNullOrEmpty(model.coverImageUrl))
                {
                    await _deleteFile.DeleteFileAsync(playlist.coverImageUrl);

                    playlist.coverImageUrl = model.coverImageUrl;
                }

                if (!playlistService.UpdatePlaylist(playlist)) return StatusCode(500, "Lỗi khi sửa Playlist");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("/deletePlaylist")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        PlaylistService playlistService = new PlaylistService(connect);

                        Playlist playlist = playlistService.GetPlaylistById(id, transaction);
                        if (playlist == null) return NotFound();

                        if (!playlistService.DeletePlaylistSong(id, transaction)) return StatusCode(500, "Lỗi khi xoá ở bảng chung bài hát");

                        await _deleteFile.DeleteFileAsync(playlist.coverImageUrl);

                        if (!playlistService.DeletePlaylist(id, transaction)) return StatusCode(500, "Lỗi khi xoá playlist");

                        transaction.Commit();
                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
