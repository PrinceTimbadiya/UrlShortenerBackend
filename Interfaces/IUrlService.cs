using UrlShortenerBackend.Models.Dtos;
using UrlShortenerBackend.Models.Entities;

namespace UrlShortenerBackend.Interfaces
{
    public interface IUrlService
    {
        public Task<UrlResponseDto> Create(UrlCreateDto dto);

        public Task<List<UrlResponseDto>> Get();

        public Task<UrlResponseDto> GetById(long id);

        public Task<UrlMaster> Delete(long id);

        public Task<string> GetOriginalUrl(string shortCode);
    }
}