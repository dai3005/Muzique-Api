using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Muzique_Api.Helpers;
using Muzique_Api.Models;
using Muzique_Api.Services;

namespace Muzique_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageUserController : ControllerBase
    {
        private IWebHostEnvironment _env;
        private DeleteFile _deleteFile;

        public ManageUserController(IWebHostEnvironment env)
        {
            _env = env;
            _deleteFile = new DeleteFile(_env);
        }
        [HttpGet("getListUser")]
        public IActionResult Get(int page, int rowperpage, string? keyword = "")
        {
            try
            {
                UserService userService = new UserService();

                return Ok(userService.GetListUser(page, rowperpage, keyword));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getUserById")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                UserService userService = new UserService();

                return Ok(userService.GetUserById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateUser(User model)
        {
            try
            {
                UserService userService = new UserService();
                User user = userService.GetUserById(model.userId);
                if (user == null) return StatusCode(500, "Người dùng không tồn tại");

                user.name = model.name;
                user.nameSearch = model.nameSearch;
                user.email = model.email;
                user.updatedAt = DateTime.Now;
                user.coverImageUrl = model.coverImageUrl;

                if (!string.IsNullOrEmpty(model.coverImageUrl))
                {
                    await _deleteFile.DeleteFileAsync(user.coverImageUrl);

                    user.coverImageUrl = model.coverImageUrl;
                }

                if (!userService.UpdateUser(user)) return StatusCode(500, "Lỗi khi sửa Người dùng");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
