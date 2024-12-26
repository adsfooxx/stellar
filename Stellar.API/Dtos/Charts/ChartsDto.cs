namespace Stellar.API.Dtos.Charts
{
    public class ChartsDto
    {
        //1.各類別遊戲的擁有人數 Polar Area
        public List<Chart_Cate_Game_Purchasetime> Chart_CGP { get; set; }
        //2.擁有人數最多的遊戲(前10 )  直線圖Vertical Bar
        public List<Chart_Cate_Game_OwnerNum> Chart_CGO { get; set; }
        //3.發行商遊戲數量/類別 直線圖Vertical Bar

        //4.願望清單內遊戲/類別 Horizontal Bar
        public List<Chart_Wishlist_Game_Cate_Count> Chart_WGCC { get; set; }
        //5.各遊戲評論數量(各遊戲好壞評論) Vertical Bar

        //6.各付款方式人數 Horizontal Bar
        public List<Chart_Payment_Num> Chart_PN { get; set; }
    }



    //1.各類別遊戲的擁有人數 Polar Area
    public class Chart_Cate_Game_Purchasetime
    {
        public string CategoryName { get; set; }
        public int EachCatePurchaseTimes { get; set; }
    }
    //2.擁有人數最多的遊戲(前10 )  直線圖Vertical Bar
    public class Chart_Cate_Game_OwnerNum
    {
        public string CategoryName { get; set; }
        public int EachCateOwnerNum { get; set; }
    }
    //3.發行商遊戲數量/類別 直線圖Vertical Bar

    //4.願望清單內遊戲/類別 Horizontal Bar
    public class Chart_Wishlist_Game_Cate_Count
    {
        public string CategoryName { get; set; }
        public int AllWishlistEachCateGameNum { get; set; }
    }
    //5.各遊戲評論數量(各遊戲好壞評論) Vertical Bar

    //6.各付款方式人數 Horizontal Bar
    public class Chart_Payment_Num
    {
        public int PaymentType { get; set; }
        public int EachPaymentNum { get; set; }
        public byte StateSuccess { get; set; }
    }
}
