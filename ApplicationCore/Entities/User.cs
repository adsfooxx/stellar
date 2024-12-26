using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class User
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 匿名
    /// </summary>
    public string NickName { get; set; } = null!;

    /// <summary>
    /// 真實姓名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 大頭貼
    /// </summary>
    public string? UserImg { get; set; }

    /// <summary>
    /// 帳號 只能唯一
    /// </summary>
    public string Account { get; set; } = null!;

    /// <summary>
    /// 錢包餘額
    /// </summary>
    public decimal WalletAmount { get; set; }

    /// <summary>
    /// 國家
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 自介
    /// </summary>
    public string? Overview { get; set; }

    /// <summary>
    /// 電郵
    /// </summary>
    public string? EmailAddress { get; set; }

    /// <summary>
    /// 電話號碼
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 會員驗證
    /// </summary>
    public byte State { get; set; }

    /// <summary>
    /// 連線狀態 暫定
    /// </summary>
    public byte Online { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// 帳號是否被鎖住(例如被刪掉)0:鎖住;1:還能活動
    /// </summary>
    public byte IsLocked { get; set; } = 1;

    public virtual ICollection<Comment> CommentReciveUsers { get; set; } = new List<Comment>();

    public virtual ICollection<Comment> CommentSendUsers { get; set; } = new List<Comment>();

    public virtual ICollection<Friend> FriendFriendUsers { get; set; } = new List<Friend>();

    public virtual ICollection<Friend> FriendUsers { get; set; } = new List<Friend>();

    public virtual ICollection<HistoryName> HistoryNames { get; set; } = new List<HistoryName>();

    public virtual ICollection<MessageCard> MessageCards { get; set; } = new List<MessageCard>();

    public virtual ICollection<Notify> Notifies { get; set; } = new List<Notify>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductCollection> ProductCollections { get; set; } = new List<ProductCollection>();

    public virtual ICollection<ProductComment> ProductComments { get; set; } = new List<ProductComment>();

    public virtual ICollection<ShoppingCartCard> ShoppingCartCards { get; set; } = new List<ShoppingCartCard>();

    public virtual ICollection<UserHeadShotCollection> UserHeadShotCollections { get; set; } = new List<UserHeadShotCollection>();

    public virtual ICollection<VerifyMail> VerifyMails { get; set; } = new List<VerifyMail>();

    public virtual ICollection<WishCard> WishCards { get; set; } = new List<WishCard>();
}
