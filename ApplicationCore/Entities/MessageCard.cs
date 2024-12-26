using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class MessageCard
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// 發送人ID
    /// </summary>
    public int SendToUserId { get; set; }

    /// <summary>
    /// 聊天內容
    /// </summary>
    public string CommitText { get; set; } = null!;

    /// <summary>
    /// 發話時間
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 收文人
    /// </summary>
    public int SendByUsetId { get; set; }

    public byte State { get; set; }

    public virtual User SendToUser { get; set; } = null!;
}
