using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.CodeAnalysis;
using System.Linq;
using Web.Enums;
using Web.Extensions;
using Web.ViewModels.Member;
using Web.ViewModels.Payment;


namespace Web.Services.Member
{
    public class PurchaseHistoryService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<PurchaseHistoryDetail> _purchaseHistoryDetailrRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ApplicationCore.Entities.Product> _productRepository;


        public PurchaseHistoryService(
            IRepository<Order> orderRepository,
            IRepository<PurchaseHistoryDetail> purchaseHistoryDetailrRepository,
            IRepository<User> userRepository,
            IRepository<ApplicationCore.Entities.Product> productRepository)
        {
            _orderRepository = orderRepository;
            _purchaseHistoryDetailrRepository = purchaseHistoryDetailrRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }
        public async Task<OrdersViewModel> GetData(int id)
        {
            var loginUserId = id;
            var user = await _userRepository.GetByIdAsync(loginUserId);

            var orders = (await _orderRepository.ListAsync(o => o.UserId == user.UserId && o.State == 1))
                          .Select(o => new PurchaseHistoryOrderViewModels
                          {
                              OrderId = o.OrderId,
                              PurchaseDate = o.Orderdate,
                              TransactionType = o.TransactionType,
                              PaymentType = o.PaymentTypeId.GetPaymentTypeName(), //使用 enum
                              TotalPrice = o.TotalPrice,
                              WalletChange = o.WalletChange,
                              WalletBlance = o.Walletbalance,
                              ProductList = _purchaseHistoryDetailrRepository.List(x => x.OrderId == o.OrderId)
                                  .Select(y => new PurchaseHistoryProductViewModels
                                  {
                                      ProductId = y.ProductId,
                                      ProductName = y.ProductName,
                                  }).ToList()
                          }).ToList();

            var model = new OrdersViewModel
            {
                UserId = loginUserId,
                UserAccount = user.Account,
                PurchaseHistoryList = orders
            };

            return model;
        }
    }
}
