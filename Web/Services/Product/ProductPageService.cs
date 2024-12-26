using ApplicationCore.Dtos;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System;
using Web.ViewModels.Product;
using ProductPageAbout = Web.ViewModels.Product.ProductPageAbout;
using ProductPageEvent = Web.ViewModels.Product.ProductPageEvent;
using Tag = Web.ViewModels.Product.Tag;


namespace Web.Services.ProductNs
{
    public class ProductPageService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCarousel> _productCarouselRepository;
        private readonly IRepository<ProductComment> _productCommentRepository;
        private readonly IRepository<Publisher> _publisherRepository;
        private readonly IRepository<TagConnect> _tagConnectRepository;
        private readonly IRepository<ApplicationCore.Entities.Tag> _tagRepository;
        private readonly IRepository<ApplicationCore.Entities.ProductPageEvent> _productPageEventRepository;
        private readonly IRepository<ApplicationCore.Entities.ProductPageAbout> _productPageAboutRepository;
        private readonly IRepository<AboutCardList> _aboutCardListRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Friend> _friendRepository;
        private readonly IRepository<ProductCollection> _productCollectionRepository;
        private readonly IRepository<ProductsDiscount> _productsDiscountRepository;
        private readonly IRepository<ShoppingCartCard> _shoppingCartRepository;
        private readonly IRepository<WishCard> _wishCardRepository;
        private readonly IRepository<ProductCollection> _productCollection;
        private readonly IProductPageQueryService _productPageQueryService;
        private readonly StellarDBContext _stellarDBContext;

        public ProductPageService(IRepository<Product> productRepository, IRepository<Category> categoryRepository, IRepository<ProductCarousel> productCarouselRepository, IRepository<ProductComment> productCommentRepository, IRepository<Publisher> publisherRepository, IRepository<TagConnect> tagConnectRepository, IRepository<ApplicationCore.Entities.Tag> tagRepository, IRepository<ApplicationCore.Entities.ProductPageEvent> productPageEventRepository, IRepository<ApplicationCore.Entities.ProductPageAbout> productPageAboutRepository, IRepository<AboutCardList> aboutCardListRepository, IRepository<User> userRepository, IRepository<Friend> friendRepository, IRepository<ProductCollection> productCollectionRepository, IRepository<ShoppingCartCard> shoppingCartRepository, IRepository<WishCard> wishCardRepository, IRepository<ProductsDiscount> productsDiscountRepository, StellarDBContext stellarDBContext, IRepository<ProductCollection> productCollection, IProductPageQueryService productPageQueryService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productCarouselRepository = productCarouselRepository;
            _productCommentRepository = productCommentRepository;
            _publisherRepository = publisherRepository;
            _tagConnectRepository = tagConnectRepository;
            _tagRepository = tagRepository;
            _productPageEventRepository = productPageEventRepository;
            _productPageAboutRepository = productPageAboutRepository;
            _aboutCardListRepository = aboutCardListRepository;
            _userRepository = userRepository;
            _friendRepository = friendRepository;
            _productCollectionRepository = productCollectionRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _wishCardRepository = wishCardRepository;
            _productsDiscountRepository = productsDiscountRepository;
            _stellarDBContext = stellarDBContext;
            _productCollection = productCollection;
            _productPageQueryService = productPageQueryService;
        }

        //更新評論
        public async Task<(string commentsResult, string commentsClass)> GetComments(int productId)
        {
            var comments = await _productCommentRepository.ListAsync(comment => comment.ProductId == productId);
            var goodComments = comments.Count(comment => comment.Comment);
            var badComments = comments.Count(comment => !comment.Comment);

            double totalComments = goodComments + badComments;
            double percentageDifference = totalComments > 0 ? Math.Abs(goodComments - badComments) / totalComments : 0;

            string commentsResult;
            string commentsClass;

            if (percentageDifference < 0.1 && totalComments > 0)
            {
                commentsResult = $"褒貶不一 <span class='comment-count'>({totalComments})</span>";
                commentsClass = "mixed-comments";
            }
            else if (goodComments > badComments)
            {
                commentsResult = $"大多好評 <span class='comment-count'>({totalComments})</span>";
                commentsClass = "good-comments";
            }
            else if (badComments > goodComments)
            {
                commentsResult = $"大多負評 <span class='comment-count'>({totalComments})</span>";
                commentsClass = "bad-comments";
            }
            else
            {
                commentsResult = $"無使用者評論";
                commentsClass = "no-comments";
            }

            return (commentsResult, commentsClass);
        }

        //增加差評
        public async Task AddBadToComment(int productId, int userId)
        {
            // 檢查該產品是否已經存在使用者的差評
            var badCommentExist = await _productCommentRepository.FirstOrDefaultAsync(comment => comment.UserId == userId && comment.ProductId == productId && comment.Comment == false);

            var goodCommentExist = await _productCommentRepository.FirstOrDefaultAsync(comment => comment.UserId == userId && comment.ProductId == productId && comment.Comment == true);

            if (badCommentExist == null)
            {
                // 建立新的差評
                var commentListItem = new ProductComment
                {
                    UserId = userId,
                    ProductId = productId,
                    Comment = false,
                };

                // 將新的差評加入資料庫
                await _productCommentRepository.AddAsync(commentListItem);
            }
        }

        //刪除差評
        public async Task RemoveBadToComment(int productId, int userId)
        {
            // 檢查該產品是否已經存在使用者的差評
            var existingCommentItem = await _productCommentRepository.FirstOrDefaultAsync(comment => comment.UserId == userId && comment.ProductId == productId && comment.Comment == false);

            if (existingCommentItem != null)
            {
                // 將新的差評移除資料庫
                await _productCommentRepository.DeleteAsync(existingCommentItem);
            }
        }

        //增加好評
        public async Task AddGoodToComment(int productId, int userId)
        {
            // 檢查該產品是否已經存在使用者的好評
            var goodCommentExist = await _productCommentRepository.FirstOrDefaultAsync(comment => comment.UserId == userId && comment.ProductId == productId && comment.Comment == true);

            var badCommentExist = await _productCommentRepository.FirstOrDefaultAsync(comment => comment.UserId == userId && comment.ProductId == productId && comment.Comment == false);

            if (goodCommentExist == null)
            {
                // 建立新的好評
                var commentListItem = new ProductComment
                {
                    UserId = userId,
                    ProductId = productId,
                    Comment = true,
                };

                // 將新的好評加入資料庫
                await _productCommentRepository.AddAsync(commentListItem);
            }
        }

        //刪除好評
        public async Task RemoveGoodToComment(int productId, int userId)
        {
            // 檢查該產品是否已經存在使用者的好評
            var existingCommentItem = await _productCommentRepository.FirstOrDefaultAsync(comment => comment.UserId == userId && comment.ProductId == productId && comment.Comment == true);

            if (existingCommentItem != null)
            {
                // 將新的好評移除資料庫
                await _productCommentRepository.DeleteAsync(existingCommentItem);
            }
        }




        //加入收藏庫(免費遊戲)
        public async Task AddToCollection(int productId, int userId)
        {
            // 檢查該產品是否已經在使用者的收藏庫內
            var existingCollection = await _productCollectionRepository.FirstOrDefaultAsync(pc => pc.UserId == userId && pc.ProductId == productId);

            if (existingCollection == null)
            {
                // 建立新的收藏庫項目
                var collectionItem = new ProductCollection
                {
                    UserId = userId,
                    ProductId = productId
                };

                // 將新的收藏庫項目加入資料庫
                await _productCollectionRepository.AddAsync(collectionItem);
            }
        }

        //加入願望清單
        public async Task AddToWishList(int productId, int userId)
        {
            Console.WriteLine(productId);
            Console.WriteLine(userId);
            // 檢查該產品是否已經在使用者的願望清單內
            var existingWishItem = await _wishCardRepository.FirstOrDefaultAsync(wc => wc.UserId == userId && wc.ProductId == productId);

            if (existingWishItem == null)
            {
                // 獲取當前使用者願望清單中最大的 WishSortId
                int maxWishSortId = _wishCardRepository.List(wc => wc.UserId == userId)
                    .Max(wc => (int?)wc.WishSortId) ?? 0;

                // 建立新的願望清單項目
                var wishListItem = new WishCard
                {
                    UserId = userId,
                    ProductId = productId,
                    AddDate = DateTime.Now,
                    WishSortId = maxWishSortId + 1
                };

                // 將新的願望清單項目加入資料庫
                await _wishCardRepository.AddAsync(wishListItem);
            }
        }
        //刪除願望清單
        public async Task RemoveFromWishList(int productId, int userId)
        {
            var wishListItem = await _wishCardRepository.FirstOrDefaultAsync(wc => wc.UserId == userId && wc.ProductId == productId);

            if (wishListItem != null)
            {
                await _wishCardRepository.DeleteAsync(wishListItem);
            }

        }

        //加入購物車
        public async Task AddToCart(int productId, int userId)
        {
            // 檢查該產品是否已經在使用者的購物車內
            var existingCartItem = await _shoppingCartRepository.FirstOrDefaultAsync(sc => sc.UserId == userId && sc.ProductId == productId);
            // 檢查該產品是否已經在使用者的收藏庫內
            var existingCollection = await _productCollectionRepository.FirstOrDefaultAsync(pc => pc.UserId == userId && pc.ProductId == productId);

            if (existingCartItem == null && existingCollection == null)
            {

                // 建立新的購物車項目
                var shoppingCartItem = new ShoppingCartCard
                {
                    UserId = userId,
                    ProductId = productId
                };

                // 將新的購物車項目加入資料庫
                await _shoppingCartRepository.AddAsync(shoppingCartItem);
            }
        }


        public async Task<ProductPageViewModel> GetProductPageServiceData(int currentProductId, int userId)
        {
            var product = await _productRepository.FirstOrDefaultAsync(x => x.ProductId == currentProductId);

            // 如果產品不存在或者狀態不為1，則返回null
            if (product == null || product.ProductStatus != 1)
            {
                return null;
            }

            // 獲取當前產品的折扣
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            //正在打折的產品
            var currentProductDiscounts = await _productsDiscountRepository.ListAsync(x => x.ProductId == currentProductId
                        && x.SalesStartDate <= currentDate
                        && x.SalesEndDate >= currentDate);

            var currentDiscount = currentProductDiscounts.Any() ? currentProductDiscounts.Min(x => x.Discount) : 1m;

            //類別
            var category = await _categoryRepository.FirstOrDefaultAsync(x => x.CategoryId == product.CategoryId);

            //輪播牆
            var carousels = (await _productCarouselRepository.ListAsync(c => c.ProductId == product.ProductId)).OrderBy(c => c.Type);

            var carouselViewModels = carousels.Select(c => new Carousel
            {
                Id = c.CarouselId,
                ImgUrl = c.CarouselUrl,
                DataSrcUrl = c.DataSourceUrl,
            }).ToList();

            //評論
            var comments = await _productCommentRepository.ListAsync(comment => comment.ProductId == product.ProductId);
            var goodComments = comments.Count(comment => comment.Comment);
            var badComments = comments.Count(comment => !comment.Comment);

            //好+差評
            double totalComments = goodComments + badComments;

            double percentageDifference = Math.Abs(goodComments - badComments) / totalComments;

            string commentsResult;
            string commentsClass;

            if (percentageDifference < 0.1 && totalComments > 0)
            {
                commentsResult = "褒貶不一";
                commentsClass = "mixed-comments";
            }
            else if (goodComments > badComments)
            {
                commentsResult = "大多好評";
                commentsClass = "good-comments";
            }
            else if (badComments > goodComments)
            {
                commentsResult = "大多負評";
                commentsClass = "bad-comments";
            }
            else
            {
                commentsResult = "無使用者評論";
                commentsClass = "no-comments";
            }

            //發行商
            var publishers = await _publisherRepository.FirstOrDefaultAsync(pub => pub.PublisherId == product.PublisherId);

            //標籤
            //Dapper
            var getTagsResult = await _productPageQueryService.GetTagsInProductPage(currentProductId);
            var tags = getTagsResult.Select(t => new Tag
            {
                Id = t.Id,
                Name = t.Name,
            }).ToList();


            //好友

            //Dapper
            var friendsWhoOwnThisGameResult = await _productPageQueryService.GetFriendsWhoOwnThisGameInProductPage(currentProductId, userId);

            var friendsWhoOwnThisGame = friendsWhoOwnThisGameResult
               .Select(f => new ProductPageFriend
               {
                   Id = f.Id,
                   ImgUrl = f.ImgUrl,
                   Online = f.Online,
                   FriendName = f.FriendName
               }).ToList();

            var CountFriends = friendsWhoOwnThisGame.Count();


            //近期活動和公告
            var productPageEvents = await _productPageEventRepository.ListAsync(productEvent => productEvent.ProductId == product.ProductId);

            var productPageEventsViewModels = productPageEvents.Select(ppe => new ProductPageEvent
            {
                Id = ppe.EventsId,
                ImgUrl = ppe.ProductImgUrl!,
                Title = ppe.Title,
                Content = ppe.Content,
                AnnounceText = ppe.AnnounceText,
                AnnounceTime = ppe.AnnouncementDate,
                FormattedAnnounceTime = $"{ppe.AnnouncementDate.Year} 年 {ppe.AnnouncementDate.Month} 月 {ppe.AnnouncementDate.Day} 日",
            }).Where(x => x.AnnounceTime <= currentDate).OrderByDescending(x => x.AnnounceTime).Take(2).ToList();

            //關於此遊戲
            var productPageAbouts = await _productPageAboutRepository.ListAsync(productAbout => productAbout.ProductId == product.ProductId);

            var productPageAboutsViewModels = productPageAbouts.Select(ppa => new ProductPageAbout
            {
                Id = ppa.AboutId,
                AboutMainTitle = ppa.AboutMainTitle,
                AboutCardList = _aboutCardListRepository.List(aboutCardList => aboutCardList.AboutId == ppa.AboutId)
              .Select(acl => new AboutCard
              {
                  ImgUrl = acl.ImageUrl,
                  Title = acl.Title,
                  Text = acl.Text,

              }).ToList()

            }).FirstOrDefault();

            // 初始化
            if (productPageAboutsViewModels == null)
            {
                productPageAboutsViewModels = new ProductPageAbout
                {
                    AboutCardList = new List<AboutCard>()
                };
            }

            //更多相似的產品
            var similarProducts = await _productRepository.ListAsync(p => p.CategoryId == product.CategoryId && p.ProductId != currentProductId && p.ProductStatus == 1);

            var similarProductViewModels = similarProducts
                .Select(sp => new SimilarProduct
                {
                    Id = sp.ProductId,
                    ImgUrl = sp.ProductMainImageUrl,
                    Title = sp.ProductName,
                    Price = sp.ProductPrice,
                    FormattedPrice = sp.ProductPrice == 0 ? "免費遊玩" : $"NT$ {Math.Round(sp.ProductPrice):N0}"
                }).ToList();



            var productPageData = new ProductPageViewModel()
            {
                ProductId = product.ProductId,
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Name = product.ProductName,
                CommentsClass = commentsClass,
                ProductPageMainArea = new ProductPageMainArea
                {
                    Id = product.ProductId,
                    CarouselUrlList = carouselViewModels,

                    MainImg = product.ProductMainImageUrl,

                    Description = product.Description,

                    Comment = commentsResult,

                    TotalComment = totalComments,

                    ShelfTime = product.ProductShelfTime,

                    FormattedShelfTime = $"{product.ProductShelfTime.Year} 年 {product.ProductShelfTime.Month} 月 {product.ProductShelfTime.Day} 日",

                    Publisher = publishers.PublisherName,

                    TagList = tags,
                },

                Price = product.ProductPrice,

                FormattedPrice = $"NT$ {Math.Round(product.ProductPrice):N0}",

                Discount = (1 - currentDiscount) * 100,

                FormattedDiscount = $"-{(1 - currentDiscount) * 100:0}%",
                FormattedDiscountForTitle = $"{(1 - currentDiscount) * 100:0}%",

                ProductPageFriendList = friendsWhoOwnThisGame.ToList(),

                ProductPageLanguageList = new List<ProductPageLanguage>
                {
                    new ProductPageLanguage { Id = 1, Name = "繁體中文", InterfaceSupport = true, FullVoiceSupport = true, SubtitlesSupport = true },
                    new ProductPageLanguage { Id = 2, Name = "英文", InterfaceSupport = true, FullVoiceSupport = false, SubtitlesSupport = true },
                    new ProductPageLanguage { Id = 3, Name = "法文", InterfaceSupport = true, FullVoiceSupport = false, SubtitlesSupport = true },
                    new ProductPageLanguage { Id = 4, Name = "德文", InterfaceSupport = true, FullVoiceSupport = false, SubtitlesSupport = true },
                    new ProductPageLanguage { Id = 5, Name = "西班牙文", InterfaceSupport = true, FullVoiceSupport = false, SubtitlesSupport = true },
                    new ProductPageLanguage { Id = 6, Name = "日文", InterfaceSupport = true, FullVoiceSupport = false, SubtitlesSupport = true },
                    new ProductPageLanguage { Id = 7, Name = "韓文", InterfaceSupport = true, FullVoiceSupport = false, SubtitlesSupport = true },
                    new ProductPageLanguage { Id = 8, Name = "波蘭文", InterfaceSupport = true, FullVoiceSupport = false, SubtitlesSupport = true },
                    new ProductPageLanguage { Id = 9, Name = "葡萄牙文 - 巴西", InterfaceSupport = true, FullVoiceSupport = false, SubtitlesSupport = true },
                    new ProductPageLanguage { Id = 10, Name = "俄文", InterfaceSupport = true, FullVoiceSupport = false, SubtitlesSupport = false },
                    new ProductPageLanguage { Id = 11, Name = "簡體中文", InterfaceSupport = true, FullVoiceSupport = false, SubtitlesSupport = false },
                    new ProductPageLanguage { Id = 12, Name = "義大利文", InterfaceSupport = true, FullVoiceSupport = false, SubtitlesSupport = false },
                },

                ProductPageEventList = productPageEventsViewModels,

                ProductPageAbout = productPageAboutsViewModels,

                SystemRequirements = product.SystemRequirement,

                SimilarProductList = similarProductViewModels

            };

            //Price轉型
            productPageData.SalesPrice = productPageData.Price * (1 - productPageData.Discount / 100);
            productPageData.FormattedSalesPrice = $"NT$ {productPageData.SalesPrice:N0}";

            //朋友數
            productPageData.CountFriends = CountFriends;

            //判斷是否有Event或About
            productPageData.IsEventExist = productPageEventsViewModels.Any();
            productPageData.IsAboutExist = productPageAboutsViewModels != null && productPageAboutsViewModels.AboutCardList.Any();

            //購買選擇
            productPageData.IsDiscountGame = currentDiscount >= 0 && currentDiscount < 1;
            productPageData.IsFreeGame = product.ProductPrice == 0;
            productPageData.IsOriginalPrice = currentDiscount == 1 && product.ProductPrice > 0;

            //判斷是否有在願望清單
            productPageData.IsInWishList = await _wishCardRepository.AnyAsync(wc => wc.UserId == userId && wc.ProductId == currentProductId);

            //判斷是否有在收藏庫
            productPageData.IsInCollectionList = await _productCollection.AnyAsync(pc => pc.UserId == userId && pc.ProductId == currentProductId);

            //判斷是否有在購物車
            productPageData.IsInCartList = await _shoppingCartRepository.AnyAsync(sc => sc.UserId == userId && sc.ProductId == currentProductId);

            //判斷是否為即將發行
            productPageData.IsInComingSoonList = currentDate < product.ProductShelfTime;

            //判斷該user是否已經對產品打過好評
            productPageData.IsGoodCommentAlready = await _productCommentRepository.AnyAsync(comment => comment.UserId == userId && comment.ProductId == currentProductId && comment.Comment == true);

            productPageData.IsBadCommentAlready = await _productCommentRepository.AnyAsync(comment => comment.UserId == userId && comment.ProductId == currentProductId && comment.Comment == false);

            return productPageData;
        }

    }


}
