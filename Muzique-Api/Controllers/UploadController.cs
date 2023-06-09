﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Muzique_Api.Helpers;
using Muzique_Api.Models;

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

        [HttpPost("/uploadFile"), DisableRequestSizeLimit]
        public async Task<IActionResult> uploadFile(IFormFile? fileImage = null, IFormFile? fileSong = null)
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

        [HttpPost("/userUploadFile"), DisableRequestSizeLimit]
        public async Task<IActionResult> userUploadFile(Upload model)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(model.fileImage);
                var stream = new MemoryStream(bytes);
                IFormFile file = new FormFile(stream, 0, stream.Length, "avatar", "avatar.jpg");

                string filePathImage = "";

                Task<string> task = _fileSaver.FileSaveAsync(file, "assets/images");
                filePathImage = task.Result;

                return Ok(filePathImage);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
