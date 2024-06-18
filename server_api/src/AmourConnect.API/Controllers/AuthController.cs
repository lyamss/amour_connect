﻿using AmourConnect.API.Services;
using AmourConnect.App.Services;
using AmourConnect.Domain.Dtos.AppLayerDtos;
using AmourConnect.Domain.Dtos.SetDtos;
using AmourConnect.Infra.Interfaces;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
namespace AmourConnect.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }



        [HttpGet("login")]
        public IActionResult Login()
        {
            var props = new AuthenticationProperties { RedirectUri = "/api/Auth/signin-google" };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }



        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleLogin()
        {
            var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (response?.Principal == null) return BadRequest();

            var EmailGoogle = response.Principal.FindFirstValue(ClaimTypes.Email);
            var userIdGoogle = response.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(EmailGoogle) || string.IsNullOrEmpty(userIdGoogle))
            {
                return BadRequest();
            }

            int? Id_User = await _userRepository.GetUserIdWithGoogleIdAsync(EmailGoogle, userIdGoogle);

            if (Id_User > 0)
            {
                return await CreateSessionLoginAndReturnResponseAsync(Id_User.Value);
            }

            CookieUtils.CreateCookieToSaveIdGoogle(Response, userIdGoogle, EmailGoogle);
            return Redirect(Env.GetString("IP_NOW_FRONTEND") + "/register");
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] SetUserRegistrationDto setuserRegistrationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (userIdGoogle, emailGoogle) = CookieUtils.GetGoogleUserFromCookie(Request);

            if (string.IsNullOrEmpty(emailGoogle) || string.IsNullOrEmpty(userIdGoogle))
            {
                return BadRequest(new ApiResponseDto { message = "Please login with Google before register", succes = false });
            }

            IActionResult result = RegexUtils.CheckBodyAuthRegister(this, setuserRegistrationDto);

            if (result != null)
            {
                return result; // return IActionResult
            }

            if (await _userRepository.GetUserByPseudoAsync(setuserRegistrationDto.Pseudo))
            {
                return BadRequest(new ApiResponseDto { message = "Pseudo Already use", succes = false });
            }

            int? id_user = await _userRepository.GetUserIdWithGoogleIdAsync(emailGoogle, userIdGoogle);

            if (id_user > 0)
            {
                return await CreateSessionLoginAndReturnResponseAsync(id_user.Value);
            }

            int? id_user2 = await _userRepository.CreateUserAsync(userIdGoogle, emailGoogle, setuserRegistrationDto);

            if (id_user2.HasValue)
            {
                await CreateSessionLoginAndReturnResponseAsync(id_user2.Value);
                await EmailUtils.MailRegisterAsync(emailGoogle, setuserRegistrationDto.Pseudo);
                return Ok(new ApiResponseDto { message = "Register finish", succes = true });
            }

            return BadRequest(new ApiResponseDto { message = "Failed to create user", succes = false });
        }



        private async Task<IActionResult> CreateSessionLoginAndReturnResponseAsync(int Id_User)
        {
            SessionUserDto sessionData = await _userRepository.UpdateSessionUserAsync(Id_User);
            CookieUtils.CreateSessionCookie(Response, sessionData);
            return Redirect(Env.GetString("IP_NOW_FRONTEND") + "/welcome");
        }
    }
}