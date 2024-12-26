using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<ApplicationCore.Entities.Order> _orderRepository;
        private readonly IRepository<PurchaseHistoryDetail> _purchaseHistoryDetailRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork, IRepository<User> userRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task<int> CreateOrderAsync(int userId, List<PurchaseHistoryDetail> purchaseHistoryDetails)
        {
            try
            {
                await _unitOfWork.BeginAsync();
                if (purchaseHistoryDetails is null || purchaseHistoryDetails.Count == 0)
                {
                    throw new Exception(" purchase history details are required");
                }
                var orderEntity = new ApplicationCore.Entities.Order()
                {

                    OrderId = 100000,
                    Orderdate = DateTime.UtcNow,
                    PaymentTypeId = 1,
                    TotalPrice = 1000000,
                    UserId = userId,
                    TransactionType = 1,
                    User = _userRepository.GetById(userId),
                };
                var order = await _orderRepository.AddAsync(orderEntity);
                if (order is null)
                {
                    throw new Exception("Order could not be created");
                }
                foreach (var item in purchaseHistoryDetails)
                {
                    item.OrderId = order.OrderId;
                }
                await _purchaseHistoryDetailRepository.AddRangeAsync(purchaseHistoryDetails);
                await _unitOfWork.CommitAsync();
                return order.OrderId;

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception("Order could not be created", ex);
            }
            finally
            {
                _unitOfWork.Dispose();
            }

        }
    }
}
