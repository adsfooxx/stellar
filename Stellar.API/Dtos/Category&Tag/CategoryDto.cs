using PhotoAlbum.Data;

namespace Stellar.API.Dtos.Category_Tag
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? CategoryImgUrl { get; set; } 
        public IFormFile? PhotoToCloudinary { get; set; }

    }
}

