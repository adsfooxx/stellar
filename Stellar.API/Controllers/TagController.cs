using ApplicationCore.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stellar.API.Dtos.Category_Tag;
using Stellar.API.Service.Category_Tag;

namespace Stellar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet("GetTags")]
        public async Task<List<TagDto>> GetTags()
        {
            return await _tagService.GetTags();

        }

        [HttpPost("CreateTag")]
        public async Task CreateTag([FromBody] Tag tag)
        {
          await _tagService.CreateTag(tag);
        }
        [HttpPut("UpdateTag")]
        public async Task UpdateTag([FromBody] Tag tag)
        {
            await _tagService.UpdateTag(tag);
        }
        [HttpDelete("DeleteTag/{id}")]
        public async Task DeleteTag(int id)
        {
            await _tagService.DeleteTag(id);
        }
    }
}
