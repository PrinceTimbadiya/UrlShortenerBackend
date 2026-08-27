using Microsoft.Extensions.Options;
using UrlShortenerBackend.Filters;
using UrlShortenerBackend.Interfaces;
using UrlShortenerBackend.Models;

namespace UrlShortenerBackend.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly string _apiKey;
        private readonly LoggingService _loggingService;

        public ApiKeyService(
            IOptions<AppSettings> appSettings,
            LoggingService loggingService)
        {
            _apiKey =
                appSettings.Value.ApiKey;

            _loggingService =
                loggingService;
        }

        public async Task<string> GetApiKey()
        {
            await _loggingService.LogAsync(
                "[START] Get API Key");

            try
            {
                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    throw new CustomException(
                        "API key is not configured.");
                }

                await _loggingService.LogAsync(
                    "[SUCCESS] API Key Retrieved");

                return _apiKey;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Get API Key Failed : {ex.Message}");

                throw;
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Get API Key Failed : {ex.Message}");

                throw;
            }
        }

        public async Task<bool> ValidateApiKey(
            string providedApiKey)
        {
            await _loggingService.LogAsync(
                "[START] Validate API Key");

            try
            {
                if (string.IsNullOrWhiteSpace(
                        providedApiKey))
                {
                    await _loggingService.LogAsync(
                        "[FAILED] API Key Validation | API Key is empty");

                    return false;
                }

                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    throw new CustomException(
                        "API key is not configured.");
                }

                var isValid =
                    providedApiKey == _apiKey;

                await _loggingService.LogAsync(
                    $"[SUCCESS] API Key Validation : {isValid}");

                return isValid;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] API Key Validation Failed : {ex.Message}");

                throw;
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] API Key Validation Failed : {ex.Message}");

                throw;
            }
        }

        public async Task<string> GenerateApiKey()
        {
            await _loggingService.LogAsync(
                "[START] Generate API Key");

            try
            {
                var apiKey =
                    await GetApiKey();

                await _loggingService.LogAsync(
                    "[SUCCESS] API Key Generated");

                return apiKey;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] API Key Generation Failed : {ex.Message}");

                throw;
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] API Key Generation Failed : {ex.Message}");

                throw;
            }
        }
    }
}