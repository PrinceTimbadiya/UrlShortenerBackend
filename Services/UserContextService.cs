using System.Security.Claims;
using UrlShortenerBackend.Interfaces;

namespace UrlShortenerBackend.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated");
            }

            return long.Parse(userIdClaim.Value);
        }
    }
}