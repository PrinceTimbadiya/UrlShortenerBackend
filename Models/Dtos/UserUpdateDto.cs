namespace UrlShortenerBackend.Models.Dtos
{
    public class UserUpdateDto
    {
        public long Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }
    }
}