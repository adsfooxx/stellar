namespace Web.ViewModels.Home
{
    public class HomeViewModel
    {
        //精選輪播
        public List<FeaturedProduct> FeaturedProducts { get; set; }
        //廣告
        public Advertise Advertise { get; set; }
        //特價輪播
        public List<SpecialPromotionProduct> SpecialPromotionProducts { get; set; }
        //分類輪播
        public List<CategoryProduct> CategoryProducts { get; set; }
        //瀏覽標題
        public List<BrowseCategory> BrowseCategories { get; set; }
        //Tab
        public List<TabProduct> TabFeaturedProducts { get; set; }
        public List<TabProduct> TabBestSellerProducts { get; set; }
        public List<TabProduct> TabAvailableSoonProducts { get; set; }
        public List<TabProduct> TabSpecialOfferProducts { get; set; }



    }

    public class FeaturedProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //暢銷標題
        public string BestSellersTitle { get; set; }
        public decimal Price { get; set; }
        public string FormatPrice { get { return Price.ToString("#,##"); } }
        public string ShelfTime { get; set; }
        public string MainImgUrl { get; set; }
        public List<ProductSubImg> ProductSubImgs { get; set; }
        public string SystemIconUrl { get; set; }
        public decimal Discount { get; set; }
        public decimal SalesPrice { get; set; }
        public string FormatSalesPrice { get { return SalesPrice.ToString("#,##"); } }
        public string RedirectUrl { get; set; }
    }

    public class ProductSubImg
    {
        public string SubImgId { get; set; }
        public string SubImgUrl { get; set; }
    }

    public class Advertise
    {
        public string ImgUrl { get; set; }
        public string RedirectUrl { get; set; }
    }


    public class TabProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string FormatPrice { get { return Price.ToString("#,##"); } }
        public string MainImgUrl { get; set; }
        public List<ProductSubImg> SubImgs { get; set; }
        public string SystemIconUrl { get; set; }
        public decimal Discount { get; set; }
        public decimal SalesPrice { get; set; }
        public string FormatSalesPrice { get { return SalesPrice.ToString("#,##"); } }
        public int EvaluateCount { get; set; }
        public List<HomeTag> TabTags { get; set; }
    }

    public class SpecialPromotionProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string FormatPrice { get { return Price.ToString("#,##"); } }
        public string Deadline { get; set; }
        public string ImgUrl { get; set; }
        public decimal Discount { get; set; }
        public decimal SalesPrice { get; set; }
        public string FormatSalesPrice { get { return SalesPrice.ToString("#,##"); } }
        //public string RedirectUrl { get; set; }
    }

    public class CategoryProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class BrowseCategory
    {
        public int Browse_id { get; set; }
        public string Browse_name { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class HomeTag
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
    }

}
