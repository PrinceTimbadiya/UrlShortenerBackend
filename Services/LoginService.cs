using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UrlShortenerBackend.Constants;
using UrlShortenerBackend.Data;
using UrlShortenerBackend.Filters;
using UrlShortenerBackend.Helpers;
using UrlShortenerBackend.Interfaces;
using UrlShortenerBackend.Models;
using UrlShortenerBackend.Models.Entities;

namespace UrlShortenerBackend.Services
{
    public class LoginService : ILoginService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILoginTokenService _loginTokenService;
        private readonly AppSettings _appSettings;
        private readonly LoggingService _loggingService;
        private readonly PasswordHasher<UserMaster> _passwordHasher;

        public LoginService(
            ApplicationDbContext dbContext,
            ILoginTokenService loginTokenService,
            IOptions<AppSettings> appSettings,
            LoggingService loggingService)
        {
            _dbContext = dbContext;
            _loginTokenService = loginTokenService;
            _appSettings = appSettings.Value;
            _loggingService = loggingService;
            _passwordHasher = new PasswordHasher<UserMaster>();
        }

        public async Task<LoginResponseModel> Login(LoginModel data)
        {
            await _loggingService.LogAsync(
                $"[START] Login attempt | Email: {data.Email}");

            try
            {
                if (string.IsNullOrWhiteSpace(data.Email) ||
                    string.IsNullOrWhiteSpace(data.Password))
                {
                    throw new CustomException(
                        "Email and password are required.");
                }

                InputValidationHelper.ValidateEmail(data.Email);

                var user = await _dbContext.UserMaster
                    .FirstOrDefaultAsync(x =>
                        x.Email == data.Email &&
                        x.IsActive == true);

                if (user == null)
                    throw new CustomException(
                        "Invalid email or password.");

                var passwordResult =
                    _passwordHasher.VerifyHashedPassword(
                        user,
                        user.PasswordHash ?? string.Empty,
                        data.Password);

                if (passwordResult == PasswordVerificationResult.Failed)
                    throw new CustomException(
                        "Invalid email or password.");

                var jwtToken =
                    await _loginTokenService.GenerateJwtToken(
                        user.Email ?? string.Empty);

                var refreshToken =
                    await _loginTokenService.GenerateRefreshTokenString();

                var tokenEntity = new LoginToken
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    JwtToken = jwtToken,
                    RefreshToken = refreshToken,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(_appSettings.Jwt.ExpiryMinutes),
                    IsRevoked = false
                };

                _dbContext.LoginToken.Add(tokenEntity);

                user.LastLoginAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                await _loggingService.LogAsync(
                    $"[SUCCESS] Login successful | UserId: {user.Id}");

                return new LoginResponseModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    JwtToken = jwtToken,
                    RefreshToken = refreshToken
                };
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Login failed | Email: {data.Email} | Error: {ex.Message}");

                throw;
            }
        }
    }
}