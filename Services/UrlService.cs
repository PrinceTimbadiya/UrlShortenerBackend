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
    public class UrlService : IUrlService
    {
        private const int ShortCodeLength = 6;

        private const string ShortCodeCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly LoggingService _loggingService;
        private readonly AppSettings _appSettings;

        public UrlService(
            ApplicationDbContext dbContext,
            IMapper mapper,
            IUserContextService userContextService,
            LoggingService loggingService,
            IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;
            _loggingService = loggingService;
            _appSettings = appSettings.Value;
        }

        public async Task<UrlResponseDto> Create(UrlCreateDto dto)
        {
            await _loggingService.LogAsync(
                "[START] Create Short URL");

            try
            {
                var currentUserId =
                    _userContextService.GetCurrentUserId();

                if (currentUserId <= 0)
                    throw new CustomException(
                        "Unauthorized user.");

                if (string.IsNullOrWhiteSpace(dto.LongUrl))
                    throw new CustomException(
                        "Long URL is required.");

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

                var entity = _mapper.Map<UrlMaster>(dto);

                entity.UserId = currentUserId;
                entity.ShortCode =
                    await GenerateUniqueShortCode();

                entity.CreatedAt = DateTime.UtcNow;
                entity.IsActive = true;

                _dbContext.UrlMaster.Add(entity);

                await _dbContext.SaveChangesAsync();

                var result =
                    _mapper.Map<UrlResponseDto>(entity);

                result.ShortUrl =
                    BuildShortUrl(result.ShortCode);

                await _loggingService.LogAsync(
                    $"[SUCCESS] Short URL Created | Id: {entity.Id} | UserId: {currentUserId} | ShortCode: {entity.ShortCode}");

                return result;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Create Short URL Failed : {ex.Message}");

                throw;
            }
        }

        public async Task<List<UrlResponseDto>> Get()
        {
            await _loggingService.LogAsync(
                "[START] Fetch Short URLs");

            try
            {
                var currentUserId =
                    _userContextService.GetCurrentUserId();

                if (currentUserId <= 0)
                    throw new CustomException(
                        "Unauthorized user.");

                var data = await _dbContext.UrlMaster
                    .Where(x =>
                        x.UserId == currentUserId &&
                        x.IsActive == true)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                var result =
                    _mapper.Map<List<UrlResponseDto>>(data);

                foreach (var item in result)
                {
                    item.ShortUrl =
                        BuildShortUrl(item.ShortCode);
                }

                await _loggingService.LogAsync(
                    $"[SUCCESS] {result.Count} Short URLs Fetched | UserId: {currentUserId}");

                return result;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Fetch Short URLs Failed : {ex.Message}");

                throw;
            }
        }

        public async Task<UrlResponseDto> GetById(long id)
        {
            await _loggingService.LogAsync(
                $"[START] Fetch Short URL By Id : {id}");

            try
            {
                var currentUserId =
                    _userContextService.GetCurrentUserId();

                if (currentUserId <= 0)
                    throw new CustomException(
                        "Unauthorized user.");

                var entity = await _dbContext.UrlMaster
                    .FirstOrDefaultAsync(x =>
                        x.Id == id &&
                        x.UserId == currentUserId);

                if (entity == null)
                    throw new CustomException(
                        "Short URL not found.");

                var result =
                    _mapper.Map<UrlResponseDto>(entity);

                result.ShortUrl =
                    BuildShortUrl(result.ShortCode);

                await _loggingService.LogAsync(
                    $"[SUCCESS] Short URL Fetched : {id}");

                return result;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Fetch Short URL Failed : {ex.Message}");

                throw;
            }
        }

        public async Task<UrlMaster> Delete(long id)
        {
            await _loggingService.LogAsync(
                $"[START] Delete Short URL : {id}");

            try
            {
                var currentUserId =
                    _userContextService.GetCurrentUserId();

                if (currentUserId <= 0)
                    throw new CustomException(
                        "Unauthorized user.");

                var entity = await _dbContext.UrlMaster
                    .FirstOrDefaultAsync(x =>
                        x.Id == id &&
                        x.UserId == currentUserId);

                if (entity == null)
                    throw new CustomException(
                        "Short URL not found.");

                entity.IsActive = false;

                await _dbContext.SaveChangesAsync();

                await _loggingService.LogAsync(
                    $"[SUCCESS] Short URL Deleted : {id}");

                return entity;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Delete Short URL Failed : {ex.Message}");

                throw;
            }
        }

        public async Task<string> GetOriginalUrl(string shortCode)
        {
            await _loggingService.LogAsync(
                $"[START] Resolve Short URL | ShortCode: {shortCode}");

            try
            {
                if (string.IsNullOrWhiteSpace(shortCode))
                    throw new CustomException(
                        "Short code is required.");

                var entity = await _dbContext.UrlMaster
                    .FirstOrDefaultAsync(x =>
                        x.ShortCode == shortCode &&
                        x.IsActive == true);

                if (entity == null)
                    throw new CustomException(
                        "Short URL not found.");

                await _loggingService.LogAsync(
                    $"[SUCCESS] Short URL Resolved | ShortCode: {shortCode}");

                return entity.LongUrl;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Resolve Short URL Failed : {ex.Message}");

                throw;
            }
        }

        private async Task<string> GenerateUniqueShortCode()
        {
            string shortCode;

            do
            {
                shortCode = GenerateShortCode(
                    ShortCodeLength);
            }
            while (await _dbContext.UrlMaster
                .AnyAsync(x =>
                    x.ShortCode == shortCode));

            return shortCode;
        }

        private string GenerateShortCode(int length)
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

        private string BuildShortUrl(string shortCode)
        {
            var baseUrl =
                _appSettings.ShortUrl.BaseUrl
                    .TrimEnd('/');

            return $"{baseUrl}/{shortCode}";
        }
    }
}