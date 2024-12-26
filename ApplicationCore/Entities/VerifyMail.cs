using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class VerifyMail
{
    public int VerifyId { get; set; }

    public int UserId { get; set; }

    public string EncodingParameter { get; set; } = null!;

    public DateTime Expired { get; set; }

    public virtual User User { get; set; } = null!;
}
