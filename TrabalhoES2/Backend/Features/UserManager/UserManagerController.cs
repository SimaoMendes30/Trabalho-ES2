using Backend.Domain.DTOs.User;
using Backend.Features.User.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Features.UserManager
{
    [ApiController]
    [Route("api/usermanager")]
    [Authorize] // Opcional: [Authorize(Roles = "Admin,SuperUser")]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserManagerController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpPut("update-role/{id}")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateRoleDto dto)
        {
            await _userService.UpdateRoleAsync(id, dto.Admin, dto.SuperUser);
            return Ok();
        }
    }
}