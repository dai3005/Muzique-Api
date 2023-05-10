using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Muzique_Api.Helpers;
using Muzique_Api.Models;
using Muzique_Api.Services;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private IWebHostEnvironment _env;
        private DeleteFile _deleteFile;

        public ArtistController(IWebHostEnvironment env)
        {
            _env = env;
            _deleteFile = new DeleteFile(_env);
        }

        [HttpGet("/getListArtist")]
        public IActionResult Get(int page, int rowperpage, string? keyword = "")
        {
            try
            {
                ArtistService artistService = new ArtistService();

                return Ok(artistService.GetListArtist(page, rowperpage, keyword));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("/getArtistDetail")]
        public IActionResult GetArtistDetail(int id)
        {
            try
            {
                ArtistService artistService = new ArtistService();

                return Ok(artistService.GetArtistDetail(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("/createArtist")]
        public IActionResult CreateArtist(Artist model)
        {
            try { 
                ArtistService artistService = new ArtistService();
                Artist artist = new Artist();
                artist.name = model.name;
                artist.nameSearch = model.nameSearch;
                artist.description = model.description;
                artist.coverImageUrl = model.coverImageUrl;
                artist.createdAt = DateTime.Now;

                if (!artistService.InsertArtist(artist)) return StatusCode(500, "Lỗi khi thêm Ca sĩ");
                return Ok(); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
        [HttpPut("/updateArtist")]
        public async Task<IActionResult> UpdateArtist(Artist model)
        {
            try
            {
                ArtistService artistService = new ArtistService();
                Artist artist = artistService.GetArtistDetail(model.artistId);
                if (artist == null) return StatusCode(500, "Ca sĩ không tồn tại");

                artist.name = model.name;
                artist.nameSearch = model.nameSearch;
                artist.description = model.description;
                artist.updatedAt = DateTime.Now;
                artist.coverImageUrl = model.coverImageUrl;

                if (!string.IsNullOrEmpty(model.coverImageUrl))
                {
                    await _deleteFile.DeleteFileAsync(artist.coverImageUrl);

                    artist.coverImageUrl = model.coverImageUrl;
                }

                if (!artistService.UpdateArtist(artist)) return StatusCode(500, "Lỗi khi sửa Ca sĩ");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("/deleteArtist")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        ArtistService artistService = new ArtistService(connect);

                        Artist artist = artistService.GetArtistDetail(id, transaction);
                        if (artist == null) return NotFound();

                        if (!artistService.DeleteArtistSong(id, transaction)) return StatusCode(500, "Lỗi khi xoá ở bảng chung bài hát");

                        await _deleteFile.DeleteFileAsync(artist.coverImageUrl);

                        if (!artistService.DeleteArtist(id, transaction)) return StatusCode(500, "Lỗi khi xoá ca sĩ");

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
