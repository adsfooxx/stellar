using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Web.MemoryCatch;
using Web.Models;
using Web.Services.Home;
using Web.ViewModels.Home;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly HomeService _homeService;
        private readonly IHomeCacheService _homoeCacheService;
        public HomeController(HomeService homeService, IHomeCacheService homoeCacheService)
        {
            //_logger = logger;
            _homeService = homeService;
            _homoeCacheService = homoeCacheService;
        }

        public async Task<IActionResult> Index()
        {
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "-1");
            var model = await GetData(loginUserId);
            
            
            return View(model);
        }
        public async Task<HomeVMBy7N> GetData(int userid)
        {
            var cacheKey = "your_data_key";

            var data = await _homoeCacheService.GetOrSetCacheAsync(cacheKey, async () =>
            {
                // 這裡放你的資料讀取邏輯，例如從資料庫讀取
                return await _homeService.GetHomeServiceData(userid);
            });

            return data;
        }

        private async Task<HomeVMBy7N> ReadDataFromDatabaseAsync(int userid)
        {
            // 你的資料庫查詢邏輯
        return    await _homeService.GetHomeServiceData(userid);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
