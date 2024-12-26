
namespace Web.ViewModels.ShoppingCart
{
    public class ShoppingCartViewModel
    {
        public int ShoppingCartId { get; set; }
        public List<ShoppingCartProduct> ShoppingCartProducts { get; set; }
        public List<ShoppingCartProduct> RecommendWithinHalfYear { get; set; }
        public List<ShoppingCartProduct> RecommendProducts2 { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class ShoppingCartProduct
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
        public byte ProductStatus { get; set; }
    }
}
