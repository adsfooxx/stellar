using Web.ViewModels.Member;

namespace Web.ViewModels.Product
{
    public class ProductPageViewModel
    {
        public int ProductId { get; set; }
        //public int PageTitle { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
        //判斷有無願望清單
        public bool IsInWishList { get; set; }
        public bool IsInCollectionList { get; set; }
        public bool IsInCartList { get; set; }
        public bool IsInComingSoonList { get; set; }
        public bool IsGoodCommentAlready { get; set; }
        public bool IsBadCommentAlready { get; set; }
        public ProductPageMainArea ProductPageMainArea { get; set; }
        public decimal Price { get; set; }
        public string FormattedPrice { get; set; }
        public decimal Discount { get; set; }
        public string FormattedDiscount { get; set; }
        public string FormattedDiscountForTitle { get; set; }
        public decimal SalesPrice { get; set; }
        public string FormattedSalesPrice { get; set; }
        public bool IsDiscountGame { get; set; }
        public bool IsFreeGame { get; set; }
        public bool IsOriginalPrice { get; set; }
        public List<ProductPageEvent> ProductPageEventList { get; set; } = new List<ProductPageEvent>();
        public bool IsEventExist { get; set; }
        public List<ProductPageFriend> ProductPageFriendList { get; set; }
        public int CountFriends { get; set; }
        public List<ProductPageLanguage> ProductPageLanguageList { get; set; }
        public ProductPageAbout ProductPageAbout { get; set; } = new ProductPageAbout
        {
            AboutCardList = new List<AboutCard>()
        };
        public bool IsAboutExist { get; set; }
        //public SystemRequirements SystemRequirements { get; set; }
        public string? SystemRequirements { get; set; }
        public List<SimilarProduct> SimilarProductList { get; set; }

        //Comment CSS
        public string CommentsClass { get; set; }
    }


    public class ProductPageMainArea
    {
        public int Id { get; set; }
        //public string ProductName { get; set; }
        public List<Carousel> CarouselUrlList { get; set; }
        //public List<CarouselVideo> VideoUrlList { get; set; }
        public string MainImg { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public double TotalComment { get; set; }
        public DateOnly ShelfTime { get; set; }

        public string FormattedShelfTime { get; set; }
        public string Publisher { get; set; }
        public List<Tag> TagList { get; set; }

    }

    //public class CarouselVideo
    //{
    //    public int Id { get; set; }
    //    public string VideoUrl { get; set; }
    //}

    public class Carousel
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public string DataSrcUrl { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductPageEvent
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public string AnnounceText { get; set; }
        public DateOnly AnnounceTime { get; set; }
        public string FormattedAnnounceTime { get; set; }

    }

    public class ProductPageFriend
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public byte Online { get; set; }
        public string FriendName { get; set; }
    }

    public class ProductPageLanguage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool InterfaceSupport { get; set; }
        public bool FullVoiceSupport { get; set; }
        public bool SubtitlesSupport { get; set; }
    }

    public class ProductPageAbout
    {
        public int Id { get; set; }
        public string AboutMainTitle { get; set; }

        public List<AboutCard> AboutCardList { get; set; }
    }

    public class AboutCard
    {
        public string ImgUrl { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }

    //public class SystemRequirements
    //{
    //    public string MinimumEquipment { get; set; }
    //    public string RecommendedEquipment { get; set; }
    //}

    public class SimilarProduct
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string FormattedPrice { get; set; }
    }

}
