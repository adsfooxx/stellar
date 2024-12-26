using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class ShoppingCartCard
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int ShoppingCartId { get; set; }

    /// <summary>
    /// 使用者ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 產品名稱
    /// </summary>
    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
