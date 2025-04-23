using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SpotParkAPI.Models;
using Microsoft.AspNetCore.Mvc;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Services.Interfaces;
using SpotParkAPI.Models.Dtos;


namespace SpotParkAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly SpotParkDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(SpotParkDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user= await _context.Users.FirstOrDefaultAsync(u => u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username/email or password");
            }

            var token = GenerateJwtToken(user);

            var userDto = new UserDto
            {
                Username = user.Username,
                Email = user.Email,
            };

            return new LoginResponse
            {
                Token = token,
                User = userDto

            };
            
        }



        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.Now
            };
                _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;

        }
            



        private string GenerateJwtToken(User user)
        {
            Console.WriteLine($"Generating token for UserId: {user.UserId}");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
               new Claim(ClaimTypes.Name,user.Username),
            };
            var token = new JwtSecurityToken(
               issuer: _configuration["Jwt:Issuer"],
               audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
