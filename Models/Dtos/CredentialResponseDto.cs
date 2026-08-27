namespace UrlShortenerBackend.Models.Dtos
{
    public class CredentialResponseDto
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string ApiKey { get; set; } = string.Empty;

        public string SecretKey { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}