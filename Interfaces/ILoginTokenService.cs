namespace UrlShortenerBackend.Interfaces
{
    public interface ILoginTokenService
    {
        Task<string> GenerateJwtToken(string email);

        Task<string> GenerateRefreshTokenString();

        Task<bool> IsJwtTokenValid(string token);

        Task<bool> IsRefreshTokenValid(
            string email,
            string refreshToken);

        Task<string> RefreshJwtToken(
            string email,
            string refreshToken);

        Task RevokeToken(
            string email,
            string refreshToken);
    }
}