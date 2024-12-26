namespace Stellar.API.Dtos.Products
{
    public class DiscountDeleteDto
    {
        public int ProductId { get; set; }
        public DateOnly? DiscountStartDate { get; set; }
        public DateOnly DiscountEndDate { get; set; }
        public decimal DiscountValue { get; set; }
    }
}
