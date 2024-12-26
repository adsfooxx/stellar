using ApplicationCore.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;

using Web.ViewModels.Member;
using Web.ViewModels.Payment;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ApplicationCore.Interfaces;
using FluentEcpay;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Product = ApplicationCore.Entities.Product;

namespace Web.Services.EcPay
{

    public class EcPayService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ApplicationCore.Entities.EcPay> _ecPayRepository;
        private readonly IRepository<ProductsDiscount> _productsDiscountRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<PurchaseHistoryDetail> _purchaseHistoryDetailRepository;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public EcPayService( IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = unitOfWork.GetRepository<Order>();
            _ecPayRepository = unitOfWork.GetRepository<ApplicationCore.Entities.EcPay>();  
            _productsDiscountRepository = unitOfWork.GetRepository<ProductsDiscount>();
            _productRepository = unitOfWork.GetRepository<Product>();
            _purchaseHistoryDetailRepository = unitOfWork.GetRepository<PurchaseHistoryDetail>();
            _configuration = configuration;
            
        }

        public async Task<IPayment> CreatePayment(PayChoiceAndDetailCheckViewModel payData)
        {


                var shoppingcart = JsonConvert.DeserializeObject<List<PayChoiceAndDetailCheckShoppingCart>>(payData.ShoppingCartJson);
                var date = DateTime.Now;
                //建立order資料

                var order = new Order()
                {

                    Orderdate = date,
                    PaymentTypeId = 1,
                    TotalPrice = payData.Total,
                    UserId = payData.UserId,
                    //付款狀態
                    State = 0,
                    TransactionType = 0,

                };
                await _orderRepository.AddAsync(order);

            try
            {
                await _unitOfWork.BeginAsync();
                var service = new
                {
                    Url = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5",
                    MerchantId = _configuration["ECPay:MerchantID"],
                    HashKey = _configuration["ECPay:HashKey"],
                    HashIV = _configuration["ECPay:HashIV"],
                    ServerUrl = _configuration["ECPay:ServerUrl"],
                    ClientUrl = _configuration["ECPay:ClientUrl"],
                };
                List<Item> productList = new List<Item>();
                foreach (var product in shoppingcart)
                {
                    var item = new Item()
                    {
                        Name = product.ProductName,
                        Price = (int)Math.Round(product.Price),
                        Quantity = 1,
                    };
                    productList.Add(item);
                }

                var transaction = new
                {
                    No = "A" + _orderRepository.FirstOrDefault(x => x.Orderdate == date).OrderId,
                    Description = "信用卡付款",
                    Date = date,
                    Method = EPaymentMethod.Credit,
                    Items = productList
                };

                IPayment payment = new PaymentConfiguration()
                    .Send.ToApi(
                        url: service.Url)
                    .Send.ToMerchant(
                        service.MerchantId)
                    .Send.UsingHash(
                        key: service.HashKey,
                        iv: service.HashIV)
                    .Return.ToServer(
                        url: service.ServerUrl)
                    .Return.ToClient(
                        url: service.ClientUrl)
                    .Transaction.New(
                        no: (transaction.No).ToString(),
                        description: transaction.Description,
                        date: transaction.Date)
                    .Transaction.UseMethod(
                        method: transaction.Method)
                    .Transaction.WithItems(
                        items: transaction.Items)
                    .Generate();
                var ecPayData = new ApplicationCore.Entities.EcPay()
                {
                    OrderId = _orderRepository.FirstOrDefault(x => x.Orderdate == date).OrderId,
                    MerchantId = payment.MerchantID,
                    MerchantTradeNo = payment.MerchantTradeNo,
                    StoreId = payment.StoreID,
                    RtnCode = 0,
                    RtnMsg = "",
                    TradeNo = payment.MerchantTradeNo,
                    TradeAmt = payment.TotalAmount,
                    PaymentDate = payment.MerchantTradeDate,
                    PaymentType = "信用卡",
                    TradeDate = payment.MerchantTradeDate,
                    CheckMacValue = payment.CheckMacValue,

                };
                //建立綠界上傳資料(建立資料庫)
                await _ecPayRepository.AddAsync(ecPayData);


                //建立詳細記錄
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                var discounts = await _productsDiscountRepository.ListAsync(d => d.SalesStartDate <= currentDate && d.SalesEndDate >= currentDate);
                List<PurchaseHistoryDetail> details = new List<PurchaseHistoryDetail>();
                foreach (var product in shoppingcart)
                {
                    var discount = discounts.FirstOrDefault(x => x.ProductId == product.ProductId)?.Discount ?? 1;

                    var purchaseHistoryProduct = new PurchaseHistoryDetail()
                    {
                        OrderId = order.OrderId,
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        SalesPrice = Math.Round(product.Price),
                        Discount = discount,
                        Price = (await _productRepository.ListAsync(x => x.ProductId == product.ProductId)).Select(x => x.ProductPrice).FirstOrDefault(),
                    };
                    details.Add(purchaseHistoryProduct);

                }
                await _purchaseHistoryDetailRepository.AddRangeAsync(details);
                await _unitOfWork.CommitAsync();
                return payment;
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
