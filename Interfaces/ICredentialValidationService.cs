namespace UrlShortenerBackend.Interfaces
{
    public interface ICredentialValidationService
    {
        public Task<long> Validate(string apiKey, string secretKey);
    }
}