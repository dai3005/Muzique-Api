using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Muzique_Api.Helpers;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {

        private IWebHostEnvironment _env;
        private FileSaver _fileSaver;


        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
            _fileSaver = new FileSaver(_env);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/uploadFile"), DisableRequestSizeLimit]
        public async Task<IActionResult> uploadFile(IFormFile? fileImage=null, IFormFile? fileSong = null)
        {
            try
            {
                string filePathImage = "";
                string filePathSong = "";
                if (fileImage != null)
                {
                    Task<string> task = _fileSaver.FileSaveAsync(fileImage, "assets/images");
                    filePathImage = task.Result;
                }
                          
                if (fileSong != null)
                {
                    
                    Task<string> taskSong = _fileSaver.FileSaveAsync(fileSong, "assets/songs");
                    filePathSong = taskSong.Result;
                }

                return Ok(new { filePathImage, filePathSong });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
