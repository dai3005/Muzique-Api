using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Muzique_Api.Services;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        [HttpGet("/getListPlaylist")]
        public ActionResult Get(int page, int rowperpage, string? keyword = "")
        {
            try
            {
                PlaylistService playlistService = new PlaylistService();

                return Ok(playlistService.GetListPlaylist(page, rowperpage, keyword));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("/getPlaylistDetail")]
        public ActionResult GetPlaylistDetail(int id)
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
    }
}
