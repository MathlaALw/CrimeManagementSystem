using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using Crime_Management_System.Servises;
using Crime_Management_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Crime_Management_System.Attributes;
using Crime_Management_System.Data;
using Crime_Management_System.Services.Implementations;

namespace Crime_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        //private readonly JwtService _jwtService;
        private readonly CrimeDbContext _db;
        private readonly ITokenService _tokens;


        public AuthController(IUserService userService,CrimeDbContext crimeDbContext,ITokenService token)
        {
            _userService = userService;
            //_jwtService = jwtService;
            _db = crimeDbContext;
            _tokens = token;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userService.GetByUsernameOrEmailAsync(dto.UsernameOrEmail);
            if (user == null || !_userService.ValidatePassword(dto.Password, user.PasswordHash, user.Salt))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            //var token = _jwtService.GenerateToken(user);
            var (token, expires) = _tokens.CreateAccessToken(user);

            //return Ok(new { token });
            return Ok(new { accessToken = token, expiresAtUtc = expires, tokenType = "Bearer" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto request)
        {
            if (request == null)
                return BadRequest(new { message = "Request body is required." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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


    }



}

