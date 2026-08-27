using Microsoft.AspNetCore.Mvc;
using UrlShortenerBackend.Filters;
using UrlShortenerBackend.Interfaces;

namespace UrlShortenerBackend.Controllers
{
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly IUrlService _urlService;
        private readonly LoggingService _loggingService;

        public RedirectController(
            IUrlService urlService,
            LoggingService loggingService)
        {
            _urlService = urlService;
            _loggingService = loggingService;
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToOriginalUrl(
            string shortCode)
        {
            await _loggingService.LogAsync(
                $"[START] Redirect Short URL | ShortCode: {shortCode}");

            try
            {
                var originalUrl =
                    await _urlService.GetOriginalUrl(
                        shortCode);

                await _loggingService.LogAsync(
                    $"[SUCCESS] Redirecting Short URL | ShortCode: {shortCode} | Url: {originalUrl}");

                return Redirect(
                    originalUrl);
            }
            catch (CustomException ex)
            {
                await _loggingService.LogErrorAsync(
                    $"[ERROR] Redirect Failed | ShortCode: {shortCode} | Error: {ex.Message}");

                throw;
            }
        }
    }
}