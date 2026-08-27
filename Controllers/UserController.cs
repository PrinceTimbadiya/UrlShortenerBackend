using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UrlShortenerBackend.Constants;
using UrlShortenerBackend.Interfaces;
using UrlShortenerBackend.Models;
using UrlShortenerBackend.Models.Dtos;

namespace UrlShortenerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Registration public
        [HttpPost("Create")]
        public async Task<IActionResult> Create(UserCreateDto data)
        {
            var result = await _userService.Create(data);

            return Ok(new ResponseModel
            {
                Status = true,
                HttpStatus = HttpStatusCode.OK,
                Message = ResponseMessages.SaveSuccess,
                Data = result
            });
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UserUpdateDto data)
        {
            var result = await _userService.Update(data);

            return Ok(new ResponseModel
            {
                Status = true,
                HttpStatus = HttpStatusCode.OK,
                Message = ResponseMessages.UpdateSuccess,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            var data = await _userService.Get();

            return Ok(new ResponseModel
            {
                Status = true,
                HttpStatus = HttpStatusCode.OK,
                Message = ResponseMessages.GetListSuccess,
                Data = data
            });
        }

        [Authorize]
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            var data = await _userService.GetById(id);

            return Ok(new ResponseModel
            {
                Status = true,
                HttpStatus = HttpStatusCode.OK,
                Message = ResponseMessages.GetSuccess,
                Data = data
            });
        }

        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            await _userService.Delete(id);

            return Ok(new ResponseModel
            {
                Status = true,
                HttpStatus = HttpStatusCode.OK,
                Message = ResponseMessages.DeleteSuccess
            });
        }
    }
}