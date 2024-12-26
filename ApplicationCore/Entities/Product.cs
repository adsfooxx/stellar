using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class Product
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 產品名稱
    /// </summary>
    public string ProductName { get; set; } = null!;

    /// <summary>
    /// 產品價格
    /// </summary>
    public decimal ProductPrice { get; set; }

    /// <summary>
    /// 發行日期
    /// </summary>
    public DateOnly ProductShelfTime { get; set; }

    /// <summary>
    /// 產品主圖片連結
    /// </summary>
    public string ProductMainImageUrl { get; set; } = null!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// 發行商ID
    /// </summary>
    public int PublisherId { get; set; }

    /// <summary>
    /// 或許會變成複文本
    /// </summary>
    public string? Event { get; set; }

    /// <summary>
    /// 或許會變成複文本
    /// </summary>
    public string? About { get; set; }

    /// <summary>
    /// 或許會變成複文本
    /// </summary>
    public string? SystemRequirement { get; set; }

    /// <summary>
    /// 類別ID
    /// </summary>
    public int CategoryId { get; set; }

    public byte ProductStatus { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProductCarousel> ProductCarousels { get; set; } = new List<ProductCarousel>();

    public virtual ICollection<ProductCollection> ProductCollections { get; set; } = new List<ProductCollection>();

    public virtual ICollection<ProductComment> ProductComments { get; set; } = new List<ProductComment>();

    public virtual ICollection<ProductPageAbout> ProductPageAbouts { get; set; } = new List<ProductPageAbout>();

    public virtual ICollection<ProductPageEvent> ProductPageEvents { get; set; } = new List<ProductPageEvent>();

    public virtual ICollection<ProductsDiscount> ProductsDiscounts { get; set; } = new List<ProductsDiscount>();

    public virtual Publisher Publisher { get; set; } = null!;

    public virtual ICollection<PurchaseHistoryDetail> PurchaseHistoryDetails { get; set; } = new List<PurchaseHistoryDetail>();

    public virtual ICollection<ShoppingCartCard> ShoppingCartCards { get; set; } = new List<ShoppingCartCard>();

    public virtual ICollection<TagConnect> TagConnects { get; set; } = new List<TagConnect>();

    public virtual ICollection<WishCard> WishCards { get; set; } = new List<WishCard>();
}
