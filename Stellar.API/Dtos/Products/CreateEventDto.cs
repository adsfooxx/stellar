namespace Stellar.API.Dtos.Products
{
#nullable disable
    public class CreateEventDto
    {
        public int EventsId { get; set; }
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string AnnounceText { get; set; }
        public string Content { get; set; }
        public IFormFile EventImgUrl { get; set; }
        public DateOnly AnnouncementDate { get; set; }
    }
}
