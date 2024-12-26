using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class ProductAdvertise
{
    public int AdvertiseId { get; set; }

    public string AdvertiseImgUrl { get; set; } = null!;
}
