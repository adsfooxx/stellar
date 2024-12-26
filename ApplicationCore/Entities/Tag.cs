using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Tag
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int TagId { get; set; }

    /// <summary>
    /// 標籤名稱
    /// </summary>
    public string TagName { get; set; } = null!;

    public virtual ICollection<TagConnect> TagConnects { get; set; } = new List<TagConnect>();
}
