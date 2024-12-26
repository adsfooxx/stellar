using ApplicationCore.Dtos;
using ApplicationCore.Interfaces;
using Web.ViewModels.Payment;
using Product = ApplicationCore.Entities.Product;

namespace Web.Services.Payment
{
    public class PayFinishedService
    {
        private readonly IRepository<ShoppingCartCard> _shoppingCartCardRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IPaymentQueryService _iPaymentQueryServiceQueryService;
        private readonly IRepository<ApplicationCore.Entities.EcPay> _ecPayRepository;
        private readonly IRepository<PurchaseHistoryDetail> _purchaseHistoryDetailRepository;
        private readonly IRepository<ProductCollection> _productCollectionRepository;
        private readonly IRepository<WishCard> _wishCardRepository;
        private readonly IUnitOfWork _unitOfWork;


        public PayFinishedService(IPaymentQueryService iPaymentQueryServiceQueryService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _shoppingCartCardRepository = _unitOfWork.GetRepository<ShoppingCartCard>();
            _productRepository = _unitOfWork.GetRepository<Product>();
            _userRepository = _unitOfWork.GetRepository<User>();
            _orderRepository = _unitOfWork.GetRepository<Order>();
            _iPaymentQueryServiceQueryService = iPaymentQueryServiceQueryService;
            _ecPayRepository = _unitOfWork.GetRepository<ApplicationCore.Entities.EcPay>();
            _productCollectionRepository = _unitOfWork.GetRepository<ProductCollection>();
            _purchaseHistoryDetailRepository = _unitOfWork.GetRepository<PurchaseHistoryDetail>();
            _wishCardRepository = _unitOfWork.GetRepository<WishCard>();

        }

        public async Task<PayFinishedViewModel> GetOrderData(int userId, int orderId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var order = await _orderRepository.GetByIdAsync(orderId);
            var orderData = new PayFinishedViewModel
            {
                UserId = userId,
                Account = user.Account,
                OrderId = orderId.ToString(),
            };
            var total = order.TotalPrice.ToString("#,##0");
            orderData.Total = total;
            return orderData;
        }
        public async Task<int> PaymentFinishedNextStep(string merchanttradeNo)
        {
            try
            {
                await _unitOfWork.BeginAsync();

                //找到這次新增的那筆訂單
                var orderId = _ecPayRepository.FirstOrDefault(x => x.MerchantTradeNo == merchanttradeNo).OrderId;
                //改變訂單狀態
                var order = await _orderRepository.FirstOrDefaultAsync(o => o.OrderId == orderId);
                order.State = 1;
                await _orderRepository.UpdateAsync(order);
                //找到ecpay的資料，並修改
                var ecpaydata = await _ecPayRepository.FirstOrDefaultAsync(o => o.OrderId == orderId);
                ecpaydata.RtnCode = 1;
                await _ecPayRepository.UpdateAsync(ecpaydata);
                //新增至遊戲庫
                List<ProductCollection> prdocuts = new List<ProductCollection>();
                var games = _purchaseHistoryDetailRepository.List(x => x.OrderId == orderId);
                foreach (var game in games)
                {
                    var gameToCollection = new ProductCollection()
                    {
                        UserId = order.UserId,
                        ProductId = game.ProductId
                    };
                    prdocuts.Add(gameToCollection);
                }
                await _productCollectionRepository.AddRangeAsync(prdocuts);
                //刪掉購物車
                var shoppingcart = await _shoppingCartCardRepository.ListAsync(x => x.UserId == order.UserId);
                await _shoppingCartCardRepository.DeleteRangeAsync(shoppingcart);

                var productIds = shoppingcart.Select(p => p.ProductId).ToList();
                var whiscard = await _wishCardRepository.ListAsync(x => x.UserId == order.UserId && productIds.Contains(x.ProductId));
                await _wishCardRepository.DeleteRangeAsync(whiscard);
                await _unitOfWork.CommitAsync();

                return orderId;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return -1;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
    }
}
