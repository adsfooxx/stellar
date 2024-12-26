using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.LinePay.Dtos.Request;
using ApplicationCore.Model.WebApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stellar.API.Dtos.Charts;
using Stellar.API.Dtos.Products;
using Stellar.API.Dtos.Supplier;
using System.Drawing;
using static Stellar.API.Dtos.Charts.ChartsDto;

namespace Stellar.API.Service.Charts
{
    public class ChartsService
    {

        private readonly IRepository<ApplicationCore.Entities.Category> _categoryRepository;
        private readonly IRepository<ApplicationCore.Entities.PurchaseHistoryDetail> _purchaseHistoryDetailRepository;
        private readonly IRepository<ApplicationCore.Entities.Product> _productRepository;
        private readonly IRepository<ApplicationCore.Entities.WishCard> _wishcardRepository;
        private readonly IRepository<ApplicationCore.Entities.Order> _orderRepository;

        public ChartsService(IRepository<Category> categoryRepository, IRepository<PurchaseHistoryDetail> purchaseHistoryDetailRepository, IRepository<ApplicationCore.Entities.Product> productRepository, IRepository<WishCard> wishcardRepository, IRepository<Order> orderRepository)
        {
            _categoryRepository = categoryRepository;
            _purchaseHistoryDetailRepository = purchaseHistoryDetailRepository;
            _productRepository = productRepository;
            _wishcardRepository = wishcardRepository;
            _orderRepository = orderRepository;
        }

        public async Task<ChartsDto> GetCharts()
        {
            //1.各類別遊戲的擁有人數 Polar Area
            var Chart_Cate_Game_Purchasetime_result = from phds in await _purchaseHistoryDetailRepository.ListAsync()
                                                      join p in await _productRepository.ListAsync()
                                                      on phds.ProductId equals p.ProductId
                                                      join cate in await _categoryRepository.ListAsync()
                                                      on p.CategoryId equals cate.CategoryId
                                                      group new { phds, p, cate } by cate.CategoryId into grouped
                                                      select new Chart_Cate_Game_Purchasetime
                                                      {
                                                          CategoryName = grouped.FirstOrDefault().cate.CategoryName,
                                                          EachCatePurchaseTimes = grouped.Count() // 計算每個產品的購買次數
                                                      };

            //2.擁有人數最多的遊戲(前10 )  直線圖Vertical Bar
            var Chart_Cate_Game_OwnerNum_result = (from phds in await _purchaseHistoryDetailRepository.ListAsync()
                                                   join p in await _productRepository.ListAsync()
                                                   on phds.ProductId equals p.ProductId
                                                   join cate in await _categoryRepository.ListAsync()
                                                   on p.CategoryId equals cate.CategoryId
                                                   group new { phds, p, cate } by cate.CategoryId into grouped
                                                   select new Chart_Cate_Game_OwnerNum
                                                   {
                                                       CategoryName = grouped.FirstOrDefault().cate.CategoryName, // 取得該分類的名稱
                                                       EachCateOwnerNum = grouped.Count() // 計算該分類的購買次數
                                                   })
                                                    .OrderByDescending(x => x.EachCateOwnerNum) // 根據購買次數由大到小排序
                                                    .Take(10); // 只取購買次數最多的前 10 個分類                                                   
            //3.發行商遊戲數量/類別 直線圖Vertical Bar
            //4.願望清單內遊戲/類別 Horizontal Bar
            var Chart_Wishlist_Game_Cate_Count_result = (from w in await _wishcardRepository.ListAsync()
                                                         join p in await _productRepository.ListAsync()
                                                         on w.ProductId equals p.ProductId
                                                         join cate in await _categoryRepository.ListAsync()
                                                         on p.CategoryId equals cate.CategoryId
                                                         group new { w, p, cate } by cate.CategoryId into grouped
                                                         select new Chart_Wishlist_Game_Cate_Count
                                                         {
                                                             CategoryName = grouped.FirstOrDefault().cate.CategoryName,
                                                             AllWishlistEachCateGameNum = grouped.Count()
                                                         })
                                                         .OrderByDescending(x => x.AllWishlistEachCateGameNum)
                                                         .Take(10);
            //5.各遊戲評論數量(各遊戲好壞評論) Vertical Bar
            //6.各付款方式人數 Horizontal Bar
            //var Chart_Payment_Num_result = from o in await _orderRepository.ListAsync()
            //                               group o by o.PaymentTypeId into grouped
            //                               select new Chart_Payment_Num
            //                               {
            //                                   PaymentType = grouped.Key,
            //                                   EachPaymentNum = grouped.Count()
            //                               };
            var Chart_Payment_Num_result = from o in await _orderRepository.ListAsync()
                                           where o.State == 1 // 只選擇 State 值為 1 的訂單
                                           group new { o.PaymentTypeId, o.State } by o.PaymentTypeId into grouped
                                           select new Chart_Payment_Num
                                           {
                                               PaymentType = grouped.Key, // 取得分組的付款方式識別碼
                                               EachPaymentNum = grouped.Count(), // 計算分組內的人數
                                               StateSuccess = grouped.Select(g => g.State).FirstOrDefault() // 取得該分組中的 State 欄位
                                           };






            //傳回所有圖表
            var chartsDtos = new ChartsDto
            {
                //1.各類別遊戲的擁有人數 Polar Area
                Chart_CGP = Chart_Cate_Game_Purchasetime_result.ToList(),
                //2.擁有人數最多的遊戲(前10 )  直線圖Vertical Bar
                Chart_CGO = Chart_Cate_Game_OwnerNum_result.ToList(),
                //3.發行商遊戲數量/類別 直線圖Vertical Bar
                //4.願望清單內遊戲/類別 Horizontal Bar
                Chart_WGCC = Chart_Wishlist_Game_Cate_Count_result.ToList(),
                //5.各遊戲評論數量(各遊戲好壞評論) Vertical Bar
                //6.各付款方式人數 Horizontal Bar
                Chart_PN = Chart_Payment_Num_result.ToList(),
            };
            return chartsDtos;
        }
    }
}
