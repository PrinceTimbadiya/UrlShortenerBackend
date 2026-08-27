using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UrlShortenerBackend.Data;
using UrlShortenerBackend.Filters;
using UrlShortenerBackend.Helpers;
using UrlShortenerBackend.Interfaces;
using UrlShortenerBackend.Models;
using UrlShortenerBackend.Models.Entities;

namespace UrlShortenerBackend.Services
{
    public class LoginTokenService : ILoginTokenService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AppSettings _appSettings;
        private readonly LoggingService _loggingService;

        public LoginTokenService(
            ApplicationDbContext dbContext,
            IOptions<AppSettings> appSettings,
            LoggingService loggingService)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
            _loggingService = loggingService;
        }

        public async Task<string> GenerateJwtToken(string email)
        {
            await _loggingService.LogAsync(
                $"[START] GenerateJwtToken | Email: {email}");

            try
            {
                var user = await _dbContext.UserMaster
                    .FirstOrDefaultAsync(u =>
                        u.Email == email);

                if (user == null)
                    throw new CustomException(
                        "User not found or unauthorized.");

                var securityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _appSettings.Jwt.Key));

                var credentials = new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        user.Id.ToString()),

                    new Claim(
                        JwtRegisteredClaimNames.Sub,
                        email),

                    new Claim(
                        JwtRegisteredClaimNames.Jti,
                        Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: _appSettings.Jwt.Issuer,
                    audience: _appSettings.Jwt.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(
                        _appSettings.Jwt.ExpiryMinutes),
                    signingCredentials: credentials
                );

                await _loggingService.LogAsync(
                    $"[SUCCESS] JWT generated | UserId: {user.Id}");

                return new JwtSecurityTokenHandler()
                    .WriteToken(token);
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] GenerateJwtToken failed | Email: {email} | Error: {ex.Message}");

                throw;
            }
        }

        public async Task<string> GenerateRefreshTokenString()
        {
            await _loggingService.LogAsync(
                "[START] GenerateRefreshToken");

            var randomBytes = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var token = Convert.ToBase64String(randomBytes);

            await _loggingService.LogAsync(
                "[SUCCESS] Refresh token generated");

            return token;
        }

        public async Task<bool> IsJwtTokenValid(string token)
        {
            await _loggingService.LogAsync(
                "[START] Validate JWT token");

            try
            {
                var userToken = await _dbContext.LoginToken
                    .FirstOrDefaultAsync(t =>
                        t.JwtToken == token &&
                        t.IsRevoked == false &&
                        t.ExpiryDate > DateTime.UtcNow);

                var isValid = userToken != null;

                await _loggingService.LogAsync(
                    $"[RESULT] JWT validation result: {isValid}");

                return isValid;
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] JWT validation failed | Error: {ex.Message}");

                throw;
            }
        }

        public async Task<bool> IsRefreshTokenValid(
            string email,
            string refreshToken)
        {
            await _loggingService.LogAsync(
                $"[START] Validate refresh token | Email: {email}");

            InputValidationHelper.ValidateEmail(email);

            var token = await _dbContext.LoginToken
                .FirstOrDefaultAsync(t =>
                    t.Email == email &&
                    t.RefreshToken == refreshToken &&
                    t.IsRevoked == false &&
                    t.ExpiryDate > DateTime.UtcNow);

            var isValid = token != null;

            await _loggingService.LogAsync(
                $"[RESULT] Refresh token valid: {isValid}");

            return isValid;
        }

        public async Task<string> RefreshJwtToken(
            string email,
            string refreshToken)
        {
            await _loggingService.LogAsync(
                $"[START] Refresh JWT | Email: {email}");

            try
            {
                var isValid = await IsRefreshTokenValid(
                    email,
                    refreshToken);

                if (!isValid)
                    throw new CustomException(
                        "Invalid or expired refresh token.");

                var user = await _dbContext.UserMaster
                    .FirstOrDefaultAsync(u =>
                        u.Email == email);

                if (user == null)
                    throw new CustomException(
                        "User not found.");

                if (user.IsActive != true)
                    throw new CustomException(
                        "Your account is deactivated. Please contact support.");

                var jwtToken = await GenerateJwtToken(
                    user.Email ?? string.Empty);

                var tokenEntity = new LoginToken
                {
                    UserId = user.Id,

                    Email = email,

                    JwtToken = jwtToken,
                    RefreshToken = refreshToken,

                    ExpiryDate = DateTime.UtcNow.AddMinutes(
                        _appSettings.Jwt.ExpiryMinutes),

                    IsRevoked = false
                };

                _dbContext.LoginToken.Add(tokenEntity);

                await _dbContext.SaveChangesAsync();

                await _loggingService.LogAsync(
                    $"[SUCCESS] JWT refreshed | UserId: {user.Id}");

                return jwtToken;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] RefreshJwtToken failed | Email: {email} | Error: {ex.Message}");

                throw;
            }
        }

        public async Task RevokeToken(
            string email,
            string refreshToken)
        {
            await _loggingService.LogAsync(
                $"[START] Revoke tokens | Email: {email}");

            try
            {
                InputValidationHelper.ValidateEmail(email);

                var tokens = await _dbContext.LoginToken
                    .Where(t =>
                        t.Email == email &&
                        t.RefreshToken == refreshToken &&
                        t.IsRevoked == false)
                    .ToListAsync();

                foreach (var item in tokens)
                {
                    item.IsRevoked = true;
                }

                await _dbContext.SaveChangesAsync();

                await _loggingService.LogAsync(
                    $"[SUCCESS] Tokens revoked | Email: {email}");
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] RevokeToken failed | Email: {email} | Error: {ex.Message}");

                throw;
            }
        }
    }
}