using ThaiBevAssignment.Models.Dto;
using ThaiBevAssignment.Models.Request;

namespace ThaiBevAssignment.Services.Interfaces
{
    public interface IAuthServices
    {
        string GenerateToken(string username);
        string GenerateRefreshToken();
        void RevokeRefreshToken(string username);
        void Register(RegisterRequest registerRequest);
        LoginDto Login(LoginRequest loginRequest);
        string RefreshToken(string refreshToken);
    }
}
