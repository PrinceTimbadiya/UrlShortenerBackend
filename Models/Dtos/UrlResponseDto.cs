namespace UrlShortenerBackend.Models.Dtos
{
    public class UrlResponseDto
    {
        public long Id { get; set; }

        public string LongUrl { get; set; } = string.Empty;

        public string ShortCode { get; set; } = string.Empty;

        public string ShortUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}