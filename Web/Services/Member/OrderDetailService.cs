using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.CodeAnalysis;
using Web.Extensions;
using Web.ViewModels.Member;

namespace Web.Services.Member
{
    public class OrderDetailService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<PurchaseHistoryDetail> _purchaseHistoryDetailRepository;
        private readonly IRepository<Product> _productRepository;

        public OrderDetailService(
            IRepository<Order> orderRepository,
            IRepository<PurchaseHistoryDetail> purchaseHistoryDetailrRepository,
            IRepository<Product> productRepository)
        {
            _orderRepository = orderRepository;
            _purchaseHistoryDetailRepository = purchaseHistoryDetailrRepository;
            _productRepository = productRepository;
        }

        public async Task<PurchaseHistoryDetailViewModel> GetOrderDetailById(int orderId)
        {
            List<HistroyDetailProductViewModels> test = new List<HistroyDetailProductViewModels> { };

            var order = await _orderRepository.GetByIdAsync(orderId);

            var productList = _purchaseHistoryDetailRepository.List(o => o.OrderId == orderId)
                         .Select(g => new HistroyDetailProductViewModels
                         {
                             ProductId = g.ProductId,
                             ProductName = g.ProductName,
                             SalesPrice = g.SalesPrice,
                             UnitePrice = g.Price,
                             ProductImgUrl = _productRepository.List(p => p.ProductId == g.ProductId)
                                            .Select(p => p.ProductMainImageUrl).FirstOrDefault(),
                             Discount = g.Discount
                         }).ToList();

            var subtotal = productList.Sum(p => p.UnitePrice);

            var totalDiscount = productList.Sum(p => p.UnitePrice - p.SalesPrice);


            var orderDetail = new PurchaseHistoryDetailViewModel
            {
                OrderId = order.OrderId,
                PaymentType = order.PaymentTypeId.GetPaymentTypeName(),
                PurchaseDate = order.Orderdate.ToString("yyyy年MM月dd日"),
                Subtotal = Math.Round(subtotal),
                TotalPrice = Math.Round(order.TotalPrice),
                HistroyDetailProducts = productList,
                OrderDiscount = Math.Round(totalDiscount),
            };
            return orderDetail;
        }
    }
}
