using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UrlShortenerBackend.Data;
using UrlShortenerBackend.Filters;
using UrlShortenerBackend.Interfaces;

namespace UrlShortenerBackend.Services
{
    public class CredentialValidationService : ICredentialValidationService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly LoggingService _loggingService;

        public CredentialValidationService(
            ApplicationDbContext dbContext,
            LoggingService loggingService)
        {
            _dbContext = dbContext;
            _loggingService = loggingService;
        }

        public async Task<long> Validate(
            string apiKey,
            string secretKey)
        {
            await _loggingService.LogAsync(
                "[START] Validate API Credentials");

            try
            {
                if (string.IsNullOrWhiteSpace(apiKey))
                    throw new CustomException(
                        "API key is required.");

                if (string.IsNullOrWhiteSpace(secretKey))
                    throw new CustomException(
                        "Secret key is required.");

                var credential =
                    await _dbContext.CredentialMaster
                        .FirstOrDefaultAsync(x =>
                            x.ApiKey == apiKey &&
                            x.IsActive == true);

                if (credential == null)
                    throw new CustomException(
                        "Invalid API credentials.");

                var secretKeyHash =
                    HashSecretKey(secretKey);

                var isValidSecretKey =
                    CryptographicOperations.FixedTimeEquals(
                        Encoding.UTF8.GetBytes(
                            credential.SecretKeyHash ?? string.Empty),
                        Encoding.UTF8.GetBytes(
                            secretKeyHash));

                if (!isValidSecretKey)
                    throw new CustomException(
                        "Invalid API credentials.");

                await _loggingService.LogAsync(
                    $"[SUCCESS] API Credentials Validated | UserId: {credential.UserId}");

                return credential.UserId;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] API Credential Validation Failed: {ex.Message}");

                throw;
            }
        }

        private static string HashSecretKey(
            string secretKey)
        {
            var bytes =
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(secretKey));

            return Convert.ToHexString(bytes);
        }
    }
}