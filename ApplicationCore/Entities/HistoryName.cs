using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class HistoryName
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int HistoryId { get; set; }

    /// <summary>
    /// 使用者ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 歷史名稱
    /// </summary>
    public string OldName { get; set; } = null!;

    /// <summary>
    /// 時間
    /// </summary>
    public DateOnly Time { get; set; }

    public virtual User User { get; set; } = null!;
}
