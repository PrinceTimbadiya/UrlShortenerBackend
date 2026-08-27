using UrlShortenerBackend.Models;

namespace UrlShortenerBackend.Interfaces
{
    public interface ILoginService
    {
        Task<LoginResponseModel> Login(LoginModel data);
    }
}