using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

/// <summary>
/// 0:照片，1:影片
/// </summary>
public partial class ProductCarousel
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int CarouselId { get; set; }

    /// <summary>
    /// 超連結
    /// </summary>
    public string CarouselUrl { get; set; } = null!;

    /// <summary>
    /// 產品ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 圖片導向超連結
    /// </summary>
    public string DataSourceUrl { get; set; } = null!;

    /// <summary>
    /// 影片/圖片
    /// </summary>
    public byte Type { get; set; }

    public virtual Product Product { get; set; } = null!;
}
