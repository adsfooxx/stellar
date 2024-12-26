using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class ProductPageEvent
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int EventsId { get; set; }

    /// <summary>
    /// 產品名稱
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 標題
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 活動文本
    /// </summary>
    public string AnnounceText { get; set; } = null!;

    /// <summary>
    /// 內文
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// 活動照片
    /// </summary>
    public string? ProductImgUrl { get; set; }

    /// <summary>
    /// 活動時間
    /// </summary>
    public DateOnly AnnouncementDate { get; set; }

    public virtual Product Product { get; set; } = null!;
}
