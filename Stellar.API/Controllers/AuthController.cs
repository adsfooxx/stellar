using ApplicationCore.Model.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stellar.API.Dtos.JWT;
using Stellar.API.Service.JWT;

namespace Stellar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwt;


        public AuthController(JwtService jwt)
        {
            this._jwt = jwt;

        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDto request)
        {
            if (IsUserValid(request))
            {
                return Ok(new BaseApiResponse(_jwt.GenerateToken(request.UserName)));
            }

            return Ok(new BaseApiResponse()
            {
                IsSuccess = false,
                ErrMsg = "Invalid credentials" // 錯誤消息
            });
        }



        //[HttpGet]
        [HttpGet("UserName")] // 設置唯一路由
        [Authorize(Roles = "Admin")]
        public IActionResult GetUserName()
        {
            return Ok(new BaseApiResponse(User.Identity.Name));
        }

        //[HttpGet]
        [HttpGet("Claims")] // 設置唯一路由
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult GetClaims()
        {
            return Ok(new BaseApiResponse(User.Claims.Select(x => new { x.Type, x.Value })));

        }

        private bool IsUserValid(LoginDto request)
        {
            //TODO 轉寫實際的User判斷邏輯

            if (
                (request.UserName.Trim().ToLower() == "David".ToLower() && request.Password == "123".ToLower()) ||
                (request.UserName.Trim().ToLower() == "Demo".ToLower() && request.Password == "123".ToLower())
               ) 
            { return true; }
            else
            { return false; }
        }
    }
}




