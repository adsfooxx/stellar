using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Model.WebApi;
using Microsoft.AspNetCore.Mvc;
using Stellar.API.Dtos.Charts;
using Stellar.API.Service.Product;
using Stellar.API.Service.Charts;

namespace Stellar.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {

        private readonly ChartsService _chartsRepository;

        public ChartsController(ChartsService chartsRepository)
        {
            _chartsRepository = chartsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetChartsData()
        {
            var chart = await _chartsRepository.GetCharts();
            return Ok(chart);
            
        }





    }
}
