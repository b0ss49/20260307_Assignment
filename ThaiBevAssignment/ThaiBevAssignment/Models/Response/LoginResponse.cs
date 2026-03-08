namespace ThaiBevAssignment.Models.Response
{
    public class LoginResponse : BaseResponse
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
