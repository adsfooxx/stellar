using ApplicationCore.Dtos.Authentication;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Helpers;
using Web.Services.Authentication;
using Web.Services.ProductNs;
using Web.ViewModels.Authentication;
using isRock.LineLoginV21;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;

namespace Web.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly RegiterService _regiterService;
        private readonly LoginService _loginService;
        private readonly IRepository<User> _userRepository;
        private readonly ProductPageService _productPageService;
        private readonly ChangePasswordService _changePasswordService;
        public IConfigurationSection Channel_ID, Channel_Secret, CallbackURL;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(RegiterService regiterService, LoginService loginService, IRepository<User> userRepository, ProductPageService productPageService, ChangePasswordService changePasswordService, IConfiguration config, IConfiguration configuration, ILogger<AuthenticationController> logger)
        {
            _regiterService = regiterService;
            _loginService = loginService;
            _userRepository = userRepository;
            _productPageService = productPageService;
            _changePasswordService = changePasswordService;

            Channel_ID = config.GetSection("LINE-Login-Setting:Channel_ID");
            Channel_Secret = config.GetSection("LINE-Login-Setting:Channel_Secret");
            CallbackURL = config.GetSection("LINE-Login-Setting:CallbackURL");
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> GoogleLogin(string credential)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(credential);

                var user = await _loginService.FindOrCreateUser(payload);

                if (user.IsLocked == 0)
                {
                    return Json(new { success = false, message = "帳號已被停權，詳細情形請向客服聯繫。" });
                }

                var claims = new List<Claim>
                {
                     new Claim(ClaimTypes.Name, user.NickName),
                     new Claim(ClaimTypes.Email, user.EmailAddress),
                     new Claim(ClaimTypes.Sid, user.UserId.ToString()),
                     new Claim("UserImgUrl", user.UserImg ?? string.Empty)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //可為多重身分
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
               
                //驗證身分持續登入
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal);

                // 登錄後自動加入購物車或願望清單
                var productIdToAdd = HttpContext.Session.GetInt32("ProductIdToAdd");
                var returnUrl = HttpContext.Session.GetString("ReturnUrl");

                if (productIdToAdd.HasValue && !string.IsNullOrEmpty(returnUrl))
                {
                    HttpContext.Session.Remove("ProductIdToAdd");
                    HttpContext.Session.Remove("ReturnUrl");

                    await _productPageService.AddToCart(productIdToAdd.Value, user.UserId);

                    return Json(new { success = true, redirectUrl = returnUrl });
                }

                var wishListProductIdToAdd = HttpContext.Session.GetInt32("WishListProductIdToAdd");

                if (wishListProductIdToAdd.HasValue)
                {
                    HttpContext.Session.Remove("WishListProductIdToAdd");
                    HttpContext.Session.Remove("ReturnUrl");

                    await _productPageService.AddToWishList(wishListProductIdToAdd.Value, user.UserId);

                    return Json(new { success = true, redirectUrl = returnUrl });
                }
                // 成功登入，返回首頁的重定向 URL
                var redirectUrl = Url.Action("Index", "Home");
                return Json(new { success = true, redirectUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登入異常");
                return Json(new { success = false, message = "登入過程中發生錯誤，請稍後再試。" });
            }
        }

        [HttpGet]
        public IActionResult LineLogin()
        {


            var settings = new LineLoginSettingsViewModel
            {
                Channel_ID = Channel_ID.Value,
                Channel_Secret = Channel_Secret.Value,
                CallbackURL = CallbackURL.Value

            };


            return View(settings);


        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SSOCallback()
        {
            // 取得返回的 code
            var code = HttpContext.Request.Query["code"].ToString();
            if (string.IsNullOrEmpty(code))
            {
                HttpContext.Response.WriteAsync("沒有正確回應code").Wait();
                return Ok();
            }

            // 使用 code 獲取 token
            var token = Utility.GetTokenFromCode(
                code,
                Channel_ID.Value,
                Channel_Secret.Value,
                CallbackURL.Value);

            // 利用 access_token 獲取用戶資料
            var user = Utility.GetUserProfile(token.access_token);

            // 使用 id_token 取得 email claim
            var jwtToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(token.id_token);
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;


            var lineUser = new User();

            lineUser = _userRepository.ListAsync(u => u.Account == user.userId && u.State == 1).Result.FirstOrDefault();

            #region CreateUser
            if (lineUser is null)
            {
                lineUser = new User()
                {
                    NickName = user.displayName,
                    EmailAddress = email,
                    Password = Hash256.GetSHA256(Guid.NewGuid().ToString("N")),
                    Account = user.userId,
                    UserImg = user.pictureUrl,
                    WalletAmount = 0,
                    Country = "台灣",
                    City = "台北市",
                    Overview = "沒有輸入任何資料",
                    State = 1,
                    Online = 1,
                    IsLocked = 1,

                };
                await _userRepository.AddAsync(lineUser);
            }

            #endregion

            #region SignIn
            var claims = new List<Claim>
                {
                    //use email or LINE user ID as login name
                    new Claim(ClaimTypes.Name, string.IsNullOrEmpty( email ) ? user.userId:email ),
                    //use LINE displayName as FullName
                    new Claim("FullName",user.displayName),
                    new Claim(ClaimTypes.Role, "nobody"),
                    new Claim(ClaimTypes.Sid,lineUser.UserId.ToString())
                };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);  //在這裡顯示 IsAuthenticated

            #endregion
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            Console.WriteLine($"User IsAuthenticated: {isAuthenticated}"); // 輸出到 Console 日誌或 Logger

            // 導向至 LineLogin Action
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {//登入

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginData)
        {//登入




            var logState = await _loginService.UserLogin(loginData);

            if (logState != -1 && logState != -2)
            {

                var claims = new List<Claim> {
                   new Claim(ClaimTypes.Name,loginData.Account),
                   new Claim(ClaimTypes.Sid,logState.ToString())
                };

                var authProperties = new AuthenticationProperties
                {
                };

                // 建立 ClaimsIdentity.
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 建立 ClaimsPrincipal.
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // 網站登入.(寫入cookie, response 回傳後生效)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    authProperties);



                //登入後自動加入剛剛新增的購物車或願望清單
                var productIdToAdd = HttpContext.Session.GetInt32("ProductIdToAdd");
                var returnUrl = HttpContext.Session.GetString("ReturnUrl");

                if (productIdToAdd.HasValue && !string.IsNullOrEmpty(returnUrl))
                {
                    HttpContext.Session.Remove("ProductIdToAdd");
                    HttpContext.Session.Remove("ReturnUrl");

                    await _productPageService.AddToCart(productIdToAdd.Value, logState);

                    return Redirect(returnUrl);
                }

                var wishListProductIdToAdd = HttpContext.Session.GetInt32("WishListProductIdToAdd");

                if (wishListProductIdToAdd.HasValue)
                {
                    HttpContext.Session.Remove("WishListProductIdToAdd");
                    HttpContext.Session.Remove("ReturnUrl");

                    await _productPageService.AddToWishList(wishListProductIdToAdd.Value, logState);

                    return Redirect(returnUrl);
                }




                return RedirectToAction("Index", "Home");

            }
            else if (logState == -1)
            {
                ViewBag.loginErr = "帳號或密碼有錯誤";
                return View();
            }
            else
            {
                ViewBag.loginErr = "帳號已被停權，詳細情形請向客服聯繫。";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Authentication");
        }
        public IActionResult Regiter()
        {//註冊

            return View(new RegiterViewModel());
        }
        public IActionResult Verify()
        {//更換密碼
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string encodingParameter)
        {

            var isChecked = await _changePasswordService.VerifyEmail(encodingParameter);
            if (isChecked)
            {

                return RedirectToAction("ChangePassword", "Authentication");
            }
            else
            {
                ViewBag.Err = "驗證失敗請再次進行驗證";
                return View(nameof(Verify));
            }
        }

        [HttpPost("/Authentication/SendMail")]
        public async Task<IActionResult> SendMail()
        {//變更密碼驗證信
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");

            await _changePasswordService.SendVerifyMail(loginUserId);
            return Ok();
        }
        public IActionResult ChangePassword()
        {//更換密碼
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changeData)
        {//更換密碼
            var loginUserId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            var updateState = await _changePasswordService.ResetPassword(loginUserId, changeData);

            if (updateState == "變更成功")
            {
                Response.Cookies.Delete("verifyCode");
                return View("ChangePasswordSuccess");
            }
            else
            {
                ViewBag.ChangePasswordErr = updateState;
                return View();
            }

        }
        [HttpPost]
        public async Task<IActionResult> Regiter(RegiterViewModel reVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _regiterService.CreatViewModelToDataModel(reVM);

                    var userid = await _regiterService.Get_UserId(reVM);

                    await _regiterService.SendMail_CheckEmail(userid);


                    ViewData["textlog"] = "註冊信已寄出，請在四十分鐘內驗證";
                    return View("Login");
                }
                //return RedirectToAction("Login", "Authentication");
                return View(reVM);
            }
            catch (Exception ex)
            {
                reVM.text = ex.Message;

                return View(reVM);
            }
        }


        [HttpGet]
        public async Task<IActionResult> VerifyEmail_Check(string encodingParameter)
        {

            var isChecked = await _regiterService.VerifyEmail(encodingParameter);
            if (isChecked)
            {

                return RedirectToAction("Login", "Authentication");
            }
            else
            {
                ViewBag.Err = "驗證失敗請再次進行驗證";
                return View(nameof(Verify));
            }
        }


    }
}

