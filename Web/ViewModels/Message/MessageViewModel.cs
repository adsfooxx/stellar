namespace Web.ViewModels.Message
{
    public class MessageViewModel
    {
        public string Title {  get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserImgUrl { get; set; }
        public bool UserState {  get; set; }
        public List<MessageIndexFriend> FriendList { get; set; }
        public List<MessageIndexCommid> CommidList { get; set; }
    }


    public class MessageIndexFriend
    {
        public int FriendId { get; set; }
        public string FriendName { get; set; }
        public string FriendImgUrl { get; set; }
        public bool FriendState {  get; set; }
        public int talkID { get; set; }

    }

    public class MessageIndexCommid
    {
        public int CommidId { get; set; }
        public int UserId { get; set; }
        public string CommUserName { get; set; }
        public string CommUserImgUrl { get; set; }
        public bool User_State { get; set; }
        public string CommidText { get; set; }
        public int state { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
