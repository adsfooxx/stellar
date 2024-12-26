using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web.Models;
using Web.Services.Member;
using Web.Services.ProductNs;
using Web.ViewModels.Member;
using ApplicationCore.Dtos.AccountPageDto;
using Web.Services.Authentication;
using ApplicationCore.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Web.Extensions; // 使用擴展方法的命名空間



namespace Web.Controllers
{
    public class MemberController : Controller
    {
        private readonly MemberIndexServices _indexServices;
        private readonly EditDataService _editDataService;
        private readonly PurchaseHistoryService _purchaseHistoryService;
        private readonly OrderDetailService _orderDetailService;
        private readonly GameLibraryService _gameLibraryService;
        private readonly NotifyPageService _notifyPageService;
        private readonly TopUpService _topUpService;
        private readonly IRepository<User> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly WishListServices _wishListService;
        private readonly AccountPageService _accountPageService;
        private readonly AddPhoneService _addPhoneService;
        private readonly ChangeEmailService _changeEmailService;

        public MemberController(MemberIndexServices indexServices, EditDataService editDataService, PurchaseHistoryService purchaseHistoryService, OrderDetailService orderDetailService, NotifyPageService notifyPageService, TopUpService topUpService, IRepository<User> userRepository, WishListServices wishListService, AccountPageService accountPageService, GameLibraryService gameLibraryService, IHttpContextAccessor httpContextAccessor, AddPhoneService addPhoneService, ChangeEmailService changeEmailService)
        {
            _indexServices = indexServices;
            _editDataService = editDataService;
            _purchaseHistoryService = purchaseHistoryService;
            _orderDetailService = orderDetailService;
            _notifyPageService = notifyPageService;
            _topUpService = topUpService;
            _userRepository = userRepository;
            _wishListService = wishListService;
            _accountPageService = accountPageService;
            _gameLibraryService = gameLibraryService;
            _httpContextAccessor = httpContextAccessor;
            _addPhoneService = addPhoneService;
            _changeEmailService = changeEmailService;
        }
        // 複製用
        //var user = HttpContext.User.Identity?.Name;
        // var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
        [Authorize]
        public async Task<IActionResult> Index(int id)
        {//個人頁面
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            if (id == 0) { id = loginUserId; }


            var indexModel = await _indexServices.GetUserData(id, loginUserId);
            indexModel.commidContent = "";

            return View(indexModel);
        }


        [HttpPost]
        public async Task<IActionResult> Index(MemberIndexViewModel mivm)
        {//個人頁面 新增留言
         // 此為void方法但是因為void不能用await   所以那邊的方法是使用task 可以不用回傳直 又可以使用async
            if (ModelState.IsValid) { 
            await _indexServices.MemberIndexCraftCommid(mivm);
        }
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            var indexModel = await _indexServices.GetUserData(mivm.UserId, loginUserId);
            indexModel.commidContent = "";
            return View(indexModel);
        }



        [HttpPost]
        public async Task<IActionResult> AddFriend(MemberIndexViewModel mivm)
        {//個人頁面 新增好友
            await _indexServices.AddNewFriend(mivm);
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            var indexModel = await _indexServices.GetUserData(mivm.UserId, loginUserId);
            return View(nameof(Index), indexModel);
        }
        [HttpPost]
        public async Task<IActionResult> CheckFriend(MemberIndexViewModel mivm)
        {//個人頁面 新增好友
            await _indexServices.CheckFriend(mivm);
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            var indexModel = await _indexServices.GetUserData(mivm.UserId, loginUserId);
            return View(nameof(Index), indexModel);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCommmid(int id,int userId)
        {
            //刪除留言
            await _indexServices.DelCommid(id);
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            var indexModel = await _indexServices.GetUserData(userId, loginUserId);

            return View(nameof(Index), indexModel);
        }

        //通知頁面
        [Authorize]
        public async Task<IActionResult> NotifyPage()
        {
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            NotifyPageViewModel notifyModel = await _notifyPageService.GetNotifyPageServiceData(loginUserId);
            return View(notifyModel);
        }
        //通知頁面 確認好友邀請
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NotifyCheckFriend(NotifyPageViewModel NPVM)
        {
            //await _notifyPageService.CheckFriend(NPVM);
            //var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            //NotifyPageViewModel notifyModel = await _notifyPageService.GetNotifyPageServiceData(loginUserId);
            //return View(nameof(NotifyPage), notifyModel);

            await _notifyPageService.CheckFriend(NPVM);
            return Json(new { status = "success" });
        }
        //通知頁面 刪除通知(已是好友)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NotifyDelete(NotifyPageViewModel NPVM)
        {
            //await _notifyPageService.DeleteNotify(NPVM);
            //var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");          
            //NotifyPageViewModel notifyModel = await _notifyPageService.GetNotifyPageServiceData(loginUserId);
            //return View(nameof(NotifyPage), notifyModel);

            await _notifyPageService.DeleteNotify(NPVM);
            return Json(new { status = "success" });
        }
        //通知頁面 拒絕好友邀請
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NotifyRejectFriend(NotifyPageViewModel NPVM)
        {
         //await _notifyPageService.RejectFriend(NPVM);
         //var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
         //NotifyPageViewModel notifyModel = await _notifyPageService.GetNotifyPageServiceData(loginUserId);
         //return View(nameof(NotifyPage), notifyModel);

            // 處理拒絕好友邀請的邏輯
            await _notifyPageService.RejectFriend(NPVM);
            return Json(new { status = "success" }); // 返回 JSON 結果給前端
        }
        [Authorize]
        public async Task<IActionResult> EditData()
        {//編輯個人頁面
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            var editModel = await _editDataService.GetUserData(userId);
            if(editModel.UserId ==-1)
            {
                return RedirectToAction(nameof(Index), "Home");
            }
            editModel.Countries.Insert(0, new SelectListItem("===請選擇國家===", "",false,true));
            editModel.Cities.Insert(0, new SelectListItem("===縣市===", "", false, true));
            return View(editModel);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditData(EditDataViewModel user)
        {//編輯個人頁面

            if(user.NickName is null)
            {
                
                user.Countries.Insert(0, new SelectListItem("===請選擇國家===", "", false, true));
                user.Cities.Insert(0, new SelectListItem("===縣市===", "", false, true));
                return View(user);
            }

            await _editDataService.EditUserData(user);

            return RedirectToAction(nameof(Index), "Member", new { id = user.UserId });
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> WishList(string searchTerm)
        {
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");

            // 調用服務層，根據用戶ID和搜尋關鍵字獲取資料
            var wishListModel = await _wishListService.GetWishListData(userId, searchTerm);

            // 判斷是否為 AJAX 請求
            if (Request.IsAjaxRequest())
            {
                return PartialView("_WishListPartial", wishListModel); // 返回部分視圖
            }

            return View(wishListModel); // 如果不是 AJAX 請求，返回完整視圖
        }






        [Authorize]
        [HttpPost]
        public async Task <IActionResult> RemoveFromWishList(int wishListItemId)
        {
            _wishListService.RemoveItemFromWishList(wishListItemId);
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            WishListViewModel WishListModel = await _wishListService.GetWishListData(loginUserId);
            
            return RedirectToAction("WishList", wishListItemId);
        }
        //加入購物車功能
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productId)
        {            
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            await _wishListService.AddToCart(productId, loginUserId);

            // 導向其他view
            return RedirectToAction("ShoppingCart", "ShoppingCart");
            //return RedirectToAction("Index", "ShoppingCart");
        }
        //導向購物車       
        public async Task<IActionResult> GoToCart(int userId)
        {
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");


            // 導向其他view
            return RedirectToAction("ShoppingCart", "ShoppingCart", new { userId = loginUserId });
        }


        //加入收藏庫
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCollection(int productId)
        {
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            _wishListService.AddToCollection(productId, loginUserId);

            // 導向其他 view
            return RedirectToAction("WishList", new { id = productId });
        }
        [Authorize]
        public async Task<IActionResult> AccountPage()
        {//帳戶詳細資料

            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            AccountPageViewModel AccountPageModel = await _accountPageService.GetAccountPageData(loginUserId);
            return View(AccountPageModel);

        }

        [Authorize]
        public async Task<IActionResult> AddPhone()
        {//新增電話號碼頁面

            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            AddPhoneViewModel AddPhoneData = await _addPhoneService.GetAddPhoneData(loginUserId);
            return View(AddPhoneData);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewPhoneNumber(int id, string number)
        {//改電話
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            await _addPhoneService.UpdatePhoneNumber(id, number);
            //TempData["SuccessMessage"] = "電話號碼更新成功！";
            
            return RedirectToAction("AccountPage");

        }

        [Authorize]
        public async Task<IActionResult> ChangeEmail()
        {//新增電子郵件頁面
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            ChangeEmailViewModel GetChangeEmailData = await _changeEmailService.GetChangeEmailData(loginUserId);
            return View(GetChangeEmailData);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmail(int id, string address)
        {
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            var resultMessage = await _changeEmailService.UpdateEmailAddress(id, address);
            // 呼叫 ChangeEmailService 更新電子郵件，並取得結果訊息
            if (resultMessage == "Email address updated successfully.")
            {
                // 成功時返回 success: true 並帶上重定向網址
                await _changeEmailService.SendCheckEmail(loginUserId);
                return Json(new { success = true, redirectUrl = Url.Action("AccountPage", "Member") });
                
            }
            else
            {
                // 失敗時返回 success: false 並帶上錯誤訊息
                return Json(new { success = false, message = resultMessage });
            }
        }




        //============================================================================================================================

        // 針對 VerifyEmail 使用完整的屬性路由設置
        [HttpGet("/Member/VerifyEmail")]
        public async Task<IActionResult> VerifyEmail(string encodingParameter)
        {
            var isChecked = await _changeEmailService.VerifyCheckEmail(encodingParameter);

            // 無論驗證成功還是失敗，都重定向到 AccountPage
            if (isChecked)
            {
                TempData["VerificationSuccess"] = "電子郵件驗證成功！";
            }
            else
            {
                TempData["ErrorMessage"] = "驗證失敗，請再次進行驗證。";
            }

            return RedirectToAction("AccountPage", "Member");
        }

       



        [HttpPost("/Member/SendMail")]
        public async Task<IActionResult> SendMail()
        {//變更密碼驗證信
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");

            await _changeEmailService.SendCheckEmail(loginUserId);
            return Ok();
        }





        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(int userId)
        {//刪除帳號
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            // 確保 userId 不為零
            if (userId <= 0)
            {
                // 返回錯誤或顯示相應消息
                ModelState.AddModelError(string.Empty, "Invalid user ID.");
                return RedirectToAction("Error"); // 或返回其他適當的視圖
            }
            await _accountPageService.RemoveAccount(userId);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");

        }



        public IActionResult PurchaseHistory()
        {
            //帳戶詳細資料      
            return View();
        }
      
        [Authorize]
        public ActionResult GameLibray()
        {
            //遊戲庫 
            return View();
        }

        public async Task<IActionResult> TopUp()
        {
            var UserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            var model = await _topUpService.GetTopUpAmount(UserId);
            return View(model);
        }

    }

}
