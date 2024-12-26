namespace Web.ViewModels.Payment
{
    public class PayFinishedViewModel
    {
        public int UserId {  get; set; }    
        public string Account {  get; set; }
        public string Total { get; set; }

        public string OrderId {  get; set; }
    }
}
