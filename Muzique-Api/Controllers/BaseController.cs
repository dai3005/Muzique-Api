using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Muzique_Api.Helpers;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {

        private IWebHostEnvironment _env;
        private FileSaver _fileSaver;


        public BaseController(IWebHostEnvironment env)
        {
            _env = env;
            _fileSaver = new FileSaver(_env);
        }

        [HttpPost("UploadFile"), DisableRequestSizeLimit]
        public async Task<IActionResult> uploadImage(IFormFile fileImage, IFormFile? fileSong = null)
        {
            try 
            {
                _fileSaver.FileSaveAsync(fileImage, "assets/images");
                Task<string> task = _fileSaver.FileSaveAsync(fileImage, "assets/images");
                string filePathImage = task.Result;
                string filePathSong = "";
                if (fileSong != null)
                {
                    _fileSaver.FileSaveAsync(fileSong, "assets/images");
                    Task<string> taskSong = _fileSaver.FileSaveAsync(fileSong, "assets/songs");
                    filePathSong = taskSong.Result;
                }

                return Ok(new { filePathImage, filePathSong });
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
