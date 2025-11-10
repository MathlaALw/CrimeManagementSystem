using Crime_Management_System.Models;
using Crime_Management_System.Services;
using Crime_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Crime_Management_System.DTOs;
using Crime_Management_System.Attributes;
using Microsoft.AspNetCore.Authorization;
using Azure.Core;
using Crime_Management_System.Services.Implementations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Crime_Management_System.Data;

namespace Crime_Management_System.Controllers
{
    [Authorize]
    [AuthorizeRoles("Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly CrimeDbContext _db;


        public UserController(IUserService userService ,CrimeDbContext crimeDbContext)
        {
            _userService = userService;
            _db = crimeDbContext;
        }



        // GET: api/user
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/user/5
        [HttpGet("GetUserByID")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST: api/user

        
        [HttpPost]
        [AuthorizeRoles("Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto  request)
        {
            if (request == null)
                return BadRequest(new { message = "Request body is required." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //email format
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(request.Email, emailPattern))
                return BadRequest(new { message = "Please User correct Email format" });


            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);
            if (existingUser != null)
            {
                if (existingUser.Username == request.Username)
                    return BadRequest(new { message = "UserName is avalible ,Please use another Name" });

                if (existingUser.Email == request.Email)
                    return BadRequest(new { message = "Email is avalible ,Please use another Name" });
            }

            // Map string to enum safely
            if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            {
                return BadRequest(new { message = "Invalid role value." });
            }

            // Generate salt
            var salt = UserService.GenerateSalt();

            // Hash password + salt
            var passwordHash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(request.Password + salt)
                )
            );

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = passwordHash,
                Salt = salt,  // ← MUST assign!
                Role = role,
                ClearanceLevel = (ClearanceLevel)request.ClearanceLevel,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "User registered successfully", userId = user.Id });
        }




        // PUT: api/user/5
        [HttpPut("UpdateUserByID")]
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
                // Generate salt
                var salt = UserService.GenerateSalt();

                // Hash password + salt
                var HashPassword = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(dto.Password + salt)));
          
                existingUser.PasswordHash = HashPassword;
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
        [HttpDelete("DeleteUser")]
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
        [HttpPut("role")]
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



