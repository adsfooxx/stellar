namespace Stellar.API.Dtos.Products
{
    public class DiscountReadDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public DateOnly? DiscountStartDate { get; set; }
        public DateOnly DiscountEndDate { get; set; }
        public decimal DiscountValue { get; set; }
    }
}
