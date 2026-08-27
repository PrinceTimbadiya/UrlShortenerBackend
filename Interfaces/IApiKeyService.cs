namespace UrlShortenerBackend.Interfaces
{
    public interface IApiKeyService
    {
        Task<string> GetApiKey();

        Task<bool> ValidateApiKey(
            string providedApiKey);

        Task<string> GenerateApiKey();
    }
}