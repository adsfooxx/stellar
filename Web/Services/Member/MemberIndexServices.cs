using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using FluentEcpay;
using System.Security.Claims;
using Web.ViewModels.Member;

namespace Web.Services.Member
{
    public class MemberIndexServices
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<HistoryName> _historyNameRepository;
        private readonly IRepository<Friend> _friendRepository;
        private readonly IRepository<ProductCollection> _productCollectionRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Notify> _notifyRepository;

        public MemberIndexServices(IRepository<User> userRepository, IRepository<Comment> commentRepository, IRepository<HistoryName> historyNameRepository, IRepository<Friend> friendRepository, IRepository<ProductCollection> productCollectionRepository, IRepository<Product> productRepository, IRepository<Notify> notify)
        {
            _userRepository = userRepository;
            _commentRepository = commentRepository;
            _historyNameRepository = historyNameRepository;
            _friendRepository = friendRepository;
            _productCollectionRepository = productCollectionRepository;
            _productRepository = productRepository;
            _notifyRepository = notify;
        }
        public async Task<MemberIndexViewModel> GetUserData(int id, int loginUser)
        {
            int userId;
            if (_userRepository.Any(x => x.UserId == id))
            {
                userId = id;
            }
            else
            {
                userId = loginUser;
            }








            int loginUserId = loginUser;
            var loginUserImg = _userRepository.List(x => x.UserId == loginUser).FirstOrDefault()!.UserImg;
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserId == userId);

            var commentList = (from C in _commentRepository.List(x => x.ReciveUserId == userId)
                               join U in _userRepository.List()
                               on C.SendUserId equals U.UserId
                               select new MemberIndexCommid
                               {
                                   CommidId = C.CommentId,
                                   CommUserName = U.NickName,
                                   CommUserImgUrl = U.UserImg!,
                                   CommidText = C.Content,
                                   CommidCreateTime = C.CreateTime
                               }).OrderByDescending(c => c.CommidCreateTime).ToList();

            var historyNameList = (from His in _historyNameRepository.List(x => x.UserId == userId)
                                   orderby His.Time ascending
                                   select new UserHistoryName
                                   {
                                       Id = His.HistoryId,
                                       UsetId = His.UserId,
                                       Name = His.OldName,
                                   }).ToList();

            var friendList = (from f in _friendRepository.List(x => (x.UserId == userId || x.FriendUserId == userId) && x.State == 1)
                              join U in _userRepository.List()
                              on f.FriendUserId equals U.UserId
                              select new MemberIndexFriend
                              {
                                  FriendId = f.FriendId,
                                  FriendName = U.NickName,
                                  FriendImgUrl = U.UserImg!,
                              }).ToList();
            var collectionGame = (from U in _userRepository.List(x => x.UserId == userId)
                                  join Col in _productCollectionRepository.List(x => x.UserId == userId)
                                                        on U.UserId equals Col.UserId
                                  join P in _productRepository.List()
                                  on Col.ProductId equals P.ProductId
                                  select new MemberIndexGame
                                  {
                                      Id = P.ProductId,
                                      Name = P.ProductName,
                                      ImgUrl = P.ProductMainImageUrl,
                                  }).ToList();

            var loginUserNickName = await _userRepository.FirstOrDefaultAsync(x => x.UserId == loginUserId);

            bool isF = false;
            bool isSend = false;
            bool sendisme = false;
            var checkFriend = _friendRepository.List(x => (x.UserId == loginUserId || x.FriendUserId == loginUserId) && (x.UserId == userId || x.FriendUserId == userId));
            if (checkFriend.Count() > 0)
            {
                isSend = true;
                if (checkFriend.First().State == 1)
                {
                    isF = true;
                }
                if (isSend && checkFriend.First().UserId == loginUserId)
                {
                    sendisme = true;
                }

            }



            var userData = new MemberIndexViewModel
            {
                sendIsMe = sendisme,
                isSendFriend = isSend,
                isFriend = isF,
                loginUser = loginUser,
                loginUserId = loginUser,
                loginUserName = loginUserNickName.ToString()!,
                loginUserImg = loginUserImg,
                Title = "個人頁面",
                UserId = user.UserId,
                UserName = user.NickName,
                UserHistoryNameList = historyNameList,
                UserImgUrl = user.UserImg!,
                Userintro = user.Overview!,
                GameList = collectionGame,
                FriendList = friendList,
                CommidList = commentList
            };
            return userData;
        }
        public async Task AddNewFriend(MemberIndexViewModel MIVM)
        {
            Friend addF = new Friend
            {
                UserId = MIVM.loginUserId,
                FriendUserId = MIVM.UserId,
                State = 0
            };

            Notify addnotify = new Notify
            {
                UserId = MIVM.UserId,
                DateTime = DateTime.Now,
                Title = "好友邀請,," + MIVM.loginUserId,
                Text = "請跟我成為朋友!!",
                ImgUrl = (await _userRepository.ListAsync(x => x.UserId == MIVM.loginUserId)).First().UserImg,
                ReadAlready = 0,

            };

            await _friendRepository.AddAsync(addF);
            await _notifyRepository.AddAsync(addnotify);
        }
        public async Task CheckFriend(MemberIndexViewModel MIVM)
        {
            var friend_state = await _friendRepository
                .FirstOrDefaultAsync(x => (x.UserId == MIVM.UserId || x.UserId == MIVM.loginUserId)
                && (x.FriendUserId == MIVM.UserId || x.FriendUserId == MIVM.loginUserId) && x.State == 0);

            friend_state.State = 1;

            var my_notify_list = (await _notifyRepository.ListAsync(x => x.UserId == MIVM.loginUserId));
            int change_id = -1;
            foreach (var item in my_notify_list)
            {
                string[] strings = item.Title.Split(",,");
                string type = strings[0];
                int send_user_id = int.Parse(strings[1]);
                if (send_user_id == MIVM.UserId && type == "好友邀請")
                {
                    change_id = item.NotifyId;
                }
            }
            if (change_id != -1)
            {
                var my_notify = my_notify_list.Where(x => x.NotifyId == change_id).First();

                my_notify.Text = "已成為朋友";
                my_notify.ReadAlready = 0;
                await _notifyRepository.UpdateAsync(my_notify);
            }
            await _friendRepository.UpdateAsync(friend_state);
        }

        public async Task MemberIndexCraftCommid(MemberIndexViewModel MIVM)
        {
            Comment cRcommeent = new Comment
            {
                SendUserId = MIVM.loginUserId,
                ReciveUserId = MIVM.UserId,
                Content = MIVM.commidContent,
                CreateTime = DateTime.Now,
            };
            await _commentRepository.AddAsync(cRcommeent);
        }
        public async Task DelCommid(int id)
        {
            Comment delItem =
            _commentRepository.FirstOrDefault(x => x.CommentId == id);
            await _commentRepository.DeleteAsync(delItem);
        }
    }
}



