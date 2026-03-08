using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ThaiBevAssignment.AppContext;
using ThaiBevAssignment.Models;
using ThaiBevAssignment.Models.Dto;
using ThaiBevAssignment.Models.Request;
using ThaiBevAssignment.Services.Interfaces;

namespace ThaiBevAssignment.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthServices(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        public string GenerateToken(string username)
        {
            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            if (audience == null || key == null || issuer == null)
            {
                throw new InvalidOperationException("JWT configuration is missing");
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);
            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException($"JWT Key must be at least 32 bytes (256 bits) long for HS256. Current length: {keyBytes.Length} bytes");
            }

            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public LoginDto Login(LoginRequest loginRequest)
        {
            if (loginRequest?.UserName == null || loginRequest.Password == null)
                throw new ArgumentException("Invalid login request");

            var user = _db.Users.SingleOrDefault(u => u.UserName == loginRequest.UserName);
            if (user == null) throw new UnauthorizedAccessException("Invalid UserName");

            if (!PasswordHasher.Verify(loginRequest.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var token = GenerateToken(user.UserName);
            var refresh = GenerateRefreshToken();
            user.RefreshToken = refresh;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _db.SaveChanges();

            return new LoginDto { Token = token, RefreshToken = refresh };
        }

        public string RefreshToken(string refreshToken)
        {
            var user = _db.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new ArgumentException("Invalid refresh token");
            var newToken = GenerateToken(user.UserName);
            var newRefresh = GenerateRefreshToken();
            user.RefreshToken = newRefresh;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _db.SaveChanges();
            return newToken;
        }

        public void Register(RegisterRequest registerRequest)
        {
            if (registerRequest is null || string.IsNullOrWhiteSpace(registerRequest.UserName) || string.IsNullOrWhiteSpace(registerRequest.Password))
                throw new ArgumentException("Invalid register request");

            if (_db.Users.Any(u => u.UserName == registerRequest.UserName))
                throw new InvalidOperationException("User already exists");

            var user = new User
            {
                UserName = registerRequest.UserName,
                PasswordHash = PasswordHasher.HashPassword(registerRequest.Password)
            };

            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public void RevokeRefreshToken(string username)
        {
            var user = _db.Users.SingleOrDefault(u => u.UserName == username);
            if (user == null) return;
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            _db.SaveChanges();
        }

    }
}
