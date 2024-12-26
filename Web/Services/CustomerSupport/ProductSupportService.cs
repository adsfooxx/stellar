using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Web.ViewModels.CustomerSupport;

namespace Web.Services.CustomerService
{
    public  class ProductSupportService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<PurchaseHistoryDetail> _purchaseHistoryDetailRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ICustomerSupportQueryService _customerSupportQueryService;

        public ProductSupportService(IRepository<Order> orderRepository, IRepository<PurchaseHistoryDetail> purchaseHistoryDetailRepository, IRepository<Product> productRepository, IRepository<User> userRepository, ICustomerSupportQueryService customerSupportQueryService)
        {
            _orderRepository = orderRepository;
            _purchaseHistoryDetailRepository = purchaseHistoryDetailRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _customerSupportQueryService = customerSupportQueryService;
        }

        public async Task<ProductSupportViewModel> GetProductSupportData(int userId, int productId)
        {
            
            var result= await _customerSupportQueryService.GetProductInProductSupport( userId, productId );
            var product = new ProductSupportViewModel
            {
                UserId = userId,
                Account = result.Account,
                ProductId = productId,
                ProductName = result.ProductName,
                ProductImgUrl = result.ProductMainImageUrl,
                PurchasedDate = (result.OrderDate is not null) ? DateTime.Parse(result.OrderDate).ToString("yyyy年MM月dd日") : "-1"
            };
            
            

            return product;
        }
    }
}
