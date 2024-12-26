using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class ProductCollection
{
    /// <summary>
    /// 他不是遊戲庫id，他是流水號
    /// </summary>
    public int CollectionId { get; set; }

    /// <summary>
    /// 使用者ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 產品ID
    /// </summary>
    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
