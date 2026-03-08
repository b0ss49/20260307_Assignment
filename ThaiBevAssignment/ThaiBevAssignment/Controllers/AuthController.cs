using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ThaiBevAssignment.Models.Auth;
using ThaiBevAssignment.Models.Request;
using ThaiBevAssignment.Models.Response;
using ThaiBevAssignment.Services.Interfaces;

namespace ThaiBevAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest == null) return BadRequest(new { Message = "Login request cannot be null" });
                var loginResult = _authServices.Login(loginRequest);
                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = loginResult.Token,
                    RefreshToken = loginResult.RefreshToken
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken)) return BadRequest(new { Message = "Refresh token cannot be null or empty" });
                var newToken = _authServices.RefreshToken(refreshToken);
                return Ok(new RefreshTokenResponse
                {
                    RefreshToken = newToken
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                if (registerRequest == null) return BadRequest(new { Message = "Register request cannot be null" });
                _authServices.Register(registerRequest);
                return Ok(new { Message = "User registered successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }

        [Authorize]
        [HttpGet("welcome")]
        public IActionResult WelcomeMessage()
        {
            //take claims
            var claims = User.Claims;
            var username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            return Ok(new { Message = $"Welcome User : {username}" });
        }
    }
}
