using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Muzique_Api.Helpers;
using Muzique_Api.Models;
using Muzique_Api.Services;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private IWebHostEnvironment _env;
        private DeleteFile _deleteFile;

        public GenreController(IWebHostEnvironment env)
        {
            _env = env;
            _deleteFile = new DeleteFile(_env);
        }

        [HttpGet("/getListGenre")]
        public IActionResult Get(int page, int rowperpage, string? keyword = "")
        {
            try
            {
                GenreService genreService = new GenreService();

                return Ok(genreService.GetListGenre(page, rowperpage, keyword));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("/getGenreDetail")]
        public IActionResult GetGenreDetail(int id)
        {
            try
            {
                GenreService genreService = new GenreService();

                return Ok(genreService.GetGenreDetail(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("/getGenreById")]
        public IActionResult GetGenreById(int id)
        {
            try
            {
                GenreService genreService = new GenreService();

                return Ok(genreService.GetGenreById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("/createGenre")]
        public IActionResult CreateGenre(Genre model)
        {
            try
            {
                GenreService genreService = new GenreService();
                Genre genre = new Genre();
                genre.name = model.name;
                genre.nameSearch = model.nameSearch;
                genre.description = model.description;
                genre.coverImageUrl = model.coverImageUrl;
                genre.createdAt = DateTime.Now;

                if (!genreService.InsertGenre(genre)) return StatusCode(500, "Lỗi khi thêm Thể loại");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("/updateGenre")]
        public async Task<IActionResult> UpdateGenre(Genre model)
        {
            try
            {
                GenreService genreService = new GenreService();
                Genre genre = genreService.GetGenreDetail(model.genreId);
                if (genre == null) return StatusCode(500, "Thể loại không tồn tại");

                genre.name = model.name;
                genre.nameSearch = model.nameSearch;
                genre.description = model.description;
                genre.updatedAt = DateTime.Now;
                genre.coverImageUrl = model.coverImageUrl;

                if (!string.IsNullOrEmpty(model.coverImageUrl))
                {
                    await _deleteFile.DeleteFileAsync(genre.coverImageUrl);

                    genre.coverImageUrl = model.coverImageUrl;
                }

                if (!genreService.UpdateGenre(genre)) return StatusCode(500, "Lỗi khi sửa Thể loại");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("/deleteGenre")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        GenreService genreService = new GenreService(connect);

                        Genre genre = genreService.GetGenreDetail(id, transaction);
                        if (genre == null) return NotFound();

                        if (!genreService.DeleteGenreSong(id, transaction)) return StatusCode(500, "Lỗi khi xoá ở bảng chung bài hát");

                        await _deleteFile.DeleteFileAsync(genre.coverImageUrl);

                        if (!genreService.DeleteGenre(id, transaction)) return StatusCode(500, "Lỗi khi xoá Thể loại");

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
