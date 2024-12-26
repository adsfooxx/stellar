using Microsoft.AspNetCore.Mvc;

namespace Stellar.API.Dtos.Products
{
    public class UpdateMainImgDto
    {
        public int productId { get; set; }

        [FromForm]
        public IFormFile mainImg {  get; set; }
    }
}
