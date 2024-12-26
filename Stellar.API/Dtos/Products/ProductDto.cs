namespace Stellar.API.Dtos.Products
{
#nullable disable
    public class ProductDto
    {
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public DateTime ProductShelfTime { get; set; }
        public IFormFile MainImgFile { get; set; }
        public string Description { get; set; }
        public int PublisherID { get; set; }
        public string SystemRequirement { get; set; }
        public int CategoryId { get; set; }
        public byte ProductStatus { get; set; }
    }

    public class ProductsCarouselDto
    {
        public int CarouselId { get; set; }
        public int ProductID { get; set; }
        public IFormFile CarouselImages { get; set; }
        public string DataSourceUrl { get; set; }
        public int Sort { get; set; }
    }

    public class TagConnectDto
    {
        public int TagId { get; set; }
    }

    public class ProductPageEventDto
    {
        public int EventsId { get; set; }
        public string Title { get; set; }
        public string AnnounceText { get; set; }
        public string Content { get; set; }
        public IFormFile ProductImgFile { get; set; }
        public DateTime AnnouncementDate { get; set; }
    }

    public class ProductPageAboutDto
    {
        public string AboutMainTitle { get; set; }
    }

    public class AboutCardListDto
    {
        public IFormFile AboutCardImgFile { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }

}
