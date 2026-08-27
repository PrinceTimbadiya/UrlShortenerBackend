using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UrlShortenerBackend.Interfaces;
using UrlShortenerBackend.Models;

namespace UrlShortenerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly ILoginTokenService _loginTokenService;

        public LoginController(
            ILoginService loginService,
            ILoginTokenService loginTokenService)
        {
            _loginService = loginService;
            _loginTokenService = loginTokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel data)
        {
            var res = await _loginService.Login(data);

            return StatusCode(
                (int)HttpStatusCode.OK,
                new ResponseModel
                {
                    Status = true,
                    HttpStatus = HttpStatusCode.OK,
                    Data = res,
                    Message = "Login successful."
                });
        }

        [HttpPost(nameof(RefreshToken))]
        public async Task<IActionResult> RefreshToken(
            RefreshTokenRequestModel data)
        {
            var jwtToken =
                await _loginTokenService.RefreshJwtToken(
                    data.Email,
                    data.RefreshToken);

            return Ok(new ResponseModel
            {
                Status = true,
                HttpStatus = HttpStatusCode.OK,
                Data = jwtToken,
                Message = "Token refreshed successfully."
            });
        }

        [Authorize]
        [HttpPost(nameof(Logout))]
        public async Task<IActionResult> Logout(
            RequestLogoutModel data)
        {
            await _loginTokenService.RevokeToken(
                data.Email,
                data.RefreshToken);

            return Ok(new ResponseModel
            {
                Status = true,
                HttpStatus = HttpStatusCode.OK,
                Message = "Logged out successfully."
            });
        }
    }
}