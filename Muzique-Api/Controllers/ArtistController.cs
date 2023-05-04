using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Muzique_Api.Models;
using Muzique_Api.Services;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        [HttpGet("/getListArtist")]
        public ActionResult Get(int page, int rowperpage, string? keyword = "")
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
        public ActionResult GetArtistDetail(int id)
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

    }
}
