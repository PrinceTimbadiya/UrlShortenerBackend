using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UrlShortenerBackend.Constants;
using UrlShortenerBackend.Filters;
using UrlShortenerBackend.Interfaces;
using UrlShortenerBackend.Models;
using UrlShortenerBackend.Models.Dtos;

namespace UrlShortenerBackend.Controllers
{
    [Route("api/external-url")]
    [ApiController]
    public class ExternalUrlController : ControllerBase
    {
        private readonly IExternalUrlService _externalUrlService;

        public ExternalUrlController(
            IExternalUrlService externalUrlService)
        {
            _externalUrlService = externalUrlService;
        }

        [AllowAnonymous]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(
            UrlCreateDto data)
        {
            try
            {
                var result =
                    await _externalUrlService.Create(data);

                return Ok(new ResponseModel
                {
                    Status = true,
                    HttpStatus = HttpStatusCode.OK,
                    Message = ResponseMessages.SaveSuccess,
                    Data = result
                });
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}