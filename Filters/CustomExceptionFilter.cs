using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using UrlShortenerBackend.Models;

namespace UrlShortenerBackend.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;
        private readonly LoggingService _loggingService;

        public CustomExceptionFilter(
            ILogger<CustomExceptionFilter> logger,
            LoggingService loggingService)
        {
            _logger = logger;
            _loggingService = loggingService;
        }

        public void OnException(ExceptionContext context)
        {
            string customMessage;

            if (context.Exception is CustomException customException)
            {
                customMessage = customException.CustomMessage;
            }
            else
            {
                customMessage =
                    "An unexpected error occurred. Please try again later or contact support.";
            }

            _logger.LogError(
                context.Exception,
                "Exception occurred: {Message}",
                customMessage);

            var ex = context.Exception;

            string errorMessage =
                $"================= EXCEPTION DETAILS ================={Environment.NewLine}" +
                $"Custom Message: {customMessage}{Environment.NewLine}" +
                $"Exception Type: {ex.GetType().FullName}{Environment.NewLine}" +
                $"Message: {ex.Message}{Environment.NewLine}" +
                $"Source: {ex.Source}{Environment.NewLine}" +
                $"TargetSite: {ex.TargetSite}{Environment.NewLine}" +
                $"HResult: {ex.HResult}{Environment.NewLine}" +
                $"StackTrace: {ex.StackTrace}{Environment.NewLine}";

            _loggingService.LogErrorAsync(errorMessage);

            var response = new ResponseModel
            {
                Status = false,
                HttpStatus = HttpStatusCode.BadRequest,
                Message = customMessage
            };

            context.Result = new JsonResult(response)
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };

            context.ExceptionHandled = true;
        }
    }
}