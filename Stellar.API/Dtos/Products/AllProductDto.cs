namespace Stellar.API.Dtos.Products
{
    public class AllProductDto
    {
        public int ProductId {  get; set; }
        public string? ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductSalesPrice { get; set; }
        public string? ProductMainImageUrl { get; set; }

        public string? ProductDiscount { get; set; }
        public DateOnly ProductShelfTime { get; set; }
        public bool ProductSellStatus { get; set; }
        public byte ProductStatus { get; set; }
        public double ProductGoodCommentPercentage { get; set; }
        public double ProductBadCommentPercentage { get; set; }
        public string ProductCategory { get; set; }
        public string ProductPublisher { get; set; }
        public string ProductMainDescription { get; set; }
        public List<ProductAnnouncement> ProductAnnouncements { get; set; }
    }
    public class ProductAnnouncement
    {
        public int ProductId { get; set; }
        public int EventsId { get; set; }
        public string Title { get; set; }
        public string AnnounceText { get; set; }
        public string Content { get; set; }
        public string ProductImgUrl { get; set; }
        public DateOnly AnnouncementDate { get; set; }
    }
}
