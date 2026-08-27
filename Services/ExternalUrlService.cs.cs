using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using UrlShortenerBackend.Data;
using UrlShortenerBackend.Filters;
using UrlShortenerBackend.Interfaces;
using UrlShortenerBackend.Models;
using UrlShortenerBackend.Models.Dtos;
using UrlShortenerBackend.Models.Entities;

namespace UrlShortenerBackend.Services
{
    public class ExternalUrlService : IExternalUrlService
    {
        private const int ShortCodeLength = 6;

        private const string ShortCodeCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly LoggingService _loggingService;
        private readonly AppSettings _appSettings;

        public ExternalUrlService(
            ApplicationDbContext dbContext,
            IMapper mapper,
            LoggingService loggingService,
            IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _loggingService = loggingService;
            _appSettings = appSettings.Value;
        }

        public async Task<UrlResponseDto> Create(
            UrlCreateDto dto)
        {
            await _loggingService.LogAsync(
                "[START] External Create Short URL");

            try
            {
                if (string.IsNullOrWhiteSpace(dto.LongUrl))
                {
                    throw new CustomException(
                        "Long URL is required.");
                }

                if (!Uri.TryCreate(
                        dto.LongUrl,
                        UriKind.Absolute,
                        out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp &&
                     uri.Scheme != Uri.UriSchemeHttps))
                {
                    throw new CustomException(
                        "Please provide a valid URL.");
                }

                var entity =
                    _mapper.Map<UrlMaster>(dto);

                // External API does not require login
                entity.UserId = null;

                entity.ShortCode =
                    await GenerateUniqueShortCode();

                entity.CreatedAt =
                    DateTime.UtcNow;

                entity.IsActive = true;

                _dbContext.UrlMaster.Add(entity);

                await _dbContext.SaveChangesAsync();

                var result =
                    _mapper.Map<UrlResponseDto>(entity);

                result.ShortUrl =
                    BuildShortUrl(
                        result.ShortCode);

                await _loggingService.LogAsync(
                    $"[SUCCESS] External Short URL Created | Id: {entity.Id} | ShortCode: {entity.ShortCode}");

                return result;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] External Create Short URL Failed: {ex.Message}");

                throw;
            }
        }

        private async Task<string> GenerateUniqueShortCode()
        {
            string shortCode;

            do
            {
                shortCode =
                    GenerateShortCode(
                        ShortCodeLength);
            }
            while (await _dbContext.UrlMaster
                .AnyAsync(x =>
                    x.ShortCode == shortCode));

            return shortCode;
        }

        private string GenerateShortCode(
            int length)
        {
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                var randomIndex =
                    RandomNumberGenerator.GetInt32(
                        ShortCodeCharacters.Length);

                result[i] =
                    ShortCodeCharacters[randomIndex];
            }

            return new string(result);
        }

        private string BuildShortUrl(
            string shortCode)
        {
            var baseUrl =
                _appSettings.ShortUrl.BaseUrl
                    .TrimEnd('/');

            return $"{baseUrl}/{shortCode}";
        }
    }
}