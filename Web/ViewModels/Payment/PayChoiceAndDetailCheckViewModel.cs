namespace Web.ViewModels.Payment
{
    public class PayChoiceAndDetailCheckViewModel
    {
        public int UserId {  get; set; }
        public string Account {  get; set; }
        public List<PayChoiceAndDetailCheckShoppingCart> ShoppingCart { get; set; }
        public decimal Total {  get; set; }
        public string TotalToString {  get; set; }
        public string ProductNameListJson {  get; set; }
        public string ShoppingCartJson { get; set; }

    }
    public class PayChoiceAndDetailCheckShoppingCart
    {
        public int ProductId { get; set; }
        public string ProductImageUrl { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }

        public string PriceToString {  get; set; }

    }
}
