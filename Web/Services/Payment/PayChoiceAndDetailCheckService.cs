using ApplicationCore.Interfaces;
using Newtonsoft.Json;
using Web.ViewModels.Payment;
using Product = ApplicationCore.Entities.Product;

namespace Web.Services.Payment
{
    public class PayChoiceAndDetailCheckService
    {
        private readonly IRepository<ShoppingCartCard> _shoppingCartCardRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ProductsDiscount> _productsDiscountRepository;
        private readonly IPaymentQueryService _paymentQueryService;
        public PayChoiceAndDetailCheckService(IRepository<ShoppingCartCard> shoppingCartCardRepository, IRepository<Product> productRepository, IRepository<User> userRepository, IRepository<ProductsDiscount> productsDiscountRepository, IPaymentQueryService paymentQueryService)
        {
            _shoppingCartCardRepository = shoppingCartCardRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _productsDiscountRepository = productsDiscountRepository;
            _paymentQueryService = paymentQueryService;
        }

        public async Task<PayChoiceAndDetailCheckViewModel> GetUserDatas(int userId)

        {

            var shoppingcarts =await _paymentQueryService.GetShoppingCartData(userId);
            var shoppingcartList = shoppingcarts.Select(x=>new PayChoiceAndDetailCheckShoppingCart
            {
                ProductId = x.ProductId,
                ProductImageUrl=x.ProductMainImageUrl!,
                ProductName=x.ProductName!,
                Price= Math.Round((decimal)x.SalesPrice),
                PriceToString= x.SalesPrice.ToString("#,##0"),
            }).ToList();


            var userAccount = _userRepository.FirstOrDefault((x) => x.UserId == userId).Account;

            var userData = new PayChoiceAndDetailCheckViewModel
            {
                UserId = userId,
                Account = userAccount,
                ShoppingCart = shoppingcartList
            };


            var total = userData.ShoppingCart.Sum((x) => x.Price);
            userData.Total = total;
            userData.TotalToString = total.ToString("#,##0");

            userData.ProductNameListJson = JsonConvert.SerializeObject(userData.ShoppingCart.Select(x => x.ProductName));
            userData.ShoppingCartJson = JsonConvert.SerializeObject(userData.ShoppingCart.Select(x => x));
            return userData;
        }



    }
}
