using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Model.WebApi;
using Microsoft.AspNetCore.Mvc;
using Stellar.API.Dtos.Supplier;
using Stellar.API.Service.Publisher;

namespace Stellar.API.Controllers
{
    [Route("api/[controller]/[action]")]
    public class PublisherController : ControllerBase
    {
        private readonly PublisherService _publisherService;

        public PublisherController(PublisherService publisherService)
        {
            _publisherService = publisherService;
        }
        [HttpGet]
        public  async Task<IActionResult> GetAllPublisher()
        {
            var publishers = await _publisherService.GetPublishers();

            //}).ToList();
            return Ok(new BaseApiResponse(publishers));
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePublisher([FromBody] PublishersDto publisher) 
        {

            await _publisherService.UpdatePublisher(publisher);

            return Ok();
        }
    }
}
