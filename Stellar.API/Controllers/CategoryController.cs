using ApplicationCore.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stellar.API.Dtos.Category_Tag;
using Stellar.API.Dtos.Products;
using Stellar.API.Service.Category_Tag;

namespace Stellar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
    
        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
      
        }

        [HttpGet("GetCategories")]
        public async Task<ActionResult<List<CategoryDto>>> GetCategorys()
        {
            var categories = await _categoryService.GetCategory();
            return Ok(categories); 

        }

        [HttpPost("CreateCategory")]
        public async Task<ActionResult> CreateCategory([FromForm] CategoryDto category)
        {
            if (category.CategoryName == null)
            {
                return BadRequest("Category is null."); // 返回 400 Bad Request
            }

            await _categoryService.CreateCategory(category);
            return CreatedAtAction(nameof(GetCategorys), new { id = category.CategoryId }, category); // 返回 201 Created
        }

        [HttpPut("UpdateCategory")]
        public async Task<ActionResult> UpdateCategory([FromForm] CategoryDto categoryDto)
        {
            if (categoryDto.CategoryName == null)
            {
                return BadRequest("CategoryDto is null."); // 返回 400 Bad Request
            }

            await _categoryService.UpdateCategory(categoryDto);
            return NoContent(); // 返回 204 No Content
        }
       
        [HttpDelete("DeleteCategory/{id}")]
        public async Task DeleteCategory(int id)
        {
            await _categoryService.DeleteCategory(id);
        }
    }
}
