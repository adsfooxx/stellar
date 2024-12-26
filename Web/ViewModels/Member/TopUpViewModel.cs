namespace Web.ViewModels.Member
{
    public class TopUpViewModel
    {
        public int UserId { get; set; }
        public string Account { get; set; }
        public decimal Balance { get; set; }
        public List<Amount> Amount { get;set; }

    }
    public class Amount
    {
        public decimal Funds { get; set; }
    }
}
