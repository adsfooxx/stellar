using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class WishCard
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int WishId { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 自訂排序
    /// </summary>
    public int WishSortId { get; set; }

    /// <summary>
    /// 加入時間
    /// </summary>
    public DateTime AddDate { get; set; }

    /// <summary>
    /// 使用者ID
    /// </summary>
    public int UserId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
