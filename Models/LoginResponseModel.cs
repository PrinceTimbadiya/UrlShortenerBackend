namespace UrlShortenerBackend.Models
{
    public class LoginResponseModel
    {
        public string JwtToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        //public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long UserId { get; set; }
    }
}