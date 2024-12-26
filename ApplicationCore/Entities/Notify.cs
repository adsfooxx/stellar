using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Notify
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int NotifyId { get; set; }

    /// <summary>
    /// 使用者ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 時間
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// 標題
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 內文
    /// </summary>
    public string Text { get; set; } = null!;

    /// <summary>
    /// 圖片
    /// </summary>
    public string ImgUrl { get; set; } = null!;

    /// <summary>
    /// 0:未讀,1:已讀
    /// </summary>
    public byte? ReadAlready { get; set; }

    public virtual User User { get; set; } = null!;
}
