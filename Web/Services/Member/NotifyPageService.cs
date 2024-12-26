using ApplicationCore.Interfaces;
using CloudinaryDotNet.Actions;
using System.Security.Claims;
using Web.ViewModels.Member;
using Web.ViewModels.Product;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace Web.Services.Member
{
    public class NotifyPageService
    {
        private readonly IRepository<Notify> _notifyRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Friend> _friendRepository;

        public NotifyPageService(IRepository<Notify> notifyRepository
            , IRepository<User> userRepository, IRepository<Friend> friendRepository)

        {
            _notifyRepository = notifyRepository;
            _userRepository = userRepository;
            _friendRepository = friendRepository;
        }
        public async Task<NotifyPageViewModel> GetNotifyPageServiceData(int loginUserId)
        {
            var notifyFromDb = await _notifyRepository.ListAsync(x => x.UserId == loginUserId); 
            var userFromDb = await _userRepository.ListAsync();

            var query = from notify in notifyFromDb                                                       
                        join user in userFromDb on notify.UserId equals user.UserId
                        where notify.Title.Contains("好友邀請")
                        orderby notify.DateTime descending 
                        select new AddFriendNotify
                        {
                            Id = ExtractUserIdFromTitle(notify.Title),//抓取邀請者ID
                            NotifyType = notify.Title,
                            NotifyText = notify.Text,
                            NotifyDate = notify.DateTime.ToString(),
                            ImgUrl = notify.ImgUrl,
                            //FriendName = user.UserName
                            ReadState = notify.ReadAlready,
                        };
          
            var result = query.ToList();
            //加入邀請者name
            foreach (var notify in result)
            {
                var friendUser = await _userRepository.FirstOrDefaultAsync(x => x.UserId == notify.Id);
                //UserName 一堆Null 所以先抓NickName
                notify.FriendName = friendUser != null ? friendUser.NickName : friendUser.UserId.ToString();             
            }
            //AddFriendImg
            foreach(var notify in result)
            {
                var friendUser = await _userRepository.FirstOrDefaultAsync(x => x.UserId == notify.Id);
                notify.ImgUrl= friendUser != null ? friendUser.UserImg :notify.ImgUrl;
            }


            //登入者資訊
            var loginuserFromDb = await _userRepository.FirstOrDefaultAsync(x=>x.UserId== loginUserId);
            var notifyPageData = new NotifyPageViewModel()
            {
                Id = loginUserId,
                ImgUrl = loginuserFromDb.UserImg,
                UserName = loginuserFromDb.UserName,
                UnreadMessageCount = 0,
                AddFriendNotifyList = result
            };
            //判斷未已讀條件 條件未改 待修正
            notifyPageData.UnreadMessageCount = notifyPageData.AddFriendNotifyList.Where(x=>x.ReadState==0).Count();

            return notifyPageData;
        }

        private int ExtractUserIdFromTitle(string title)
        {           
            if (string.IsNullOrEmpty(title)) return 0;

            // 查找 ",," 的位置
            int startIndex = title.IndexOf(",,");
            if (startIndex == -1) return 0; // 如果没找到 ",,"，返回 0            

            //string fromfriend=title.Split(",,")[0];
       
            // 提取 ",," 之后的部分
            string userIdPart = title.Substring(startIndex + 2);

                // 嘗試將提取的轉換成 int
                if (int.TryParse(userIdPart, out int userId))
                {
                    return userId; // 返回邀请者的ID
                }                       
            return 0;            
        }

        public async Task CheckFriend(NotifyPageViewModel NPVM)
        {      
            var friend_state = await _friendRepository
                .FirstOrDefaultAsync(x => (x.UserId == NPVM.Id || x.UserId == NPVM.SelectedNotifyId)
                && (x.FriendUserId == NPVM.Id || x.FriendUserId == NPVM.SelectedNotifyId) && x.State == 0);

            friend_state.State = 1;

            var my_notify_list = (await _notifyRepository.ListAsync(x => x.UserId == NPVM.Id));
            int change_id = -1;
            foreach (var item in my_notify_list)
            {
                string[] strings = item.Title.Split(",,");
                string type = strings[0];
                int send_user_id = int.Parse(strings[1]);
                if (send_user_id == NPVM.SelectedNotifyId && type == "好友邀請")
                {
                    change_id = item.NotifyId;
                }
            }
            if (change_id != -1)
            {
                var my_notify = my_notify_list.Where(x => x.NotifyId == change_id).First();

                my_notify.Text = "已成為朋友";
                my_notify.ReadAlready = 1;
                await _notifyRepository.UpdateAsync(my_notify);
            }
            await _friendRepository.UpdateAsync(friend_state);
        }
        public async Task DeleteNotify(NotifyPageViewModel NPVM)
        {                  
            var my_notify_list = (await _notifyRepository.ListAsync(x => x.UserId == NPVM.Id));            

            foreach (var item in my_notify_list)
            {
                string[] strings = item.Title.Split(",,");
                //string type = strings[0];
                int send_user_id = int.Parse(strings[1]);
                if (send_user_id == NPVM.SelectedNotifyId )
                {
                    var my_notify = my_notify_list.FirstOrDefault(x => x.NotifyId == item.NotifyId);
                    await _notifyRepository.DeleteAsync(my_notify);
                }
            }        
        }

        public async Task RejectFriend(NotifyPageViewModel NPVM)
        {
            var my_notify_list = (await _notifyRepository.ListAsync(x => x.UserId == NPVM.Id));
            foreach (var item in my_notify_list)
            {
                string[] strings = item.Title.Split(",,");
                string type = strings[0];
                int send_user_id = int.Parse(strings[1]);
                if (send_user_id == NPVM.SelectedNotifyId && type == "好友邀請")
                {
                    var my_notify = my_notify_list.FirstOrDefault(x => x.NotifyId == item.NotifyId);
                    await _notifyRepository.DeleteAsync(my_notify);
                }
            }
            var friend_state = await _friendRepository
               .FirstOrDefaultAsync(x => (x.UserId == NPVM.SelectedNotifyId)
               && (x.FriendUserId == NPVM.Id ) && x.State == 0);

            await _friendRepository.DeleteAsync(friend_state);
        }
    }
}
