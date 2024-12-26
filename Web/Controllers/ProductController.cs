using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Services.Partial;
using Web.Services.ProductNs;
using Web.Services.Search;
using Web.ViewModels.Product;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductPageService _productPageService;
        private readonly ProductSearchServices _productrespondservices;
        private readonly SearchRequestServices _searchrequestservices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StoreNavbarService _storeNavbarService;



        public ProductController(ProductPageService productPageService, ProductSearchServices productrespondservices, SearchRequestServices searchrequestservices, IHttpContextAccessor httpContextAccessor, StoreNavbarService storeNavbarService)
        {
            _productPageService = productPageService;
            _productrespondservices = productrespondservices;
            _searchrequestservices = searchrequestservices;
            _httpContextAccessor = httpContextAccessor;
            _storeNavbarService = storeNavbarService;
        }
        public int GetUserId()
        {
            var userId = Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            return userId;
        }
        public IActionResult Index()
        {
            return View();
        }

        //取得評論
        [HttpGet]
        public async Task<IActionResult> UpdateComments(int productId)
        {
            var (commentsResult, commentsClass) = await _productPageService.GetComments(productId);

            return Json(new { commentsResult, commentsClass });
        }

        //增加差評跟刪除差評
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ModifyBadComment(int commentProductId, string actionType)
        {
            if (!User.Identity.IsAuthenticated)
            {
                //HttpContext.Session.SetInt32("WishListProductIdToAdd", commentProductId);
                //HttpContext.Session.SetString("ReturnUrl", Request.Headers["Referer"].ToString());

                return Json(new { success = false, redirectUrl = Url.Action("Login", "Authentication") });
            }

            if (actionType == "add")
            {
                await _productPageService.AddBadToComment(commentProductId, GetUserId());
                return Json(new { success = true, action = "add" });
            }

            else if (actionType == "remove")
            {
                await _productPageService.RemoveBadToComment(commentProductId, GetUserId());
                return Json(new { success = true, action = "remove" });
            }

            return Json(new { success = false });
        }

        //增加好評跟刪除好評
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ModifyGoodComment(int commentProductId, string actionType)
        {
            if (!User.Identity.IsAuthenticated)
            {
                //HttpContext.Session.SetInt32("WishListProductIdToAdd", commentProductId);
                //HttpContext.Session.SetString("ReturnUrl", Request.Headers["Referer"].ToString());

                return Json(new { success = false, redirectUrl = Url.Action("Login", "Authentication") });
            }

            if (actionType == "add")		 
            {
                await _productPageService.AddGoodToComment(commentProductId, GetUserId());
                return Json(new { success = true, action = "add" });
            }

            else if (actionType == "remove")
            {
                await _productPageService.RemoveGoodToComment(commentProductId, GetUserId());
                return Json(new { success = true, action = "remove" });
            }

            return Json(new { success = false });
        }

        //到時加入傳入的id參數
        public async Task<IActionResult> ProductPage(int id)
        {

            ProductPageViewModel productPageDataModel = await _productPageService.GetProductPageServiceData(id, GetUserId());

            if (productPageDataModel == null)
            {
                return NotFound();
                //return RedirectToAction("ErrorPage", "Errors", new { statuscode = 888 });
            }

            return View(productPageDataModel);
        }

        //動態更新storenavbar願望清單的數量
        [HttpGet]
        public IActionResult GetWishListCount()
        {
            int userId = GetUserId();
            var wishCount = _storeNavbarService.GetNavbar(userId).WishCount;
            return Json(new { wishCount });
        }

        //加入願望清單
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> ModifyWishList(int wishListProductId, string actionType)
        {
            if (!User.Identity.IsAuthenticated)
            {
                HttpContext.Session.SetInt32("WishListProductIdToAdd", wishListProductId);
                HttpContext.Session.SetString("ReturnUrl", Request.Headers["Referer"].ToString());

                return Json(new { success = false, redirectUrl = Url.Action("Login", "Authentication") });
            }

            if (actionType == "collection")
            {
                return Json(new { success = true, redirectUrl = Url.Action("GameLibray", "Member") });
            }

            if (actionType == "add")
            {
                await _productPageService.AddToWishList(wishListProductId, GetUserId());
                return Json(new { success = true, action = "add" });
            }

            else if (actionType == "remove")
            {
                await _productPageService.RemoveFromWishList(wishListProductId, GetUserId());
                return Json(new { success = true, action = "remove" });
            }

            return Json(new { success = false });
        }

        //加入購物車功能
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> AddToCart(int productId)
        {
            Console.WriteLine(productId);


            if (!User.Identity.IsAuthenticated)
            {
                HttpContext.Session.SetInt32("ProductIdToAdd", productId);
                //儲存當前頁面至Session
                HttpContext.Session.SetString("ReturnUrl", Request.Headers["Referer"].ToString());

                return RedirectToAction("Login", "Authentication");
            }

            await _productPageService.AddToCart(productId, GetUserId());

            // 導向其他view
            return RedirectToAction("ShoppingCart", "ShoppingCart");
        }

        //已擁有遊戲導向收藏庫
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCollection(int productId)
        {

            await _productPageService.AddToCollection(productId, GetUserId());

            // 導向其他 view
            return RedirectToAction("GameLibray", "Member", new { id = productId });
        }

        //已於購物車內直接導向至購物車
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DirectToShoppingCart(int productId)
        {

            await _productPageService.AddToCart(productId, GetUserId());

            // 導向其他 view
            return RedirectToAction("ShoppingCart", "ShoppingCart", new { userId = GetUserId() });
        }

        //已於收藏庫內直接導向至收藏庫
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DirectToCollection(int productId)
        {

            await _productPageService.AddToCollection(productId, GetUserId());

            // 導向其他 view
            return RedirectToAction("GameLibray", "Member");
        }



        // 這是現在的productSearch頁面

        public IActionResult ProductSearchMin(string query = null, string minPrice = null, string maxPrice = null, string categoryId = null, string tagId = null, string categoryName = null, string tagName = null, string typeBy = null)
        {
            return View();
        }


        public async Task<IActionResult> SearchRequest()
        {
            var model1 = await _searchrequestservices.GetSelectData();
            return View(model1);
        }
    }
}
