using UrlShortenerBackend.Models.Dtos;
using UrlShortenerBackend.Models.Entities;

namespace UrlShortenerBackend.Interfaces
{
    public interface ICredentialService
    {
        public Task<CredentialResponseDto> Create();

        public Task<CredentialResponseDto> Get();

        public Task<CredentialMaster> Delete(long id);
    }
}