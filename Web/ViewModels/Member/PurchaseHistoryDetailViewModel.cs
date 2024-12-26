using ApplicationCore.Interfaces;
using ApplicationCore.Entities;

namespace Web.ViewModels.Member
{
    public class PurchaseHistoryDetailViewModel
    {

        //public int OrderDetailId { get; set; } //流水號
        public int OrderId { get; set; } //購買紀錄ID 與 訂單ID ??

        public string PaymentType { get; set; } //綠界 linePay steam錢包
        public string PurchaseDate { get; set; }

        public List<HistroyDetailProductViewModels> HistroyDetailProducts { get; set; }
        public decimal OrderDiscount { get; set; } //總共折扣
      
        public decimal Subtotal { get; set; } //小計=總計+折扣
        public decimal TotalPrice { get; set; }
    }

    public class HistroyDetailProductViewModels
    {

        public decimal Discount { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitePrice { get; set; } //單價
        public decimal SalesPrice { get; set; } //折扣後的單價
        public string ProductImgUrl { get; set; }
    }
}