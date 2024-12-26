using ApplicationCore.Dtos.CustomerSupportChat;
using ApplicationCore.Interfaces;
using Infrastructure.Data.Linebot;
using Infrastructure.Services.Linebot.LineMessage;
using isRock.LineBot;

using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.ControllersApi
{
    public class ChatController : LineWebHookControllerBase
    {
        private readonly string _adminUserId;
        private readonly Bot _bot;
        private readonly MessageService _messageService;
        private readonly IRepository<User> _userRepository;

        public ChatController(LineMessagingApiSettings lineMessagingApiSettingsSettings,
            MessageService lineMessageService,
            IRepository<User> userRepository)
        {
            _messageService = lineMessageService;
            _adminUserId = lineMessagingApiSettingsSettings.UserId;
            ChannelAccessToken = lineMessagingApiSettingsSettings.ChannelAccessToken;
            _bot = new Bot(ChannelAccessToken);
            _userRepository = userRepository;
        }

        [Route("api/LineBotChatWebHook")]
        public async Task<IActionResult> GetChatResult()
        {
            try
            {
                if (IsLineVerify()) return Ok();
                foreach (var lineEvent in ReceivedMessage.events)
                {
                    var lineUserId = lineEvent.source.userId;
                    var user = GetUserInfo(lineUserId);
                    _bot.DisplayLoadingAnimation(lineEvent.source.userId, 20);
                    var responseMessage =
                        await _messageService.ProcessMessageAsync(user.userId, user.displayName,
                            lineEvent.message.text);
                    _bot.ReplyMessage(lineEvent.replyToken, responseMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _bot.PushMessage(_adminUserId, "系統忙碌中，請稍後再試。");
                return Ok();
            }

            return Ok();
        }

        private bool IsLineVerify()
        {
            return ReceivedMessage.events == null || ReceivedMessage.events.Count() <= 0 ||
                   ReceivedMessage.events.FirstOrDefault().replyToken == "00000000000000000000000000000000";
        }
        [HttpPost]
        [Route("api/LayoutChat")]
        public async Task<IActionResult> GetLayoutChatResult([FromBody] ChatMessage message)
        {
            var loginUserId = HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0";
            string NickName;
            string responseMessage;
            if (loginUserId != "0")
            {
                NickName = (await _userRepository.GetByIdAsync(Int32.Parse(loginUserId))).NickName;
            }else
            {
                NickName = "visitor";
            }
            try { 
            responseMessage = await _messageService.ProcessMessageAsync(loginUserId, NickName, message.Content.Trim());
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok(new { response= responseMessage });
        }
    }
}
