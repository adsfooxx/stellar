namespace Web.ViewModels.CustomerSupport
{
    public class ProductSupportViewModel
    {
        public int UserId {  get; set; }
        public string Account {  get; set; }
        public int ProductId {  get; set; }
        public string ProductName { get; set; }
        public string ProductImgUrl { get; set; }


        public string PurchasedDate { get; set; }
    }
}
