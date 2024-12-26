using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class TagConnect
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int TagConnectId { get; set; }

    /// <summary>
    /// 產品ID
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// tag流水號
    /// </summary>
    public int TagId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
