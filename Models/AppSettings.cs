namespace UrlShortenerBackend.Models
{
    public class ConnectionStrings
    {
        public string SqlServer { get; set; } = string.Empty;
    }

    public class Jwt
    {
        public string Key { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public int ExpiryMinutes { get; set; }
    }

    public class DatabaseSettings
    {
        public string Provider { get; set; } = string.Empty;
    }

    public class ShortUrlSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
    }

    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new();

        public DatabaseSettings DatabaseSettings { get; set; } = new();

        public Jwt Jwt { get; set; } = new();

        public ShortUrlSettings ShortUrl { get; set; } = new();

        // APIs that do not require API Key
        public List<string> ApiBypass { get; set; } = new();

        // Main application API Key
        public string ApiKey { get; set; } = string.Empty;

        public string LogFilePath { get; set; } = string.Empty;
    }
}