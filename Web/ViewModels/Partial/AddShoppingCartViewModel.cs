namespace Web.ViewModels.Partial
{
    public class AddShoppingCartViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImgUrl { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductPriceFormat { get; set; }
        public decimal Discount { get; set; }
        public string DiscountFormat { get; set; }
        public decimal SalesPrice { get; set; }
        public string SalesPriceFormat { get; set; }
    }
}
