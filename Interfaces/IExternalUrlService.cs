using UrlShortenerBackend.Models.Dtos;

namespace UrlShortenerBackend.Interfaces
{
    public interface IExternalUrlService
    {
        Task<UrlResponseDto> Create(
            UrlCreateDto dto);
    }
}