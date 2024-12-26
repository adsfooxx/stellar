using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class AboutCardList
{
    public string ImageUrl { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Text { get; set; } = null!;

    public int AboutCardId { get; set; }

    public int AboutId { get; set; }

    public virtual ProductPageAbout About { get; set; } = null!;
}
