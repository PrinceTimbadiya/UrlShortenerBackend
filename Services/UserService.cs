using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShortenerBackend.Constants;
using UrlShortenerBackend.Data;
using UrlShortenerBackend.Filters;
using UrlShortenerBackend.Helpers;
using UrlShortenerBackend.Interfaces;
using UrlShortenerBackend.Models.Dtos;
using UrlShortenerBackend.Models.Entities;

namespace UrlShortenerBackend.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly LoggingService _loggingService;
        private readonly PasswordHasher<UserMaster> _passwordHasher;

        public UserService(
            ApplicationDbContext dbContext,
            LoggingService loggingService)
        {
            _dbContext = dbContext;
            _loggingService = loggingService;
            _passwordHasher = new PasswordHasher<UserMaster>();
        }

        public async Task<long> Create(UserCreateDto dto)
        {
            await _loggingService.LogAsync("[START] Create User");

            try
            {
                InputValidationHelper.ValidateEmail(dto.Email);

                if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                {
                    throw new CustomException(
                        "User password must be minimum 6 characters.");
                }

                var usernameExists = await _dbContext.UserMaster
                    .AnyAsync(x =>
                        x.Username == dto.Username &&
                        x.IsActive == true);

                if (usernameExists)
                    throw new CustomException(
                        ResponseMessages.AlreadyExists);

                var emailExists = await _dbContext.UserMaster
                    .AnyAsync(x =>
                        x.Email == dto.Email &&
                        x.IsActive == true);

                if (emailExists)
                    throw new CustomException(
                        ResponseMessages.AlreadyExists);

                var entity = new UserMaster
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                entity.PasswordHash = _passwordHasher.HashPassword(
                    entity,
                    dto.Password!);

                _dbContext.UserMaster.Add(entity);

                await _dbContext.SaveChangesAsync();

                await _loggingService.LogAsync(
                    $"[SUCCESS] User Created : {entity.Id}");

                return entity.Id;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Create User Failed : {ex.Message}");

                throw;
            }
        }

        public async Task<long> Update(UserUpdateDto dto)
        {
            await _loggingService.LogAsync(
                $"[START] Update User : {dto.Id}");

            try
            {
                var entity = await _dbContext.UserMaster
                    .FirstOrDefaultAsync(x => x.Id == dto.Id);

                if (entity == null)
                    throw new CustomException("User not found");

                if (!string.IsNullOrWhiteSpace(dto.Username))
                {
                    var usernameExists = await _dbContext.UserMaster
                        .AnyAsync(x =>
                            x.Username == dto.Username &&
                            x.Id != dto.Id &&
                            x.IsActive == true);

                    if (usernameExists)
                        throw new CustomException(
                            ResponseMessages.AlreadyExists);

                    entity.Username = dto.Username;
                }

                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    InputValidationHelper.ValidateEmail(dto.Email);

                    var emailExists = await _dbContext.UserMaster
                        .AnyAsync(x =>
                            x.Email == dto.Email &&
                            x.Id != dto.Id &&
                            x.IsActive == true);

                    if (emailExists)
                        throw new CustomException(
                            ResponseMessages.AlreadyExists);

                    entity.Email = dto.Email;
                }

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    if (dto.Password.Length < 6)
                    {
                        throw new CustomException("User password must be minimum 6 characters.");
                    }

                    entity.PasswordHash = _passwordHasher.HashPassword(
                        entity,
                        dto.Password);
                }

                entity.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                await _loggingService.LogAsync(
                    $"[SUCCESS] User Updated : {entity.Id}");

                return entity.Id;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Update User Failed : {ex.Message}");

                throw;
            }
        }

        public async Task<List<UserResponseDto>> Get()
        {
            await _loggingService.LogAsync(
                "[START] Fetch Users");

            try
            {
                var data = await _dbContext.UserMaster
                    .Select(x => new UserResponseDto
                    {
                        Id = x.Id,
                        Username = x.Username,
                        Email = x.Email,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                        LastLoginAt = x.LastLoginAt,
                        IsActive = x.IsActive
                    })
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                await _loggingService.LogAsync(
                    $"[SUCCESS] {data.Count} Users Fetched");

                return data;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Fetch Users Failed : {ex.Message}");

                throw;
            }
        }

        public async Task<UserResponseDto> GetById(long id)
        {
            await _loggingService.LogAsync(
                $"[START] Fetch User By Id : {id}");

            try
            {
                var entity = await _dbContext.UserMaster
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new CustomException("User not found");

                var data = new UserResponseDto
                {
                    Id = entity.Id,
                    Username = entity.Username,
                    Email = entity.Email,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt,
                    LastLoginAt = entity.LastLoginAt,
                    IsActive = entity.IsActive
                };

                await _loggingService.LogAsync(
                    $"[SUCCESS] User Fetched : {id}");

                return data;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Fetch User Failed : {ex.Message}");

                throw;
            }
        }

        public async Task<UserMaster> Delete(long id)
        {
            await _loggingService.LogAsync(
                $"[START] Delete User : {id}");

            try
            {
                var entity = await _dbContext.UserMaster
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    throw new CustomException("User not found");

                _dbContext.UserMaster.Remove(entity);

                await _dbContext.SaveChangesAsync();

                await _loggingService.LogAsync(
                    $"[SUCCESS] User Deleted : {id}");

                return entity;
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Delete User Failed : {ex.Message}");

                throw;
            }
        }
    }
}