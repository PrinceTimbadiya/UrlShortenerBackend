namespace UrlShortenerBackend.Models.Dtos
{
    public class UserResponseDto
    {
        public long Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public bool? IsActive { get; set; }
    }
}