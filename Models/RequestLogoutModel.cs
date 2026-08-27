namespace UrlShortenerBackend.Models
{
    public class RequestLogoutModel
    {
        public string Email { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}