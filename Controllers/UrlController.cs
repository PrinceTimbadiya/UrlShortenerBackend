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
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public UrlController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(
            UrlCreateDto data)
        {
            try
            {
                var result =
                    await _urlService.Create(data);

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

        [Authorize]
        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data =
                    await _urlService.Get();

                return Ok(new ResponseModel
                {
                    Status = true,
                    HttpStatus = HttpStatusCode.OK,
                    Message = ResponseMessages.GetListSuccess,
                    Data = data
                });
            }
            catch (CustomException)
            {
                throw;
            }
        }

        [Authorize]
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var data =
                    await _urlService.GetById(id);

                return Ok(new ResponseModel
                {
                    Status = true,
                    HttpStatus = HttpStatusCode.OK,
                    Message = ResponseMessages.GetSuccess,
                    Data = data
                });
            }
            catch (CustomException)
            {
                throw;
            }
        }

        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _urlService.Delete(id);

                return Ok(new ResponseModel
                {
                    Status = true,
                    HttpStatus = HttpStatusCode.OK,
                    Message = ResponseMessages.DeleteSuccess
                });
            }
            catch (CustomException)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToOriginalUrl(
            string shortCode)
        {
            try
            {
                var originalUrl =
                    await _urlService.GetOriginalUrl(
                        shortCode);

                return Redirect(originalUrl);
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}