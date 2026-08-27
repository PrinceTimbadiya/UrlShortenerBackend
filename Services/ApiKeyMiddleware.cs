using System.Net;
using System.Text.Json;
using UrlShortenerBackend.Interfaces;
using UrlShortenerBackend.Models;

namespace UrlShortenerBackend.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IApiKeyService _apiKeyService;
        private readonly AppSettings _appSettings;

        public ApiKeyMiddleware(
            RequestDelegate next,
            IApiKeyService apiKeyService,
            Microsoft.Extensions.Options.IOptions<AppSettings>
                appSettings)
        {
            _next = next;
            _apiKeyService = apiKeyService;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(
            HttpContext context)
        {
            var requestPath =
                context.Request.Path;

            // ============================================
            // Allow short URL redirect
            // Example:
            // GET /w2H3dt
            // ============================================

            if (context.Request.Method == "GET" &&
                !requestPath.StartsWithSegments("/api") &&
                requestPath != "/")
            {
                await _next(context);
                return;
            }

            // ============================================
            // Bypass configured APIs
            // ============================================

            if (_appSettings.ApiBypass
                .Contains(requestPath.Value))
            {
                await _next(context);
                return;
            }

            // ============================================
            // Check API Key Header
            // ============================================

            if (!context.Request.Headers.TryGetValue(
                    "AK",
                    out var extractedApiKey))
            {
                await ReturnJsonResponse(
                    context,
                    HttpStatusCode.Unauthorized,
                    "API Key is missing.");

                return;
            }

            // ============================================
            // Check Empty API Key
            // ============================================

            if (string.IsNullOrWhiteSpace(
                    extractedApiKey))
            {
                await ReturnJsonResponse(
                    context,
                    HttpStatusCode.BadRequest,
                    "API Key cannot be empty.");

                return;
            }

            // ============================================
            // Validate API Key
            // ============================================

            var isValid =
                await _apiKeyService.ValidateApiKey(
                    extractedApiKey.ToString());

            if (!isValid)
            {
                await ReturnJsonResponse(
                    context,
                    HttpStatusCode.Forbidden,
                    "Invalid API Key.");

                return;
            }

            await _next(context);
        }

        private async Task ReturnJsonResponse(
            HttpContext context,
            HttpStatusCode statusCode,
            string message)
        {
            var response =
                new ResponseModel
                {
                    Status = false,
                    HttpStatus = statusCode,
                    Data = null,
                    Message = message
                };

            context.Response.StatusCode =
                (int)statusCode;

            context.Response.ContentType =
                "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}