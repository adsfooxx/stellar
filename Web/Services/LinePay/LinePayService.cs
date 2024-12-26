using ApplicationCore.Interfaces;
using ApplicationCore.LinePay.Dtos.Request;
using ApplicationCore.LinePay.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Web.Controllers;
using Web.Services.ShoppingCart;
using Web.ViewModels.ShoppingCart;
using Dapper;
using Microsoft.CodeAnalysis;
using ApplicationCore.LinePay.Dtos.Confirm;


namespace Web.Services.LinePay
{
    public class LinePayService
    {
        private readonly ILogger<LinePayController> _logger; // 日誌記錄器
        private readonly string sqlConnection;
        private readonly LinePayApiOptions _linePayApiOptions;
        private readonly IRepository<Order> _orderRepository;
        private readonly ShoppingCartService _shoppingCartService;
        private readonly IRepository<ShoppingCartCard> _shoppingCartCardRepository;
        private readonly IRepository<ProductCollection> _productCollectionRepository;
        private readonly IRepository<PurchaseHistoryDetail> _purchaseHistoryDetailRepository;
        private readonly IRepository<ApplicationCore.Entities.Product> _productRepository;
        private readonly IRepository<ProductsDiscount> _productsDiscountRepository;
        private readonly IRepository<WishCard> _wishCardRepository;
        private readonly IRepository<ApplicationCore.Entities.LinePay> _LinePayRepository;


        private readonly string _callbackUriBaseAddress  ;
        //private const string CallbackUriBaseAddress = "https://stellarstellar.azurewebsites.net/";
        //private const string CallbackUriBaseAddress = "  https://localhost:7168/";

      

        public LinePayService(
            IHttpClientFactory httpClientFactory,
            IRepository<Order> orderRepository,
             IRepository<ProductCollection> productCollectionRepository,
            IRepository<PurchaseHistoryDetail> orderDetailRepository,
             ShoppingCartService shoppingCartService,
             IRepository<ApplicationCore.Entities.Product> productRepository,
             IConfiguration configuration,
              IRepository<ProductsDiscount> productsDiscountRepository,
              IRepository<WishCard> wishCardRepository,
              IRepository<ShoppingCartCard> shoppingCartCardRepository,
              ILogger<LinePayController> logger,
              IOptions<LinePayApiOptions> options,
              IRepository<ApplicationCore.Entities.LinePay> linePayRepository,
              IConfiguration configuration1
               )

        {
            _callbackUriBaseAddress = configuration["LinePayApi:CallbackUriBaseAddress"];

            var linePayApiOptions = options?.Value ??
                throw new ArgumentException(nameof(options), "linePayApiOptions is required.");

            sqlConnection = configuration.GetConnectionString("StellarDB");

            // 透過 IHttpClientFactory 建立 HttpClient 物件
            var httpClient = httpClientFactory.CreateClient();

            // 設定 LinePayApiOptions 物件
            _linePayApiOptions = new LinePayApiOptions
            {
                ChannelId = linePayApiOptions.ChannelId,
                ChannelSecret = linePayApiOptions.ChannelSecret,
                HttpClient = httpClient,
                IsSandBox = linePayApiOptions.IsSandBox, // 是否為測試環境
                BaseAddress = linePayApiOptions.BaseAddress, //可以自行設定`BaseAddress`決定使用的API環境，或用`IsSandBox`來設定。
            };
            _productCollectionRepository = productCollectionRepository;
            _orderRepository = orderRepository;
            _purchaseHistoryDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
            _productsDiscountRepository = productsDiscountRepository;
            _shoppingCartService = shoppingCartService;
            _wishCardRepository = wishCardRepository;
            _shoppingCartCardRepository = shoppingCartCardRepository;
            _logger = logger;
            _LinePayRepository = linePayRepository;
            //_unitOfWork = unitOfWork;

        }

        private const string TWD = "TWD";
        private List<Package> packages;
        private Order order;


        public LinePayApiOptions GetOptions()
        {
            return _linePayApiOptions;
        }

        public async Task<PaymentRequest> CreatePayment(int userId, List<ShoppingCartProduct> products)
        {
            var items = (await _shoppingCartService.GetShoppingCartData(userId));
           
            order = this.CreateOrder(userId, items);
            await _orderRepository.AddAsync(order);

            // 確保 OrderId 現在是可用的
            if (order.OrderId <= 0)
            {
                throw new InvalidOperationException("Order ID is not set after saving the order.");
            }

            var orderId = order.OrderId; 
            // 建立 LinePayApi 物件

            // 生成確認和取消的 URL
            string confirmUrl, cancelUrl;
            (confirmUrl, cancelUrl) = GeneratePaymentUrls(_callbackUriBaseAddress);

            CreatePackages(products);

            // 建立 PaymentRequest 物件
            PaymentRequest paymentRequest = CreatePaymentRequest(confirmUrl, cancelUrl);



            return paymentRequest;
        }


        public LinePayApiOptions GetLinePayOptions()
        {
            return _linePayApiOptions;
        }
        private PaymentRequest CreatePaymentRequest(string confirmUrl, string cancelUrl)
        {
            return new PaymentRequest
            {
                Currency = TWD,
                OrderId = order.OrderId.ToString(),
                RedirectUrls = new RedirectUrls
                {
                    ConfirmUrl = confirmUrl,
                    CancelUrl = cancelUrl
                },
                Packages = packages
            };
        }

        private void CreatePackages(List<ShoppingCartProduct> products)
        {

            // 創建 Packages 列表
            packages = products.Select(p => new Package
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Package",
                Products = new List<ApplicationCore.LinePay.Dtos.Request.Product>
        {
            new ApplicationCore.LinePay.Dtos.Request.Product
            {
                Id = p.ProductId.ToString(),
                Name = p.ProductName,
                Quantity = 1,
                Price = p.SalesPrice,
                ImageUrl=p.ProductImgUrl,
            }
        }
            }).ToList();
        }


        private Order CreateOrder(int userId, ShoppingCartViewModel items)
        {
            return new Order()
            {
                Orderdate = DateTime.UtcNow,
                PaymentTypeId = 2,
                TotalPrice = items.TotalPrice,
                UserId = userId,
                State = 0,
                TransactionType = 0,
            };
        }



        private (string confirmUrl, string cancelUrl) GeneratePaymentUrls(string baseAddress)
        {
            var callbackUri = new Uri(baseAddress);
            var confirmUrl = new Uri(callbackUri, "api/LinePay/Confirm").ToString();
            var cancelUrl = new Uri(callbackUri, "api/LinePay/Cancel").ToString();
            return (confirmUrl, cancelUrl);
        }
        public async Task<int?> GetOrderId(int userId)
        {
            string sql = """
                    SELECT TOP 1 OrderId 
                    FROM [Order] 
                    WHERE UserId = @UserId 
                    ORDER BY OrderDate DESC
                """
            ;

            using (SqlConnection conn = new SqlConnection(sqlConnection))
            {
                var OrderId = await conn.ExecuteScalarAsync<int?>(sql, new { UserId = userId });

                if (OrderId is not null)
                {
                    return OrderId;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task CreateLinePayItem(string transactionId, Order order ,decimal amount, string currency="TWD") {
            ApplicationCore.Entities.LinePay linePay = new ()
            {
                OrderId = order.OrderId.ToString(),
                TransactionId = transactionId,
                Amount = amount,
                Currency = currency,
                Status = order.State.ToString(),
                CreatedAt = DateTime.UtcNow,
            };
            await _LinePayRepository.AddAsync(linePay);
        }


        public async Task DeleteWishListItem(int userId, IEnumerable<int> productIds)
        {
            var wishCardEntities = await _wishCardRepository.ListAsync(w => w.UserId == userId && productIds.Contains(w.ProductId));
            await _wishCardRepository.DeleteRangeAsync(wishCardEntities);
        }

        public async Task DeleteShoppingCartItem(int userId, IEnumerable<int> productIds)
        {
            var shoppingCartEntities = await _shoppingCartCardRepository.ListAsync(s => s.UserId == userId && productIds.Contains(s.ProductId));

            await _shoppingCartCardRepository.DeleteRangeAsync(shoppingCartEntities);
        }

        public async Task CreateGameLibraryItem(int userId, IEnumerable<int> productIds)
        {

            List<ProductCollection> gameLibraryEntities = new();
            gameLibraryEntities = productIds.Select(id => new ProductCollection { UserId = userId, ProductId = id }).ToList();

            await _productCollectionRepository.AddRangeAsync(gameLibraryEntities);
        }


        public async Task CreatePurchaseHistoryDetail(List<PurchaseHistoryDetail> purchaseHistorys)
        {
            List<PurchaseHistoryDetail> details = new List<PurchaseHistoryDetail>();
            details = purchaseHistorys.Select(p => new PurchaseHistoryDetail
            {
                OrderId = p.OrderId,
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price,
                SalesPrice = p.SalesPrice,
                Discount = p.Discount,
            }).ToList();

            await _purchaseHistoryDetailRepository.AddRangeAsync(details);
        }

        public async Task ChangeOrderState(int orderId)
        {
            var state = (await _orderRepository.GetByIdAsync(orderId));
            state.State = 1;
            await _orderRepository.UpdateAsync(state);
        }
    }
}

