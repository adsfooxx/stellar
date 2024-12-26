using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class ProductPageAbout
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int AboutId { get; set; }

    /// <summary>
    /// 產品ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 關於的標題
    /// </summary>
    public string AboutMainTitle { get; set; } = null!;

    public virtual ICollection<AboutCardList> AboutCardLists { get; set; } = new List<AboutCardList>();

    public virtual Product Product { get; set; } = null!;
}
