using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Friend
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int FriendId { get; set; }

    /// <summary>
    /// 使用者
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 朋友ID
    /// </summary>
    public int FriendUserId { get; set; }

    /// <summary>
    /// 邀請狀態
    /// </summary>
    public byte State { get; set; }

    public virtual User FriendUser { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
