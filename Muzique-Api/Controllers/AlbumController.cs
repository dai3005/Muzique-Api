using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Muzique_Api.Helpers;
using Muzique_Api.Models;
using Muzique_Api.Services;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private IWebHostEnvironment _env;
        private DeleteFile _deleteFile;

        public AlbumController(IWebHostEnvironment env)
        {
            _env = env;
            _deleteFile = new DeleteFile(_env);
        }

        [HttpGet("/getListAlbum")]
        public IActionResult Get(int page, int rowperpage, string? keyword = "")
        {
            try
            {
                AlbumService albumService = new AlbumService();
                AlbumViewModel albumViewModel = albumService.GetListAlbum(page, rowperpage, keyword);
                for (int i = 0; i < albumViewModel.ListData.Count; i++)
                {
                    int playlistId = albumViewModel.ListData[i].albumId;
                    List<int>? listSongIds = albumService.GetListSongByAlbumId(playlistId);

                    albumViewModel.ListData[i].listSongId = listSongIds;
                }
                return Ok(albumViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("/getAlbumDetail")]
        public IActionResult GetAlbumDetail(int id)
        {
            try
            {
                AlbumService albumService = new AlbumService();

                return Ok(albumService.GetAlbumDetail(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("/getAlbumById")]
        public IActionResult GetAlbumById(int id)
        {
            try
            {
                AlbumService albumService = new AlbumService();

                return Ok(albumService.GetAlbumById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("/createAlbum")]
        public IActionResult CreateAlbum(Album model)
        {
            try
            {
                AlbumService albumService = new AlbumService();
                Album album = new Album();
                album.name = model.name;
                album.nameSearch = model.nameSearch;
                album.description = model.description;
                album.coverImageUrl = model.coverImageUrl;
                album.createdAt = DateTime.Now;
                album.artistId = model.artistId;


                if (!albumService.InsertAlbum(album)) return StatusCode(500, "Lỗi khi thêm Album");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("/updateAlbum")]
        public async Task<IActionResult> UpdateAlbum(Album model)
        {
            try
            {
                AlbumService albumService = new AlbumService();
                Album album = albumService.GetAlbumDetail(model.albumId);
                if (album == null) return StatusCode(500, "Ca sĩ không tồn tại");

                album.name = model.name;
                album.nameSearch = model.nameSearch;
                album.description = model.description;
                album.updatedAt = DateTime.Now;
                album.artistId = model.artistId;

                if (!string.IsNullOrEmpty(model.coverImageUrl))
                {
                    await _deleteFile.DeleteFileAsync(album.coverImageUrl);

                    album.coverImageUrl = model.coverImageUrl;
                }

                if (!albumService.UpdateAlbum(album)) return StatusCode(500, "Lỗi khi sửa Album");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("/deleteAlbum")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        AlbumService albumService = new AlbumService(connect);

                        Album album = albumService.GetAlbumDetail(id, transaction);
                        if (album == null) return NotFound();

                        if (!albumService.DeleteAlbumSong(id, transaction)) return StatusCode(500, "Lỗi khi xoá ở bảng chung bài hát");

                        await _deleteFile.DeleteFileAsync(album.coverImageUrl);

                        if (!albumService.DeleteAlbum(id, transaction)) return StatusCode(500, "Lỗi khi xoá album");

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
