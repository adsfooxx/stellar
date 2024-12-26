namespace Stellar.API.Dtos.Products
{
#nullable disable
    public class CreateProductRequestDto
    {
        public ProductDto ProductDto { get; set; }
        public List<ProductsCarouselDto> CarouselDtos { get; set; }
        public List<int> TagIds { get; set; }
        public List<ProductPageEventDto> EventDtos { get; set; }
        public ProductPageAboutDto AboutDto { get; set; }
        public List<AboutCardListDto> AboutCardListDtos { get; set; }
    }
}
