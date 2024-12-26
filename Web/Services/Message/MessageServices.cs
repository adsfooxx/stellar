using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Mono.TextTemplating;
using System.ComponentModel.Design;
using Web.ViewModels.Message;
using Web.ViewModels.Product;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Infrastructure.Data.Mongo.Entity;
using Infrastructure.Data.Mongo.Repository;


namespace Web.Services.Message
{
    public class MessageServices
    {
        private readonly IRepository<ApplicationCore.Entities.User> _userRepository;
        private readonly IRepository<Friend> _friendRepository;
        private readonly IRepository<MessageCard> _messageCardRepository;
        private readonly IRepository<Notify> _notifyRepository;

        //AI

        private readonly MongoRepository<ChatDto> _mongoRepository;



        public MessageServices(IRepository<ApplicationCore.Entities.User> userRepository, IRepository<Friend> friendRepository, IRepository<MessageCard> messageCardRepository, IRepository<Notify> notifyRepository, MongoRepository<ChatDto> mongoRepository)
        {
            _userRepository = userRepository;
            _friendRepository = friendRepository;
            _messageCardRepository = messageCardRepository;
            _notifyRepository = notifyRepository;
            _mongoRepository = mongoRepository;
        }

        //獲取資料
        public async Task<MessageViewModel> GetUserData(int loginUserId)
        {
            var loginUser = await _userRepository.FirstOrDefaultAsync(x => x.UserId == loginUserId);
            var friendList = (from friend in _friendRepository.List(x => (x.UserId == loginUserId || x.FriendUserId == loginUserId) && x.State == 1)
                              join user in _userRepository.List()
                              on friend.FriendUserId equals user.UserId
                              join user2 in _userRepository.List()
                              on friend.UserId equals user2.UserId
                              select new MessageIndexFriend
                              {
                                  FriendId = friend.UserId == loginUserId ? friend.FriendUserId : friend.UserId,
                                  FriendName = user.NickName == loginUser.NickName ? user2.NickName : user.NickName,
                                  FriendImgUrl = user.UserImg == loginUser.UserImg ? user2.UserImg! : user.UserImg!,
                                  FriendState = false,
                                  talkID = friend.FriendId
                              }).ToList();
            var commend = (from M in _messageCardRepository.List(x => x.SendToUserId == loginUserId)
                           join U in _userRepository.List()
                           on M.SendByUsetId equals U.UserId
                           select new MessageIndexCommid
                           {
                               CommidId = M.MessageId,
                               UserId = M.SendByUsetId,
                               CommUserName = U.NickName,
                               CommUserImgUrl = U.UserImg,
                               CommidText = M.CommitText,
                               User_State = U.Online == 1 ? true : false,
                               state = M.State,
                               CreateTime = M.CreateTime,
                           }).ToList();
            foreach (var man in friendList)
            {

                if (_userRepository.List(x => x.UserId == man.FriendId).First().Online == 1)
                {
                    man.FriendState = true;
                }


            }

            foreach (var item in commend)
            {
                int x = item.state - 1;
                if (x == 0)
                {
                    var i = _messageCardRepository.List(x => x.MessageId == item.CommidId).First();
                    _messageCardRepository.Delete(i);
                }
            }
            var userData = new MessageViewModel
            {
                Title = "這裡是群聊",
                UserId = loginUser.UserId,
                UserName = loginUser.NickName,
                UserImgUrl = loginUser.UserImg!,
                UserState = loginUser.Online == 1 ? true : false,
                FriendList = friendList,
                CommidList = commend,
            };
            return userData;
        }


        //新增留言
        public async Task AddCommend(int loginUserId, string text, int sendToUser)
        {
            var result = new MessageCard
            {
                SendToUserId = sendToUser,
                CommitText = text,
                CreateTime = DateTime.Now,
                SendByUsetId = loginUserId,
                State = 1,
            };
            await _messageCardRepository.AddAsync(result);
            var addNotify = new Notify
            {
                UserId = sendToUser,
                DateTime = DateTime.Now,
                Title = "聊天室,," + loginUserId,
                Text = "有來自" + _userRepository.FirstOrDefault(x => x.UserId == loginUserId).UserName + "的訊息",
                ImgUrl = _userRepository.FirstOrDefault(x => x.UserId == loginUserId).UserImg,
                ReadAlready = 0
            };
            await _notifyRepository.AddAsync(addNotify);

        }

        //改變登入狀態
        public async Task ChangeState(int loginUserId, int state)
        {

            var user = (await _userRepository.ListAsync(x => x.UserId == loginUserId)).First();
            switch (state)
            {
                case 0:
                    user.Online = 0;
                    break;

                case 1:
                    user.Online = 1;
                    break;
            }

            await _userRepository.UpdateAsync(user);


        }



      



        public async Task WriteHistoryAddToDb(string user, string chat)
        {
            ChatDto chatDto = new ChatDto()
            {
                User_Id = user,
                Chat = chat,
            };

            await _mongoRepository.CreateAsync(chatDto);
        }
        public async Task<List<ChatDto>> GetChatHistoryByUserIdAsync(string userId)
        {

            return await _mongoRepository.FindAsync("User_Id", userId);
        }


    }
}
