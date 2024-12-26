

namespace Web.ViewModels.Member;

public class OrdersViewModel
{
    public int UserId { get; set; }
    public string UserAccount { get; set; }

    public List<PurchaseHistoryOrderViewModels> PurchaseHistoryList { get; set; }
  
}
//單筆購買紀錄資訊
public class PurchaseHistoryOrderViewModels
{
    public int OrderId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public List<PurchaseHistoryProductViewModels> ProductList { get; set; }
    
    public int TransactionType {  get; set; }//交易類型 ex:購買or儲值or退款，要改成enum
    public string PaymentType { get; set; }  //綠界 linePay steam錢包 要改成enum
    public decimal TotalPrice { get; set; }
    public decimal? WalletChange { get; set; }//可為null
    public decimal? WalletBlance {  get; set; }//可為null
}

public class PurchaseHistoryProductViewModels
{
    //public int OrderId { get; set; }
    public int ProductId {  get; set; }
    public string ProductName { get; set; }
 
    //public decimal SalesPrice {  get; set; }//折扣後金額
}

