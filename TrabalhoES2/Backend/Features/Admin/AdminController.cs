using Backend.Domain.DTOs.User;
using Backend.Features.User.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Features.Admin
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]          // só administradores
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        /*────────── READ ──────────*/

        // GET api/admin/users      → devolve a lista completa
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDetailsDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // GET api/admin/users/{id} → devolve um utilizador específico
        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserDetailsDto>> GetUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return user is null 
                ? NotFound() 
                : Ok(user);
        }

        /*────────── WRITE ─────────*/

        // POST api/admin/users
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
        {
            await _userService.CreateAsync(dto);
            return Ok();
        }

        // PUT api/admin/users/{id}
        [HttpPut("users/{id}")]
        public async Task<IActionResult> EditUser(int id, [FromBody] UserUpdateDto dto)
        {
            await _userService.UpdateAsync(id, dto);
            return Ok();
        }

        // DELETE api/admin/users/{id}
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteAsync(id);
            return Ok();
        }
    }
}
