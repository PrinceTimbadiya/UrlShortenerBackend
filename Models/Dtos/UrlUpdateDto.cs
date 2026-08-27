namespace UrlShortenerBackend.Models.Dtos
{
    public class UrlUpdateDto
    {
        public long Id { get; set; }

        public string LongUrl { get; set; } = string.Empty;
    }
}