namespace Web.ViewModels.Member
{
    public class NotifyPageViewModel
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public string UserName { get; set; }
        public int UnreadMessageCount { get; set; }
        public List<AddFriendNotify> AddFriendNotifyList { get; set; }

        // 用來傳遞選中的 notify.Id
        public int SelectedNotifyId { get; set; }
    }

    public class AddFriendNotify
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public string NotifyType { get; set; }
        public string NotifyDate { get; set; }
        public string FriendName { get; set; }
        public string NotifyText { get; set; }

        public byte?  ReadState { get; set; }
    }
}
