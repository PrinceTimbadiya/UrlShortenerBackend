using Microsoft.AspNetCore.Mvc;
using UrlShortenerBackend.Interfaces;

namespace UrlShortenerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiKeyController : ControllerBase
    {
        private readonly IApiKeyService _apiKeyService;

        public ApiKeyController(
            IApiKeyService apiKeyService)
        {
            _apiKeyService =
                apiKeyService;
        }

        [HttpGet("GenerateKey")]
        public async Task<IActionResult> GenerateApiKey()
        {
            var apiKey =
                await _apiKeyService.GenerateApiKey();

            return Ok(new
            {
                ak = apiKey
            });
        }
    }
}