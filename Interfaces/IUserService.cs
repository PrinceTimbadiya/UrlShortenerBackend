using UrlShortenerBackend.Models.Dtos;
using UrlShortenerBackend.Models.Entities;

namespace UrlShortenerBackend.Interfaces
{
    public interface IUserService
    {
        public Task<long> Create(UserCreateDto dto);
        public Task<long> Update(UserUpdateDto dto);
        public Task<List<UserResponseDto>> Get();
        public Task<UserResponseDto> GetById(long id);
        public Task<UserMaster> Delete(long id);
    }
}