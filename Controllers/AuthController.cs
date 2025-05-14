using Microsoft.AspNetCore.Mvc;
using SmartTaskAPI.Data;
using SmartTaskAPI.Models;
using SmartTaskAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;

namespace SmartTaskAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly RedisService _redisService;

        public AuthController(AppDbContext context, IConfiguration configuration, RedisService redisService)
        {
            _context = context;
            _configuration = configuration;
            _redisService = redisService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            var existingUser = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (existingUser) return BadRequest("Username already exists.");

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User successfully registered.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Invalid username or password.");

            var token = CreateToken(user);
            var refreshToken = GenerateRefreshToken();

            await _redisService.SetRefreshTokenAsync(user.Id, refreshToken, TimeSpan.FromDays(7));

            return Ok(new { token, refreshToken });
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Unauthorized");

            int userId = int.Parse(userIdClaim);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return Unauthorized("User not found");

            var storedToken = await _redisService.GetRefreshTokenAsync(userId);
            if (storedToken == null || storedToken != refreshToken)
                return Unauthorized("Invalid or expired refresh token");

            var newAccessToken = CreateToken(user);
            var newRefreshToken = GenerateRefreshToken();

            await _redisService.SetRefreshTokenAsync(user.Id, newRefreshToken, TimeSpan.FromDays(7));

            return Ok(new { token = newAccessToken, refreshToken = newRefreshToken });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserResponseDto>> GetMe()
        {
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role.ToString()
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            await _redisService.SetRefreshTokenAsync(userId, "", TimeSpan.Zero);

            return Ok(new { message = "Logged out successfully" });
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
