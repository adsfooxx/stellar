using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Model.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stellar.API.Dtos.User;

namespace Stellar.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;

        public UserController(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> getAllUser()
        {
            var user = (await _userRepository.ListAsync()).Select(u =>
            new AllUserDto
            {
                UserId = u.UserId,
                Account = u.Account,
                Country = u.Country,
                Email = u.EmailAddress,
                IsLock = u.IsLocked,
                NickName = u.NickName,
                Wallet = u.WalletAmount
            }
            ).ToList();


            return Ok(new BaseApiResponse(user));
        }

        // 修改用户信息的 API
        //ApiRequest.httpPut(`api/user/updateUser/${userId  }`, userData);
        [HttpPut("{userid}/{islock}")]
        [HttpPut]
        public async Task<IActionResult> updateUser(int userid, byte islock)
        {
            var user = await _userRepository.GetByIdAsync(userid);

            if (user == null)
            {
                return NotFound(new BaseApiResponse(user));
            }

            // 更新用户信息

            user.IsLocked = islock;

            await _userRepository.UpdateAsync(user);

            return Ok(new BaseApiResponse(user));
        }


        [HttpPost("{userid}/{nickName}/{email}/{country}")]
        // 新增用户的 API
        [HttpPost]
        public async Task<IActionResult> editUser(int userid, string nickName, string email, string country)
        {

            var user = await _userRepository.GetByIdAsync(userid);
            // 创建新的用户实例


            user.NickName = nickName;
            user.EmailAddress = email;
            user.Country = country;

            await _userRepository.UpdateAsync(user);

            return Ok(new BaseApiResponse(user));
        }


    }
}
