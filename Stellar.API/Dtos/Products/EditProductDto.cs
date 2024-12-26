namespace Stellar.API.Dtos.Products
{
    public class EditProductDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductSalesPrice { get; set; }
        public string? ProductMainImageUrl { get; set; }
        public string ProductMainDescription { get; set; }
    }
}
