namespace Web.ViewModels.CustomerSupport
{
    public class CustomerSupportViewModel
    {
        public int UserId {  get; set; }
        public string Account {  get; set; }

        public List<CustomerSupportGame> Games { get; set; }
    }

    //從玩家收藏庫的遊戲裡找
    public class CustomerSupportGame
    {
        public int ProductId {  get; set; }
        public string ProductName { get; set; }
        public string ProductImgUrl {  get; set; }

    }
}
