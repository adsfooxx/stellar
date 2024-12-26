using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class ProductComment
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int CommentsId { get; set; }

    /// <summary>
    /// 產品ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 玩家ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 好評/壞評
    /// </summary>
    public bool Comment { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
