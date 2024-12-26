using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Category
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 大類別名稱
    /// </summary>
    public string CategoryName { get; set; } = null!;

    /// <summary>
    /// 類別圖片
    /// </summary>
    public string? CategoryImgUrl { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
