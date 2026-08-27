using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UrlShortenerBackend.Data;
using UrlShortenerBackend.Filters;
using UrlShortenerBackend.Interfaces;
using UrlShortenerBackend.Models.Dtos;
using UrlShortenerBackend.Models.Entities;

namespace UrlShortenerBackend.Services
{
    public class CredentialService : ICredentialService
    {
        private const int ApiKeyLength = 32;
        private const int SecretKeyLength = 64;

        private const string CredentialCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly LoggingService _loggingService;

        public CredentialService(
            ApplicationDbContext dbContext,
            IMapper mapper,
            IUserContextService userContextService,
            LoggingService loggingService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;
            _loggingService = loggingService;
        }

        public async Task<CredentialResponseDto> Create()
        {
            await _loggingService.LogAsync(
                "[START] Create API Credential");

            try
            {
                var currentUserId =
                    _userContextService.GetCurrentUserId();

                if (currentUserId <= 0)
                    throw new CustomException(
                        "Unauthorized user.");

                var existingCredential =
                    await _dbContext.CredentialMaster
                        .FirstOrDefaultAsync(x =>
                            x.UserId == currentUserId &&
                            x.IsActive == true);

                if (existingCredential != null)
                    throw new CustomException(
                        "Active API credential already exists.");

                var apiKey =
                    await GenerateUniqueApiKey();

                var secretKey =
                    GenerateSecretKey();

                var entity = new CredentialMaster
                {
                    UserId = currentUserId,
                    ApiKey = apiKey,
                    SecretKeyHash = HashSecretKey(secretKey),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _dbContext.CredentialMaster.Add(entity);

                await _dbContext.SaveChangesAsync();

                var result =
                    _mapper.Map<CredentialResponseDto>(entity);

                result.SecretKey = secretKey;

                await _loggingService.LogAsync(
                    $"[SUCCESS] API Credential Created | Id: {entity.Id} | UserId: {currentUserId}");

                return result;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Create API Credential Failed : {ex.Message}");

                throw;
            }
        }

        public async Task<CredentialResponseDto> Get()
        {
            await _loggingService.LogAsync(
                "[START] Fetch API Credential");

            try
            {
                var currentUserId =
                    _userContextService.GetCurrentUserId();

                if (currentUserId <= 0)
                    throw new CustomException(
                        "Unauthorized user.");

                var entity =
                    await _dbContext.CredentialMaster
                        .FirstOrDefaultAsync(x =>
                            x.UserId == currentUserId &&
                            x.IsActive == true);

                if (entity == null)
                    throw new CustomException(
                        "API credential not found.");

                var result =
                    _mapper.Map<CredentialResponseDto>(entity);

                // SecretKey is never returned after creation.
                result.SecretKey = string.Empty;

                await _loggingService.LogAsync(
                    $"[SUCCESS] API Credential Fetched | UserId: {currentUserId}");

                return result;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Fetch API Credential Failed : {ex.Message}");

                throw;
            }
        }

        public async Task<CredentialMaster> Delete(long id)
        {
            await _loggingService.LogAsync(
                $"[START] Delete API Credential : {id}");

            try
            {
                var currentUserId =
                    _userContextService.GetCurrentUserId();

                if (currentUserId <= 0)
                    throw new CustomException(
                        "Unauthorized user.");

                var entity =
                    await _dbContext.CredentialMaster
                        .FirstOrDefaultAsync(x =>
                            x.Id == id &&
                            x.UserId == currentUserId &&
                            x.IsActive == true);

                if (entity == null)
                    throw new CustomException(
                        "API credential not found.");

                entity.IsActive = false;
                entity.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                await _loggingService.LogAsync(
                    $"[SUCCESS] API Credential Deleted | Id: {id} | UserId: {currentUserId}");

                return entity;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Delete API Credential Failed : {ex.Message}");

                throw;
            }
        }

        private async Task<string> GenerateUniqueApiKey()
        {
            string apiKey;

            do
            {
                apiKey =
                    GenerateRandomCredential(
                        ApiKeyLength);
            }
            while (await _dbContext.CredentialMaster
                .AnyAsync(x =>
                    x.ApiKey == apiKey));

            return apiKey;
        }

        private string GenerateSecretKey()
        {
            return GenerateRandomCredential(
                SecretKeyLength);
        }

        private string GenerateRandomCredential(int length)
        {
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                var randomIndex =
                    RandomNumberGenerator.GetInt32(
                        CredentialCharacters.Length);

                result[i] =
                    CredentialCharacters[randomIndex];
            }

            return new string(result);
        }

        private string HashSecretKey(string secretKey)
        {
            var bytes =
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(secretKey));

            return Convert.ToHexString(bytes);
        }
    }
}