using ApplicationCore.LinePay.Dtos.Confirm;
using ApplicationCore.LinePay;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ApplicationCore.Interfaces;
using Web.Services.ShoppingCart;
using System.Security.Claims;
using Microsoft.CodeAnalysis;
using Web.Services.LinePay;
using MimeKit.Cryptography;



namespace Web.Controllers
{
    public class LinePayController : Controller
    {
        private readonly ILogger<LinePayController> _logger;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<User> _userRepository;
        private readonly LinePayService _linePayService;
        private readonly ShoppingCartService _shoppingCartService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;
        private readonly IConfiguration _configuration;

        public LinePayController(

              ILogger<LinePayController> logger,
               LinePayService linePayService,
              ShoppingCartService shoppingCartService,
              IUnitOfWork unitOfWork,
              IRepository<LinePay> linePayRepository,
              IConfiguration configuration,
              IRepository<Product> productRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _orderRepository = unitOfWork.GetRepository<Order>();
            _linePayService = linePayService;
            _userRepository = unitOfWork.GetRepository<User>();
            _shoppingCartService = shoppingCartService;
            _configuration = configuration;
            _productRepository = productRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Payment()
        {
            try
            {
                await _unitOfWork.BeginAsync();
                // 取得user id
                var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");

                _logger.LogInformation("User {UserId} is attempting to process payment.", userId);

                var linePayApi = new LinePayApi(_linePayService.GetOptions(), _configuration);

                var products = (await _shoppingCartService.GetShoppingCartData(userId)).ShoppingCartProducts;

                // 建立 PaymentRequest 物件
                var paymentRequest = await _linePayService.CreatePayment(userId, products);

                // 發送付款請求
                var response = await linePayApi.RequestAsync(paymentRequest);


                // 將使用者導向 LINE Pay 付款頁面
                if (response.Info == null)
                {
                    return BadRequest("LINE Pay API 回應的 Info 為 null");
                }

                // 檢查 PaymentUrl 是否為 null 或空
                if (string.IsNullOrEmpty(response.Info.PaymentUrl?.Web))
                {
                    return BadRequest("LINE Pay API 回應的 PaymentUrl 為 null 或空");
                }

                var orderId = paymentRequest.OrderId;

                var orderIdJson = JsonConvert.SerializeObject(orderId);
                Response.Cookies.Append("orderId", orderIdJson, new CookieOptions
                {
                    // 設定 Cookie 選項，例如過期時間
                    Expires = DateTimeOffset.UtcNow.AddDays(7),  // 設定過期為 7 天
                    HttpOnly = true, // 防止 JavaScript 訪問
                    Secure = true // 只在 HTTPS 連接中傳送
                });
                var entity = (await _productRepository.ListAsync(pr => products.Select(p => p.ProductId).Contains(pr.ProductId))).Select(p => new { ProductId = p.ProductId, Price = p.ProductPrice }).ToDictionary
                    (g => g.ProductId, g => g.Price);


                var orderDetails = products.Select(p =>
                {
                    entity.TryGetValue(p.ProductId, out var price);

                   return new PurchaseHistoryDetail
                    {
                        OrderId = int.Parse(orderId),
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        SalesPrice = p.SalesPrice,
                        Price = price,
                        Discount = p.Discount,
                    };
                });






                string orderDetailsJson = JsonConvert.SerializeObject(orderDetails);
                Response.Cookies.Append("orderDetails", orderDetailsJson, new CookieOptions
                {

                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true
                });

                await _unitOfWork.CommitAsync();
                // 將使用者導向 LINE Pay 付款頁面
                return Redirect(response.Info.PaymentUrl.Web);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception("有錯", ex);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }




        [HttpGet]
        [Route("api/LinePay/Confirm")]
        public async Task<IActionResult> Confirm([FromQuery] string transactionId, [FromQuery] string orderId)
        {
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");

            var order = await _orderRepository.GetByIdAsync(int.Parse(orderId));

            // 建立 LinePayApi 物件
            var linePayApi = new LinePayApi(_linePayService.GetLinePayOptions(), _configuration);
            var userAccount = (await _userRepository.GetByIdAsync(userId)).Account;

            // 建立 ConfirmRequest 物件
            var confirmRequest = new ConfirmRequest
            {
                Amount = order.TotalPrice,
                Currency = "TWD"
            };

            // 發送確認付款請求
            var confirmResponse = await linePayApi.ConfirmAsync(transactionId, confirmRequest);
            //ViewData["TransactionId"] = transactionId;
            ViewData["Account"] = userAccount;
            ViewData["OrderId"] = orderId;
            ViewData["總計"] = confirmRequest.Amount;

            // 確認付款失敗處理
            if (confirmResponse.ReturnCode != "0000")
            {
                ViewData["ReturnCode"] = confirmResponse.ReturnCode;
                ViewData["ReturnMessage"] = confirmResponse.ReturnMessage;
                return View("ConfirmFailure");
            }


            List<PurchaseHistoryDetail> orderDetails = new();
            string orderDetailsJson = Request.Cookies["orderDetails"];

            if (!string.IsNullOrEmpty(orderDetailsJson))
            {
                // 反序列化成 Dictionary<int, List<ProductDetails>>
                orderDetails = JsonConvert.DeserializeObject<List<PurchaseHistoryDetail>>(orderDetailsJson);
            }


            //建立購賣紀錄詳細
            await _linePayService.CreatePurchaseHistoryDetail(orderDetails);

            //跟改訂單狀態
            await _linePayService.ChangeOrderState(int.Parse(orderId));
            var productIds = orderDetails.Select(o => o.ProductId);

            //刪除購物車紀錄
            await _linePayService.DeleteShoppingCartItem(userId, productIds);

            //刪除願望清單紀錄
            await _linePayService.DeleteWishListItem(userId, productIds);
            await _linePayService.CreateGameLibraryItem(userId, productIds);

            //建立LinePay紀錄
            await _linePayService.CreateLinePayItem(transactionId, order, confirmRequest.Amount);

            return View("Confirm");

        }


        [HttpGet]
        [Route("api/LinePay/Cancel")]
        public IActionResult Cancel()
        {
            return View("Cancel");
        }

    }

}

