using Web.ViewModels.Partial;

namespace Web.ViewModels.Member
{
    public class WishListViewModel
    {
        public int UserId { get; set; }
        public string UserImgUrl { get; set; }
        public string UserNickName { get; set; }
        public int SortType { get; set; }
        public List<WishProductCard> WishListItem { get; set; } // 願望商品的集合
        public List<AddShoppingCartViewModel> AddShoppingCartViewModels { get; set; }
        public string NoResultMessage { get; set; } // 搜尋無結果時的訊息
    }

    public class WishProductCard
    {
        public int SortID { get; set; }
        public int WishItemID { get; set; }
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public List<Tags> GameLabel { get; set; }  // 商品的標籤集合
        public GameDiscount DiscounAndSalesprice { get; set; }
        public DateTime AddDate { get; set; }
        public string DirectUrl { get; set; }
        public int ProductId { get; set; }
        public GameComment Comment { get; set; }
        public bool IsInCartList { get; set; } // 用於表示該商品是否已在購物車中
    }

    public class Tags
    {
        public int tagProductId { get; set; }
        public int tagId { get; set; }
        public string tagName { get; set; } // 每個標籤的名稱
    }

    public class GameDiscount
    {
        public int discountProductId { get; set; }
        public decimal OringinalPrice { get; set; }
        public string OringinalPriceFormat { get; set; }
        public decimal Discount { get; set; }
        public string DiscountFormat { get; set; }
        public decimal SalesPrice { get; set; }
        public string SalesPriceFormat { get; set; }
        public bool IsInDiscountTime { get; set; }
    }

    public class GameComment
    {
        public int commentProductId { get; set; }
        public int goodComments { get; set; }
        public int badComments { get; set; }
        public int percentageDifference { get; set; }
    }
}
