using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UrlShortenerBackend.Constants;
using UrlShortenerBackend.Filters;
using UrlShortenerBackend.Interfaces;
using UrlShortenerBackend.Models;

namespace UrlShortenerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CredentialController : ControllerBase
    {
        private readonly ICredentialService _credentialService;

        public CredentialController(
            ICredentialService credentialService)
        {
            _credentialService = credentialService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var result =
                    await _credentialService.Create();

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

        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data =
                    await _credentialService.Get();

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

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _credentialService.Delete(id);

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
    }
}