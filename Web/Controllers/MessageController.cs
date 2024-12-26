using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Web.Services.Message;

namespace Web.Controllers
{
    public class MessageController : Controller
    {
        private readonly MessageServices _messageServices;

        public MessageController(MessageServices messageServices)
        {
            _messageServices = messageServices;
        }

        public async Task<IActionResult> Index()
        {//聊天室

            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            if (loginUserId != 0) {
                var indexModel = await _messageServices.GetUserData(loginUserId);



                return View(indexModel);
            } else {

                return RedirectToAction("Login", "Authentication");
            }




        }


        [HttpPost]
        public async Task<IActionResult> AddCommend(string text, int sendToUser)
        {
            //新增離線留言至資料庫
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            if (loginUserId != 0)
            {
                await _messageServices.AddCommend(loginUserId, text, sendToUser);


                var indexModel = await _messageServices.GetUserData(loginUserId);
                return View(nameof(Index),indexModel);
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }



        [HttpPost]
        public async Task<IActionResult> changeState(int state)
        {
            //新增留言
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            if (loginUserId != 0)
            {
                await _messageServices.ChangeState(loginUserId, state);


                var indexModel = await _messageServices.GetUserData(loginUserId);
                return View(nameof(Index), indexModel);
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }

       

    }
}
