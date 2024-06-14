using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Web2API.Models;

namespace Web2API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminApiController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminApiController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // Phương thức để lấy danh sách người dùng
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users.Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email
            }).ToList();
            return Ok(users);
        }

        // Phương thức để xóa người dùng
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "User deleted successfully." });
                }
                else
                {
                    return BadRequest("Error deleting user.");
                }
            }
            else
            {
                return NotFound("User not found.");
            }
        }
    }
}
