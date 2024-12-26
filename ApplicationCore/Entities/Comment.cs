using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Comment
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int CommentId { get; set; }

    /// <summary>
    /// 發送人
    /// </summary>
    public int SendUserId { get; set; }

    /// <summary>
    /// 收訊人
    /// </summary>
    public int ReciveUserId { get; set; }

    /// <summary>
    /// 內文
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// 留言時間
    /// </summary>
    public DateTime CreateTime { get; set; }

    public virtual User ReciveUser { get; set; } = null!;

    public virtual User SendUser { get; set; } = null!;
}
