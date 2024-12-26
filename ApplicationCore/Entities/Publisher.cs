using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Publisher
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int PublisherId { get; set; }

    /// <summary>
    /// 發行商名稱
    /// </summary>
    public string? PublisherName { get; set; }

    public string? ContactName { get; set; }

    public string? Country { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public byte? Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
