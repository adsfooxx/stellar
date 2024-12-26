namespace Web.ViewModels.Member
{
    public class AccountPageViewModel
    {
        public string UserNickName { get; set; }
        public int UserID { get; set; }   
        public string Country { get; set; }
        public decimal WalletAmount {  get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public byte State { get; set; }
    }
}
