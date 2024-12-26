using ApplicationCore.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Services.Member;
using Web.ViewModels.Member;

namespace Web.test
{
    [ApiController]
    [Route("api/[controller]/[action]")]


    public class MemberApiController : ControllerBase
    {
        private readonly GameLibraryService _gameLibraryService;
        private readonly PurchaseHistoryService _purchaseHistoryServic;
        private readonly OrderDetailService _orderDetailService;

        public MemberApiController(GameLibraryService gameLibraryService, PurchaseHistoryService purchaseHistoryService, OrderDetailService orderDetailService)
        {
            _gameLibraryService = gameLibraryService;
            _purchaseHistoryServic = purchaseHistoryService;
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public async Task< ActionResult<GameLiberyViewModel>> GameLibray()
        {
            //帳戶詳細資料
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            try
            {
                var model = await _gameLibraryService.GetData(userId);
                if (model == null)
                {
                    return NotFound();  //404 Not Found
                }
                return Ok(model); // 200 OK 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpGet]
        public async Task<ActionResult<OrdersViewModel>> PurchaseHistory()
        {
            //帳戶詳細資料
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            try
            {
                var model = await _purchaseHistoryServic.GetData(userId);
                if (model == null)
                {
                    return NotFound();
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersViewModel>> OrderDetail( int id)
        {
        
            try
            {
                var model = await _orderDetailService.GetOrderDetailById(id);
                if (model == null)
                {
                    return NotFound();
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(500, "找不到");
            }
        }

    }
}
