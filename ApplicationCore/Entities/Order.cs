using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Order
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 購買日期
    /// </summary>
    public DateTime Orderdate { get; set; }

    /// <summary>
    /// 金流分類
    /// </summary>
    public int PaymentTypeId { get; set; }

    /// <summary>
    /// 總額
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// 使用者ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 付款狀態(ex:成功?失敗?都幾)
    /// </summary>
    public byte State { get; set; }

    /// <summary>
    /// 餘額
    /// </summary>
    public decimal? Walletbalance { get; set; }

    /// <summary>
    /// 錢包改變
    /// </summary>
    public decimal? WalletChange { get; set; }

    /// <summary>
    /// 購買/儲值
    /// </summary>
    public byte TransactionType { get; set; }

    public virtual ICollection<EcPay> EcPays { get; set; } = new List<EcPay>();

    public virtual ICollection<PurchaseHistoryDetail> PurchaseHistoryDetails { get; set; } = new List<PurchaseHistoryDetail>();

    public virtual User User { get; set; } = null!;
}
