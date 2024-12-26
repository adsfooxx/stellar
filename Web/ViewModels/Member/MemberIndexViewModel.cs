using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Member
{
    public class MemberIndexViewModel
    {
        public int loginUser { get; set; }
        public string loginUserName { get; set; }
        public string loginUserImg { get; set; }
        public string Title { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<UserHistoryName> UserHistoryNameList { get; set; }
        public string UserImgUrl { get; set; }
        public string Userintro { get; set; }
        public List<MemberIndexGame> GameList { get; set; }
        public List<MemberIndexFriend> FriendList { get; set; }
        public List<MemberIndexCommid> CommidList { get; set; }
        public float AllGameTime { get; set; }
        public CraftCommidVM CraftCommidVM { get; set; }
        public int loginUserId { get; set; }
        public int commidSendUserId { get; set; }
        public int commidReciveUserId { get; set; }
        [Required(ErrorMessage = "請輸入留言")]
        public string commidContent { get; set; }
        public DateTime commidCreateTime { get; set; }
        public bool isFriend { get; set; }
        public bool isSendFriend { get; set; }
        public bool sendIsMe { get; set; }
    }
    public class UserHistoryName
    {
        public int Id { get; set; }
        public int UsetId { get; set; }
        public string Name { get; set; }
    }
    public class MemberIndexGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
    }
    public class MemberIndexFriend
    {
        public int FriendId { get; set; }
        public string FriendName { get; set; }
        public string FriendImgUrl { get; set; }
    }
    public class MemberIndexCommid
    {
        public int CommidId { get; set; }
        public string CommUserName { get; set; }
        public string CommUserImgUrl { get; set; }
        public string CommidText { get; set; }
        public DateTime CommidCreateTime { get; set; }
    }

    public class CraftCommidVM
    {
        public int commidSendUserId { get; set; }
        public int commidReciveUserId { get; set; }
        public string commidContent { get; set; }
        public DateTime commidCreateTime { get; set; }
    }
}
