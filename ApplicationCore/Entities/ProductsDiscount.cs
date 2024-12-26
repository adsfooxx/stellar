using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class ProductsDiscount
{
    public int DiscountId { get; set; }

    public int ProductId { get; set; }

    public DateOnly? SalesStartDate { get; set; }

    public DateOnly SalesEndDate { get; set; }

    public decimal Discount { get; set; }

    public virtual Product Product { get; set; } = null!;
}
