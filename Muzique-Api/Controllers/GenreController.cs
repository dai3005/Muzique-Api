using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Muzique_Api.Services;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        [HttpGet("/getListGenre")]
        public ActionResult Get(int page, int rowperpage, string? keyword = "")
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
        public ActionResult GetArtistDetail(int id)
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
    }
}
