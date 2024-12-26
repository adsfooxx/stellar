using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using FluentEcpay;
using MailKit.Search;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Web.ViewModels.Member;
using Web.ViewModels.Partial;

namespace Web.Services.Member
{
    public class WishListServices
    {
        private readonly IRepository<WishCard> _wishCardRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<TagConnect> _tagConnectRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<ProductComment> _productCommentRepository;
        private readonly IRepository<ProductCarousel> _productCarouselRepository;
        private readonly IRepository<ProductsDiscount> _productsDiscountRepository;
        private readonly IRepository<ShoppingCartCard> _shoppingCartRepository;
        private readonly IRepository<ProductCollection> _productCollectionRepository;

        public WishListServices(
            IRepository<WishCard> wishCardRepository,
            IRepository<User> userRepository,
            IRepository<Product> productRepository,
            IRepository<TagConnect> tagConnectRepository,
            IRepository<Tag> tagRepository,
            IRepository<ProductComment> productCommentRepository,
            IRepository<ProductCarousel> productCarouselRepository,
            IRepository<ProductsDiscount> productsDiscountRepository,
            IRepository<ShoppingCartCard> shoppingCartRepository,
            IRepository<ProductCollection> productCollectionRepository)
        {
            _wishCardRepository = wishCardRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _tagConnectRepository = tagConnectRepository;
            _tagRepository = tagRepository;
            _productCommentRepository = productCommentRepository;
            _productCarouselRepository = productCarouselRepository;
            _productsDiscountRepository = productsDiscountRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _productCollectionRepository = productCollectionRepository;
        }

        // 移除產品自願望清單
        public void RemoveItemFromWishList(int wishListItemId)
        {
            var wishListItem = _wishCardRepository.FirstOrDefault(wc => wc.WishId == wishListItemId);
            if (wishListItem != null)
            {
                _wishCardRepository.Delete(wishListItem);
            }
        }

        // 加入購物車
        public async Task AddToCart(int productId, int userId)
        {
            var existingCartItem = _shoppingCartRepository.FirstOrDefault(sc => sc.UserId == userId && sc.ProductId == productId);
            if (existingCartItem == null)
            {
                var shoppingCartItem = new ShoppingCartCard
                {
                    UserId = userId,
                    ProductId = productId
                };
                await _shoppingCartRepository.AddAsync(shoppingCartItem);
            }
        }

        // 加入收藏庫
        public void AddToCollection(int productId, int userId)
        {
            var existingCollection = _productCollectionRepository.FirstOrDefault(pc => pc.UserId == userId && pc.ProductId == productId);
            if (existingCollection == null)
            {
                var collectionItem = new ProductCollection
                {
                    UserId = userId,
                    ProductId = productId
                };
                var wishListItem = _wishCardRepository.FirstOrDefault(wc => wc.ProductId == productId);
                _productCollectionRepository.Add(collectionItem);
                if (wishListItem != null)
                {
                    _wishCardRepository.Delete(wishListItem);
                }
            }
        }

        // 獲取願望清單資料
        public async Task<WishListViewModel> GetWishListData(int userId, string searchTerm = null)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                return new WishListViewModel
                {
                    NoResultMessage = "找不到使用者資料"
                };
            }

            // 取得標籤
            var gametags = (from w in _wishCardRepository.List(x => x.UserId == userId)
                            join tc in _tagConnectRepository.List()
                            on w.ProductId equals tc.ProductId
                            join t in _tagRepository.List()
                            on tc.TagId equals t.TagId
                            orderby w.WishSortId
                            select new Tags
                            {
                                tagProductId = w.ProductId,
                                tagId = t.TagId,
                                tagName = t.TagName,
                            }).ToList();

            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var everyProductDiscount = (from w in _wishCardRepository.List()
                                        join p in _productRepository.List()
                                        on w.ProductId equals p.ProductId
                                        join pd in _productsDiscountRepository.List()
                                        on w.ProductId equals pd.ProductId into productDiscounts
                                        from pd in productDiscounts.DefaultIfEmpty()
                                        select new GameDiscount
                                        {
                                            discountProductId = w.ProductId,
                                            OringinalPrice = p.ProductPrice,
                                            OringinalPriceFormat = p.ProductPrice.ToString("#,###"),
                                            Discount = pd == null ? 0 : (1 - pd.Discount) * 100,
                                            DiscountFormat = pd == null ? "-1" : ((1 - pd.Discount) * 100).ToString("##"),
                                            SalesPrice = pd == null ? p.ProductPrice : Math.Round(p.ProductPrice * pd.Discount),
                                            SalesPriceFormat = pd == null
                                                ? p.ProductPrice.ToString("#,###")
                                                : Math.Round(p.ProductPrice * pd.Discount).ToString("#,###"),
                                            IsInDiscountTime = pd != null && (pd.SalesStartDate <= currentDate && pd.SalesEndDate >= currentDate)
                                        }).ToList();

            var WishCardProductComments = (from p in _productRepository.List()
                                           join pc in _productCommentRepository.List()
                                           on p.ProductId equals pc.ProductId into productComments
                                           from pcts in productComments.DefaultIfEmpty()
                                           group pcts by p.ProductId into groupbyproductid
                                           select new GameComment
                                           {
                                               commentProductId = groupbyproductid.Key,
                                               goodComments = groupbyproductid.Count(c => c != null && c.Comment),
                                               badComments = groupbyproductid.Count(c => c != null && !c.Comment),
                                               percentageDifference = groupbyproductid.Count() > 0
                                                   ? Math.Abs((groupbyproductid.Count(c => c != null && c.Comment) - groupbyproductid.Count(c => c != null && !c.Comment))) / groupbyproductid.Count()
                                                   : 0
                                           }).ToList();

            // 設定願望商品查詢
            var wishListQuery = from w in _wishCardRepository.List(x => x.UserId == userId)
                                join p in _productRepository.List(x => x.ProductStatus == 1)
                                on w.ProductId equals p.ProductId
                                join pc in _productCollectionRepository.List()
                                on new { w.UserId, w.ProductId } equals new { pc.UserId, pc.ProductId } into productCollections
                                from pc in productCollections.DefaultIfEmpty()
                                where pc == null // 排除收藏庫中的產品
                                join sc in _shoppingCartRepository.List(x => x.UserId == userId)
                                on p.ProductId equals sc.ProductId into shoppingCarts
                                from sc in shoppingCarts.DefaultIfEmpty()
                                join pd in _productsDiscountRepository.List()
                                on w.ProductId equals pd.ProductId into productDiscounts
                                from pd in productDiscounts.DefaultIfEmpty()
                                select new WishProductCard
                                {
                                    SortID = w.WishSortId,
                                    WishItemID = w.WishId,
                                    ImgUrl = p.ProductMainImageUrl,
                                    Name = p.ProductName,
                                    Comment = WishCardProductComments.FirstOrDefault(x => x.commentProductId == w.ProductId),
                                    ReleaseDate = p.ProductShelfTime,
                                    GameLabel = gametags.Where(x => x.tagProductId == p.ProductId).ToList(),
                                    AddDate = w.AddDate,
                                    DirectUrl = "/product/productpage/" + p.ProductId,
                                    ProductId = p.ProductId,
                                    DiscounAndSalesprice = everyProductDiscount.FirstOrDefault(x => x.discountProductId == w.ProductId),
                                    IsInCartList = _shoppingCartRepository.Any(sc => sc.UserId == w.UserId && sc.ProductId == w.ProductId)
                                };

            // 如果提供了搜尋字串，則根據名稱和標籤進行篩選
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                wishListQuery = wishListQuery.Where(w =>
                    w.Name.Contains(searchTerm) ||
                    w.GameLabel.Any(tag => tag.tagName.Contains(searchTerm)));
            }

            var wishListItems = wishListQuery.ToList();

            // 無論願望商品清單是否為空，皆傳回使用者資料
            return new WishListViewModel
            {
                UserId = user.UserId,
                UserImgUrl = user.UserImg,
                UserNickName = user.NickName,
                SortType = 2,
                WishListItem = wishListItems.DistinctBy(x => x.ProductId).ToList(),  // 去除重複
                NoResultMessage = wishListItems.Count == 0 ? "無相關結果" : null // 顯示無結果時的提示訊息
            };
        }
    }
}
