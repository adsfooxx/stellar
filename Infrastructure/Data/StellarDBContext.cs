using System;
using System.Collections.Generic;
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public partial class StellarDBContext : DbContext
{
    public StellarDBContext()
    {
    }

    public StellarDBContext(DbContextOptions<StellarDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AboutCardList> AboutCardLists { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<EcPay> EcPays { get; set; }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<HistoryName> HistoryNames { get; set; }

    public virtual DbSet<LinePay> LinePays { get; set; }

    public virtual DbSet<MessageCard> MessageCards { get; set; }

    public virtual DbSet<Notify> Notifies { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductAdvertise> ProductAdvertises { get; set; }

    public virtual DbSet<ProductCarousel> ProductCarousels { get; set; }

    public virtual DbSet<ProductCollection> ProductCollections { get; set; }

    public virtual DbSet<ProductComment> ProductComments { get; set; }

    public virtual DbSet<ProductPageAbout> ProductPageAbouts { get; set; }

    public virtual DbSet<ProductPageEvent> ProductPageEvents { get; set; }

    public virtual DbSet<ProductsDiscount> ProductsDiscounts { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<PurchaseHistoryDetail> PurchaseHistoryDetails { get; set; }

    public virtual DbSet<ShoppingCartCard> ShoppingCartCards { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TagConnect> TagConnects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserHeadShotCollection> UserHeadShotCollections { get; set; }

    public virtual DbSet<VerifyMail> VerifyMails { get; set; }

    public virtual DbSet<WishCard> WishCards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:StellarDB");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Chinese_Taiwan_Stroke_CI_AS");

        modelBuilder.Entity<AboutCardList>(entity =>
        {
            entity.HasKey(e => e.AboutCardId);

            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.About).WithMany(p => p.AboutCardLists)
                .HasForeignKey(d => d.AboutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AboutCardLists_ProductPageAbouts");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.CategoryId).HasComment("流水號");
            entity.Property(e => e.CategoryImgUrl).HasComment("類別圖片");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasComment("大類別名稱");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK_Commends");

            entity.Property(e => e.CommentId).HasComment("流水號");
            entity.Property(e => e.Content)
                .HasMaxLength(2000)
                .HasComment("內文");
            entity.Property(e => e.CreateTime)
                .HasComment("留言時間")
                .HasColumnType("datetime");
            entity.Property(e => e.ReciveUserId).HasComment("收訊人");
            entity.Property(e => e.SendUserId).HasComment("發送人");

            entity.HasOne(d => d.ReciveUser).WithMany(p => p.CommentReciveUsers)
                .HasForeignKey(d => d.ReciveUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Commends_Users1");

            entity.HasOne(d => d.SendUser).WithMany(p => p.CommentSendUsers)
                .HasForeignKey(d => d.SendUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Commends_Users");
        });

        modelBuilder.Entity<EcPay>(entity =>
        {
            entity.ToTable("EcPay");

            entity.Property(e => e.EcPayId).HasComment("流水號");
            entity.Property(e => e.CheckMacValue).HasComment("檢查碼");
            entity.Property(e => e.MerchantId)
                .HasMaxLength(10)
                .HasComment("特店編號")
                .HasColumnName("MerchantID");
            entity.Property(e => e.MerchantTradeNo)
                .HasMaxLength(20)
                .HasComment("特電交易編號");
            entity.Property(e => e.OrderId).HasComment("訂單ID");
            entity.Property(e => e.PaymentDate)
                .HasMaxLength(20)
                .HasComment("付款時間");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(20)
                .HasComment("特電選擇的付款方式");
            entity.Property(e => e.PaymentTypeChargeFee).HasComment("交易手續費金額");
            entity.Property(e => e.RtnCode).HasComment("交易狀態");
            entity.Property(e => e.RtnMsg)
                .HasMaxLength(200)
                .HasComment("交易訊息");
            entity.Property(e => e.SimulatePaid).HasComment("是否為模擬付款");
            entity.Property(e => e.StoreId)
                .HasMaxLength(20)
                .HasComment("特電旗下店鋪代號")
                .HasColumnName("StoreID");
            entity.Property(e => e.TradeAmt).HasComment("交易金額");
            entity.Property(e => e.TradeDate)
                .HasMaxLength(20)
                .HasComment("訂單成立時間");
            entity.Property(e => e.TradeNo)
                .HasMaxLength(20)
                .HasComment("綠界的交易編號");

            entity.HasOne(d => d.Order).WithMany(p => p.EcPays)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EcPay_Order");
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.ToTable("Friend");

            entity.Property(e => e.FriendId).HasComment("流水號");
            entity.Property(e => e.FriendUserId).HasComment("朋友ID");
            entity.Property(e => e.State).HasComment("邀請狀態");
            entity.Property(e => e.UserId).HasComment("使用者");

            entity.HasOne(d => d.FriendUser).WithMany(p => p.FriendFriendUsers)
                .HasForeignKey(d => d.FriendUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friend_Users1");

            entity.HasOne(d => d.User).WithMany(p => p.FriendUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friend_Users");
        });

        modelBuilder.Entity<HistoryName>(entity =>
        {
            entity.HasKey(e => e.HistoryId);

            entity.ToTable("HistoryName");

            entity.Property(e => e.HistoryId).HasComment("流水號");
            entity.Property(e => e.OldName)
                .HasMaxLength(50)
                .HasComment("歷史名稱");
            entity.Property(e => e.Time).HasComment("時間");
            entity.Property(e => e.UserId).HasComment("使用者ID");

            entity.HasOne(d => d.User).WithMany(p => p.HistoryNames)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistoryName_Users");
        });

        modelBuilder.Entity<LinePay>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LinePay__3214EC077A1D6777");

            entity.ToTable("LinePay");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CancelUrl).HasMaxLength(255);
            entity.Property(e => e.ConfirmUrl).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.OrderId).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TransactionId).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<MessageCard>(entity =>
        {
            entity.HasKey(e => e.MessageId);

            entity.ToTable("MessageCard");

            entity.Property(e => e.MessageId).HasComment("流水號");
            entity.Property(e => e.CommitText)
                .HasMaxLength(2000)
                .HasComment("聊天內容");
            entity.Property(e => e.CreateTime)
                .HasComment("發話時間")
                .HasColumnType("datetime")
                .HasColumnName("CreateTIme");
            entity.Property(e => e.SendByUsetId).HasComment("收文人");
            entity.Property(e => e.SendToUserId).HasComment("發送人ID");
            entity.Property(e => e.State).HasColumnName("state");

            entity.HasOne(d => d.SendToUser).WithMany(p => p.MessageCards)
                .HasForeignKey(d => d.SendToUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageCard_Users");
        });

        modelBuilder.Entity<Notify>(entity =>
        {
            entity.ToTable("Notify");

            entity.Property(e => e.NotifyId).HasComment("流水號");
            entity.Property(e => e.DateTime)
                .HasComment("時間")
                .HasColumnType("datetime");
            entity.Property(e => e.ImgUrl)
                .HasMaxLength(2000)
                .HasComment("圖片");
            entity.Property(e => e.ReadAlready).HasComment("0:未讀,1:已讀");
            entity.Property(e => e.Text)
                .HasMaxLength(1000)
                .HasComment("內文");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasComment("標題");
            entity.Property(e => e.UserId).HasComment("使用者ID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notify_Users");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasComment("流水號");
            entity.Property(e => e.Orderdate)
                .HasComment("購買日期")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentTypeId)
                .HasComment("金流分類")
                .HasColumnName("PaymentTypeID");
            entity.Property(e => e.State).HasComment("付款狀態(ex:成功?失敗?都幾)");
            entity.Property(e => e.TotalPrice)
                .HasComment("總額")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransactionType).HasComment("購買/儲值");
            entity.Property(e => e.UserId).HasComment("使用者ID");
            entity.Property(e => e.WalletChange)
                .HasComment("錢包改變")
                .HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Walletbalance)
                .HasComment("餘額")
                .HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Users");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasComment("流水號");
            entity.Property(e => e.About)
                .HasMaxLength(2000)
                .HasComment("或許會變成複文本");
            entity.Property(e => e.CategoryId).HasComment("類別ID");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasComment("描述");
            entity.Property(e => e.Event)
                .HasMaxLength(2000)
                .HasComment("或許會變成複文本");
            entity.Property(e => e.ProductMainImageUrl).HasComment("產品主圖片連結");
            entity.Property(e => e.ProductName)
                .HasMaxLength(50)
                .HasComment("產品名稱");
            entity.Property(e => e.ProductPrice)
                .HasComment("產品價格")
                .HasColumnType("money");
            entity.Property(e => e.ProductShelfTime).HasComment("發行日期");
            entity.Property(e => e.PublisherId)
                .HasComment("發行商ID")
                .HasColumnName("PublisherID");
            entity.Property(e => e.SystemRequirement).HasComment("或許會變成複文本");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Categories");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Products)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Publisher");
        });

        modelBuilder.Entity<ProductAdvertise>(entity =>
        {
            entity.HasKey(e => e.AdvertiseId);

            entity.ToTable("ProductAdvertise");

            entity.Property(e => e.AdvertiseImgUrl).HasMaxLength(1000);
        });

        modelBuilder.Entity<ProductCarousel>(entity =>
        {
            entity.HasKey(e => e.CarouselId);

            entity.ToTable("ProductCarousel", tb => tb.HasComment("0:照片，1:影片"));

            entity.Property(e => e.CarouselId).HasComment("流水號");
            entity.Property(e => e.CarouselUrl).HasComment("超連結");
            entity.Property(e => e.DataSourceUrl).HasComment("圖片導向超連結");
            entity.Property(e => e.ProductId).HasComment("產品ID");
            entity.Property(e => e.Type).HasComment("影片/圖片");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductCarousels)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCarousel_Product");
        });

        modelBuilder.Entity<ProductCollection>(entity =>
        {
            entity.HasKey(e => e.CollectionId);

            entity.ToTable("ProductCollection");

            entity.Property(e => e.CollectionId).HasComment("他不是遊戲庫id，他是流水號");
            entity.Property(e => e.ProductId).HasComment("產品ID");
            entity.Property(e => e.UserId).HasComment("使用者ID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductCollections)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCollection_Product");

            entity.HasOne(d => d.User).WithMany(p => p.ProductCollections)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCollection_Users");
        });

        modelBuilder.Entity<ProductComment>(entity =>
        {
            entity.HasKey(e => e.CommentsId);

            entity.Property(e => e.CommentsId).HasComment("流水號");
            entity.Property(e => e.Comment)
                .HasComment("好評/壞評")
                .HasColumnName("comment");
            entity.Property(e => e.ProductId).HasComment("產品ID");
            entity.Property(e => e.UserId).HasComment("玩家ID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductComments)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductComments_Product");

            entity.HasOne(d => d.User).WithMany(p => p.ProductComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductComments_Users");
        });

        modelBuilder.Entity<ProductPageAbout>(entity =>
        {
            entity.HasKey(e => e.AboutId);

            entity.Property(e => e.AboutId).HasComment("流水號");
            entity.Property(e => e.AboutMainTitle).HasComment("關於的標題");
            entity.Property(e => e.ProductId).HasComment("產品ID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPageAbouts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductPageAbouts_Product");
        });

        modelBuilder.Entity<ProductPageEvent>(entity =>
        {
            entity.HasKey(e => e.EventsId);

            entity.Property(e => e.EventsId).HasComment("流水號");
            entity.Property(e => e.AnnounceText)
                .HasMaxLength(2000)
                .HasComment("活動文本");
            entity.Property(e => e.AnnouncementDate).HasComment("活動時間");
            entity.Property(e => e.Content)
                .HasMaxLength(200)
                .HasComment("內文");
            entity.Property(e => e.ProductId).HasComment("產品名稱");
            entity.Property(e => e.ProductImgUrl).HasComment("活動照片");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasComment("標題");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPageEvents)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductPageEvents_Product");
        });

        modelBuilder.Entity<ProductsDiscount>(entity =>
        {
            entity.HasKey(e => e.DiscountId);

            entity.ToTable("ProductsDiscount");

            entity.Property(e => e.Discount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductsDiscounts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductsDiscount_Product");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.ToTable("Publisher");

            entity.Property(e => e.PublisherId).HasComment("流水號");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.ContactName).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.PublisherName)
                .HasMaxLength(100)
                .HasComment("發行商名稱");
        });

        modelBuilder.Entity<PurchaseHistoryDetail>(entity =>
        {
            entity.HasKey(e => e.PurchaseHistoryId);

            entity.ToTable("PurchaseHistoryDetail");

            entity.Property(e => e.PurchaseHistoryId).HasComment("流水號");
            entity.Property(e => e.Discount)
                .HasComment("折扣")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderId).HasComment("訂單ID");
            entity.Property(e => e.Price)
                .HasComment("單價")
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasComment("產品ID");
            entity.Property(e => e.ProductName)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasComment("產品名稱");
            entity.Property(e => e.SalesPrice)
                .HasComment("折扣後金額")
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.PurchaseHistoryDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseHistoryDetail_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseHistoryDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseHistoryDetail_Product");
        });

        modelBuilder.Entity<ShoppingCartCard>(entity =>
        {
            entity.HasKey(e => e.ShoppingCartId);

            entity.ToTable("ShoppingCartCard");

            entity.Property(e => e.ShoppingCartId).HasComment("流水號");
            entity.Property(e => e.ProductId).HasComment("產品名稱");
            entity.Property(e => e.UserId).HasComment("使用者ID");

            entity.HasOne(d => d.Product).WithMany(p => p.ShoppingCartCards)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ShoppingCartCard_Product1");

            entity.HasOne(d => d.User).WithMany(p => p.ShoppingCartCards)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ShoppingCartCard_Users1");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.TagId).HasComment("流水號");
            entity.Property(e => e.TagName)
                .HasMaxLength(50)
                .HasComment("標籤名稱");
        });

        modelBuilder.Entity<TagConnect>(entity =>
        {
            entity.HasKey(e => e.TagConnectId).HasName("PK_TagConnect_1");

            entity.ToTable("TagConnect");

            entity.Property(e => e.TagConnectId).HasComment("流水號");
            entity.Property(e => e.ProductId).HasComment("產品ID");
            entity.Property(e => e.TagId).HasComment("tag流水號");

            entity.HasOne(d => d.Product).WithMany(p => p.TagConnects)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TagConnect_Product");

            entity.HasOne(d => d.Tag).WithMany(p => p.TagConnects)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TagConnect_Tags");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasComment("流水號");
            entity.Property(e => e.Account)
                .HasMaxLength(50)
                .HasComment("帳號 只能唯一");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasComment("城市");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasComment("國家");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(255)
                .HasComment("電郵");
            entity.Property(e => e.IsLocked).HasComment("帳號是否被鎖住(例如被刪掉)0:鎖住;1:還能活動");
            entity.Property(e => e.NickName)
                .HasMaxLength(50)
                .HasComment("匿名");
            entity.Property(e => e.Online).HasComment("連線狀態 暫定");
            entity.Property(e => e.Overview)
                .HasMaxLength(1000)
                .HasComment("自介");
            entity.Property(e => e.Password).HasComment("密碼");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasComment("電話號碼");
            entity.Property(e => e.State).HasComment("會員驗證");
            entity.Property(e => e.UserImg).HasComment("大頭貼");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasComment("真實姓名");
            entity.Property(e => e.WalletAmount)
                .HasComment("錢包餘額")
                .HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<UserHeadShotCollection>(entity =>
        {
            entity.HasKey(e => e.HeadShotId).HasName("PK_UserHeadShots");

            entity.ToTable("UserHeadShotCollection");

            entity.HasOne(d => d.User).WithMany(p => p.UserHeadShotCollections)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserHeadShots_Users");
        });

        modelBuilder.Entity<VerifyMail>(entity =>
        {
            entity.HasKey(e => e.VerifyId);

            entity.ToTable("VerifyMail");

            entity.Property(e => e.EncodingParameter).HasColumnName("encodingParameter");
            entity.Property(e => e.Expired).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.VerifyMails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VerifyMail_Users");
        });

        modelBuilder.Entity<WishCard>(entity =>
        {
            entity.HasKey(e => e.WishId);

            entity.ToTable("WishCard");

            entity.Property(e => e.WishId).HasComment("流水號");
            entity.Property(e => e.AddDate)
                .HasComment("加入時間")
                .HasColumnType("datetime");
            entity.Property(e => e.ProductId).HasComment("商品ID");
            entity.Property(e => e.UserId)
                .HasComment("使用者ID")
                .HasColumnName("UserID");
            entity.Property(e => e.WishSortId).HasComment("自訂排序");

            entity.HasOne(d => d.Product).WithMany(p => p.WishCards)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WishCard_Product");

            entity.HasOne(d => d.User).WithMany(p => p.WishCards)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WishCard_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
