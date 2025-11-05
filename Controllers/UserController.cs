using Crime_Management_System.Models;
using Crime_Management_System.Services;
using Crime_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Crime_Management_System.DTOs;
using Crime_Management_System.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace Crime_Management_System.Controllers
{
    [Authorize]
    [AuthorizeRoles("Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;


        public UserController(IUserService userService)
        {
            _userService = userService;
        }



        // GET: api/user
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST: api/user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // You need the current admin username or some creator identifier
            var adminUsername = User.Identity?.Name ?? "system";

            var result = await _userService.CreateUserAsync(createUserDto, adminUsername);

            return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
        }



        // PUT: api/user/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
                return NotFound();

          // updte 
            if (!string.IsNullOrEmpty(dto.Email))
                existingUser.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.FullName))
                existingUser.FullName = dto.FullName;

            if (!string.IsNullOrEmpty(dto.Password))
            {
               
                var salt = BCrypt.Net.BCrypt.GenerateSalt();
                var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password, salt);
                existingUser.PasswordHash = hash;
                existingUser.Salt = salt;
            }

            if (dto.Role.HasValue)
                existingUser.Role = dto.Role.Value;

            if (dto.ClearanceLevel.HasValue)
                existingUser.ClearanceLevel = dto.ClearanceLevel.Value;

            existingUser.UpdatedAt = DateTime.UtcNow;

            await _userService.UpdateUserAsync(existingUser);
            return NoContent();
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
          
            if (id <= 0)
                return BadRequest(new { message = "Invalid user ID." });

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID {id} was not found." });

            await _userService.DeleteUserAsync(id);

            // Return a success message
            return Ok(new { message = $"User '{user.Username}' has been deleted successfully." });
        }


        // PUT: api/user/5/role
        [HttpPut("{id}/role")]
        public async Task<IActionResult> AssignRoleAndClearance(int id, [FromBody] RoleAssignmentDto dto)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userService.AssignRoleAndClearanceAsync(id, dto.Role, dto.ClearanceLevel);
            return NoContent();
            }

        }

            public class RoleAssignmentDto
              {
            public UserRole Role { get; set; }
            public int ClearanceLevel { get; set; }
            }
        }



