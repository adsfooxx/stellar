using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.LinePay.Dtos.Request;
using ApplicationCore.Model.WebApi;
using Infrastructure.Data.Mongo.Entity;
using Microsoft.AspNetCore.Mvc;

using Stellar.API.Dtos.Products;
using Stellar.API.Service.Product;
using Stellar.API.Service.Publisher;

namespace Stellar.API.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /*抓取產品用*/
        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            var products = await _productService.GetProducts();

            return Ok(products);
        }

        /*新增產品*/
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("數據不得為空");
                }

                await _productService.AddProduct(request);

                return Ok("產品上架成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "上架產品時發生錯誤");
                return StatusCode(500, $"錯誤原因: {ex.Message}");
            }
        }

        /*編輯產品主圖*/
        [HttpPost]
        public async Task<IActionResult> UpdateMainImg([FromForm] UpdateMainImgDto updateMainImgDto)
        {
            await _productService.UpdateMainImage(updateMainImgDto);
            return Ok();
        }

        /*編輯產品*/
        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromBody] EditProductDto product)
        {
            Console.WriteLine(product);
            await _productService.UpdateProduct(product);

            return Ok();
        }

        /*產品折扣區*/
        [HttpPost]
        public async Task<IActionResult> RemoveSelectedProductDiscounts([FromBody] DiscountDeleteDto deleteDiscountData)
        {
            Console.WriteLine(deleteDiscountData);
            await _productService.RemoveSelectedProductDiscount(deleteDiscountData);

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> GetSelectedProductDiscount([FromBody] List<int> productIds)
        {
            var selectedProductDiscount = await _productService.GetSelectedProductDiscount(productIds);

            return Ok(selectedProductDiscount);
        }
        [HttpPost]
        public async Task<IActionResult> DiscountProduct([FromBody] List<DiscountCreateDto> discountData)
        {
            if (discountData == null || !discountData.Any())
            {
                return BadRequest("No data received.");
            }
            await _productService.UpdateProductDiscount(discountData);
            Console.WriteLine(discountData);

            return Ok();
        }

        /*產品上下架停用區*/
        [HttpPost]
        public async Task<IActionResult> ActivateProducts([FromBody] List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
            {
                return BadRequest("No product IDs were provided.");
            }

            try
            {
                await _productService.ActivateProducts(productIds);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeactivateProducts([FromBody] List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
            {
                return BadRequest("No product IDs were provided.");
            }

            try
            {
                // 使用 service 方法進行批量下架
                await _productService.DeactivateProducts(productIds);
                return Ok();
            }
            catch (Exception ex)
            {
                // 你可以捕捉異常來進行錯誤處理
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /*上架產品抓取類別及標籤*/
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _productService.GetCategories();

            return Ok(categories);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            var tags = await _productService.GetTags();

            return Ok(tags);
        }

        /*產品公告區*/
        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement([FromForm] CreateEventDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("數據不得為空");
                }

                await _productService.AddProductEvent(request);

                return Ok("新增公告成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "上架產品時發生錯誤");
                return StatusCode(500, $"錯誤原因: {ex.Message}");
            }
        }
        /*[HttpPost]
        public async Task<IActionResult> ReadAnnouncements([FromBody] ProductAnnouncement request)
        {
            var productEvent = await _productService.GetProductEvents(request.ProductId);

            return Ok(productEvent);
        }*/
        [HttpPost]
        public async Task<IActionResult> UpdateAnnouncement([FromForm] CreateEventDto @event)
        {
            try
            {
                await _productService.EditProductEvent(@event);

                return Ok("更新成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("{eventid}")]
        public async Task<IActionResult> DeleteAnnouncement(int eventid)
        {
            await _productService.RemoveProductEvent(eventid);
            return Ok();
        }

        /*產品輪播圖區*/
        [HttpPost]
        public async Task<IActionResult> UpdateAndCreateCarousels([FromForm] List<ProductsCarouselDto> request)
        {
            await _productService.UpdateAndCreateCarousels(request);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> ReadAllCarousels([FromBody] ProductsCarouselDto request)
        {
            var productCarousel = await _productService.GetAllCarousels(request.ProductID);

            return Ok(productCarousel);
        }
        [HttpDelete("{carouselId}")]
        public async Task<IActionResult> DeleteCarousel(int carouselId)
        {
            await _productService.DeleteCarousel(carouselId);
            return Ok();
        }
    }
}
