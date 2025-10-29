using Crime_Management_System.Attributes;
using Crime_Management_System.Data;
using Crime_Management_System.Helper;
using Crime_Management_System.Models;
using Crime_Management_System.Servises;
using Crime_Management_System.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Crime_Management_System.Controllers
{

        // Controllers/AuthController.cs
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
            private readonly JwtService _jwtService;
            private readonly CrimeDbContext _context;

            public AuthController(JwtService jwtService, CrimeDbContext context)
            {
                _jwtService = jwtService;
                _context = context;
            }

            [HttpPost("login")]
            [AllowAnonymous]
            public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
            {
                // Find user by username
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                // In real implementation, you'd retrieve the salt from database
                // For demo, we'll assume salt is stored with user or use a fixed approach
                byte[] salt = GetUserSalt(user); // You need to implement this based on your storage

                // Verify password
                if (!PasswordHelper.VerifyPassword(request.Password, user.PasswordHash, salt))
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                // Generate token
                var token = _jwtService.GenerateToken(user);

                return Ok(new AuthResponse
                {
                    Token = token,
                    Role = user.Role.ToString(),
                    ClearanceLevel = user.ClearanceLevel.ToString(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                });
            }

            [HttpPost("register")]
            [AuthorizeRoles("Admin")]
            public async Task<ActionResult> Register([FromBody] UserRegistrationRequest request)
            {
                // Check if username exists
                if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                {
                    return BadRequest(new { message = "Username already exists" });
                }

                // Generate salt and hash password
                byte[] salt = PasswordHelper.GenerateSalt();
                string passwordHash = PasswordHelper.HashPassword(request.Password, salt);

                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = passwordHash,
                    Role = Enum.Parse<UserRole>(request.Role, true), // Convert string to UserRole enum
                    ClearanceLevel = Enum.Parse<ClearanceLevel>(request.ClearanceLevel, true), // Convert string to ClearanceLevel enum
                    IsActive = true
                };

                // Store salt with user (you need to add Salt field to User model)
                // user.Salt = Convert.ToBase64String(salt);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User registered successfully" });
            }

            private byte[] GetUserSalt(User user)
            {
                // Implement based on how you store salt
                // This is a placeholder - you need to store and retrieve salt properly
                return Convert.FromBase64String("default-salt-base64-here");
            }
        }
    }

