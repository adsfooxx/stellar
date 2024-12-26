using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class PurchaseHistoryDetail
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int PurchaseHistoryId { get; set; }

    /// <summary>
    /// 訂單ID
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 產品ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 折扣
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// 折扣後金額
    /// </summary>
    public decimal SalesPrice { get; set; }

    /// <summary>
    /// 產品名稱
    /// </summary>
    public string ProductName { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
