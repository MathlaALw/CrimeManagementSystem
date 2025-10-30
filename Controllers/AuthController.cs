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
            private readonly IPasswordService _passwords;
            private readonly ITokenService _tokens;

        public AuthController(JwtService jwtService, CrimeDbContext context, IPasswordService passwords, ITokenService tokens)
            {
                _jwtService = jwtService;
                _context = context;
                _passwords = passwords;
                _tokens = tokens;


        }

            [HttpPost("login")]
            [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UsernameOrEmail) || string.IsNullOrWhiteSpace(dto.Password))
                return Unauthorized();

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail);

            if (user == null || !user.IsActive)
                return Unauthorized();

            if (!_passwords.Verify(user.PasswordHash, dto.Password))
                return Unauthorized();

            var (token, expires) = _tokens.CreateAccessToken(user);

            return Ok(new AuthResponseDto
            {
                AccessToken = token,
                ExpiresAtUtc = expires
            });
        }


            [HttpPost("register")]
            [AllowAnonymous]
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

