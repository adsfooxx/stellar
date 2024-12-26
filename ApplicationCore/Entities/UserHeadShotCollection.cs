using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class UserHeadShotCollection
{
    public int HeadShotId { get; set; }

    public int UserId { get; set; }

    public string HeadShotUrl { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
