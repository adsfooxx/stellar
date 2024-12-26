using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.LinePay.Dtos.Request;
using CloudinaryDotNet;
using Dapper;
using Infrastructure.Data.Mongo.Entity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stellar.API.Dtos.Products;
using System.Collections.Generic;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Stellar.API.Service.Product
{
#nullable disable
    public class ProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _connectionString;
        private readonly IRepository<ApplicationCore.Entities.Product> _productRepository;
        private readonly IRepository<ProductComment> _productCommentRepository;
        private readonly IRepository<ApplicationCore.Entities.Publisher> _publisherRepository;
        private readonly IRepository<ProductsDiscount> _productsDiscountRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<ProductPageEvent> _productPageEventRepository;
        private readonly IRepository<ProductPageAbout> _productPageAboutRepository;
        private readonly IRepository<AboutCardList> _aboutCardListRepository;
        private readonly IRepository<ProductCarousel> _productCarouselRepository;
        private readonly CloudinaryService _cloudinaryService;

        public ProductService(IUnitOfWork unitOfWork, IConfiguration configuration, IRepository<ApplicationCore.Entities.Product> productRepository, IRepository<ProductsDiscount> productsDiscountRepository, IRepository<Category> categoryRepository, IRepository<Tag> tagRepository, CloudinaryService cloudinaryService, IRepository<ProductComment> productCommentRepository, IRepository<ApplicationCore.Entities.Publisher> publisherRepository, IRepository<ProductPageEvent> productPageEventRepository, IRepository<ProductPageAbout> productPageAboutRepository, IRepository<AboutCardList> aboutCardListRepository, IRepository<ProductCarousel> productCarouselRepository)
        {
            _unitOfWork = unitOfWork;
            _connectionString = configuration.GetConnectionString("StellarDB") ?? throw new ArgumentNullException("找不到連線字串");
            _productRepository = productRepository;
            _productsDiscountRepository = productsDiscountRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _cloudinaryService = cloudinaryService;
            _productCommentRepository = productCommentRepository;
            _publisherRepository = publisherRepository;
            _productPageEventRepository = productPageEventRepository;
            _productPageAboutRepository = productPageAboutRepository;
            _aboutCardListRepository = aboutCardListRepository;
            _productCarouselRepository = productCarouselRepository;
        }

        /*抓取產品用*/
        public async Task<List<AllProductDto>> GetProducts()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var products = await _productRepository.ListAsync();

            // 一次性獲取所有產品對應的折扣
            var allProductDiscounts = await _productsDiscountRepository.ListAsync(pd =>
                pd.SalesStartDate <= currentDate && pd.SalesEndDate >= currentDate);

            // 一次性獲取所有產品的評論
            var allComments = await _productCommentRepository.ListAsync();

            // 一次性獲取所有公告
            var allAnnouncements = await _productPageEventRepository.ListAsync();

            // 一次性獲取所有分類
            var allCategories = await _categoryRepository.ListAsync();

            // 一次性獲取所有發行商
            var allPublishers = await _publisherRepository.ListAsync();

            var productDtos = new List<AllProductDto>();

            foreach (var p in products)
            {
                var formattedPrice = $"NT$ {Math.Round(p.ProductPrice):N0}";

                // 找到對應產品的折扣
                var currentProductDiscounts = allProductDiscounts.Where(pd => pd.ProductId == p.ProductId);
                var currentDiscount = currentProductDiscounts.Any() ? currentProductDiscounts.Min(x => x.Discount) : 1m;
                var formattedDiscount = currentDiscount < 1 ? $"- {(1 - currentDiscount) * 100:0}%" : "";

                var productSalesPrice = (p.ProductPrice * currentDiscount);

                var productSellStatus = currentDate >= p.ProductShelfTime;

                // 找到對應產品的評論
                var comments = allComments.Where(comment => comment.ProductId == p.ProductId).ToList();
                var goodComments = comments.Count(comment => comment.Comment);
                var badComments = comments.Count(comment => !comment.Comment);
                var totalComments = goodComments + badComments;

                double goodCommentsPercentage = 0;
                double badCommentsPercentage = 0;
                if (totalComments > 0)
                {
                    goodCommentsPercentage = Math.Round((double)goodComments / totalComments * 100, 0);
                    badCommentsPercentage = Math.Round((double)badComments / totalComments * 100, 0);
                }

                // 找到對應產品的公告
                var announcements = allAnnouncements.Where(x => x.ProductId == p.ProductId).ToList();

                // 找到對應的分類和發行商
                var category = allCategories.FirstOrDefault(x => x.CategoryId == p.CategoryId);
                var publisher = allPublishers.FirstOrDefault(x => x.PublisherId == p.PublisherId);

                productDtos.Add(new AllProductDto
                {
                    ProductId = p.ProductId,
                    ProductMainImageUrl = p.ProductMainImageUrl,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductSalesPrice = productSalesPrice,
                    ProductMainDescription = p.Description,
                    ProductDiscount = formattedDiscount,
                    ProductShelfTime = p.ProductShelfTime,
                    ProductSellStatus = productSellStatus,
                    ProductStatus = p.ProductStatus,
                    ProductGoodCommentPercentage = goodCommentsPercentage,
                    ProductBadCommentPercentage = badCommentsPercentage,
                    ProductCategory = category?.CategoryName,
                    ProductPublisher = publisher?.PublisherName,
                    ProductAnnouncements = announcements.Select(a => new ProductAnnouncement
                    {
                        EventsId = a.EventsId,
                        Title = a.Title,
                        AnnounceText = a.AnnounceText,
                        Content = a.Content,
                        ProductImgUrl = a.ProductImgUrl,
                        AnnouncementDate = a.AnnouncementDate
                    }).ToList()
                });
            }

            return productDtos;
        }

        /*新增產品*/
        public async Task AddProduct([FromForm] CreateProductRequestDto request)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {



                    var mainImageUrl = _cloudinaryService.UploadImage(request.ProductDto.MainImgFile);

                    var productSql = @"
                        INSERT INTO Product (ProductName, ProductPrice, ProductShelfTime, ProductMainImageUrl, Description, PublisherID, SystemRequirement, CategoryId, ProductStatus)
                        VALUES (@ProductName, @ProductPrice, @ProductShelfTime, @ProductMainImageUrl, @Description, @PublisherID, @SystemRequirement, @CategoryId, @ProductStatus);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                    var productId = await connection.QuerySingleAsync<int>(productSql, new
                    {
                        request.ProductDto.ProductName,
                        request.ProductDto.ProductPrice,
                        request.ProductDto.ProductShelfTime,
                        ProductMainImageUrl = mainImageUrl,
                        request.ProductDto.Description,
                        request.ProductDto.PublisherID,
                        request.ProductDto.SystemRequirement,
                        request.ProductDto.CategoryId,
                        ProductStatus = 0
                    }, transaction);

                    foreach (var carouselDto in request.CarouselDtos)
                    {
                        var carouselImageUrl = _cloudinaryService.UploadImage(carouselDto.CarouselImages);

                        var carouselSql = @"
                        INSERT INTO ProductCarousel (ProductId, CarouselUrl, DataSourceUrl, Type)
                        VALUES (@ProductId, @CarouselUrl, @DataSourceUrl, @Type);";

                        await connection.ExecuteAsync(carouselSql, new
                        {
                            ProductId = productId,
                            CarouselUrl = carouselImageUrl,
                            DataSourceUrl = carouselImageUrl,
                            Type = carouselDto.Sort + 1
                        }, transaction);
                    }

                    if (request.TagIds != null)
                    {
                        foreach (var tagDto in request.TagIds)
                        {
                            var tagSql = @"
                        INSERT INTO TagConnect (ProductId, TagId)
                        VALUES (@ProductId, @TagId);";

                            await connection.ExecuteAsync(tagSql, new
                            {
                                ProductId = productId,
                                TagId = tagDto
                            }, transaction);
                        }
                    }

                    if (request.EventDtos != null)
                    {
                        foreach (var eventDto in request.EventDtos)
                        {
                            var eventImageUrl = _cloudinaryService.UploadImage(eventDto.ProductImgFile);

                            var eventSql = @"
                        INSERT INTO ProductPageEvents (ProductId, Title, AnnounceText, Content, ProductImgUrl, AnnouncementDate)
                        VALUES (@ProductId, @Title, @AnnounceText, @Content, @ProductImgUrl, @AnnouncementDate);";

                            await connection.ExecuteAsync(eventSql, new
                            {
                                ProductId = productId,
                                Title = eventDto.Title,
                                AnnounceText = eventDto.AnnounceText,
                                Content = eventDto.Content,
                                ProductImgUrl = eventImageUrl,
                                AnnouncementDate = eventDto.AnnouncementDate
                            }, transaction);
                        }
                    }

                    var aboutSql = @"
                    INSERT INTO ProductPageAbouts (ProductId, AboutMainTitle)
                    VALUES (@ProductId, @AboutMainTitle);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                    var aboutId = await connection.QuerySingleAsync<int>(aboutSql, new
                    {
                        ProductId = productId,
                        AboutMainTitle = request.AboutDto.AboutMainTitle
                    }, transaction);

                    if (request.AboutCardListDtos != null)
                    {
                        foreach (var cardDto in request.AboutCardListDtos)
                        {
                            var cardImageUrl = _cloudinaryService.UploadImage(cardDto.AboutCardImgFile);

                            var cardSql = @"
                        INSERT INTO AboutCardLists (AboutId, ImageUrl, Title, Text)
                        VALUES (@AboutId, @ImageUrl, @Title, @Text);";

                            await connection.ExecuteAsync(cardSql, new
                            {
                                AboutId = aboutId,
                                ImageUrl = cardImageUrl,
                                Title = cardDto.Title,
                                Text = cardDto.Text
                            }, transaction);
                        }
                    }

                    transaction.Commit();
                }

                catch
                {
                    transaction.Rollback();

                }
                finally
                {
                    transaction.Dispose();
                }
            }
        }

        /*編輯產品主圖*/
        public async Task UpdateMainImage(UpdateMainImgDto UpdateMainImgDto)
        {
            var MainImageUrl = _cloudinaryService.UploadImage(UpdateMainImgDto.mainImg);
            var productData = await _productRepository.GetByIdAsync(UpdateMainImgDto.productId);
            productData.ProductMainImageUrl = MainImageUrl;
            await _productRepository.UpdateAsync(productData);
        }

        /*編輯產品*/
        public async Task UpdateProduct(EditProductDto product)
        {
            var productInDB = await _productRepository.GetByIdAsync(product.ProductId);

            if (productInDB != null)
            {
                productInDB.ProductName = product.ProductName;
                productInDB.ProductPrice = product.ProductPrice;
                productInDB.Description = product.ProductMainDescription;

                await _productRepository.UpdateAsync(productInDB);
            }
        }

        /*產品折扣區*/
        //刪除折扣活動
        public async Task RemoveSelectedProductDiscount(DiscountDeleteDto deleteDiscountData)
        {
            var formatedDiscountValue = (100 - deleteDiscountData.DiscountValue) / 100;

            // 查詢資料庫中與該產品 ID 和折扣日期匹配的折扣活動
            var discountsToRemove = await _productsDiscountRepository.ListAsync(pd =>
                pd.ProductId == deleteDiscountData.ProductId &&
                pd.SalesStartDate == deleteDiscountData.DiscountStartDate &&
                pd.SalesEndDate == deleteDiscountData.DiscountEndDate &&
                pd.Discount == formatedDiscountValue);

            if (discountsToRemove.Any())
            {
                await _productsDiscountRepository.DeleteRangeAsync(discountsToRemove);
            }
        }
        //制定折扣
        public async Task UpdateProductDiscount(List<DiscountCreateDto> discountData)
        {
            // 批量查詢需要更新折扣表的產品
            var discountsToAdd = new List<ProductsDiscount>();

            foreach (var discount in discountData)
            {
                decimal formatedDiscountValue = (100 - discount.DiscountValue) / 100;

                var newDiscount = new ProductsDiscount
                {
                    ProductId = discount.ProductId,
                    SalesStartDate = discount.DiscountStartDate,
                    SalesEndDate = discount.DiscountEndDate,
                    Discount = formatedDiscountValue
                };

                discountsToAdd.Add(newDiscount);
            }

            // 新增折扣資料到資料庫
            await _productsDiscountRepository.AddRangeAsync(discountsToAdd);
        }
        //讀取被選產品折扣
        public async Task<List<DiscountReadDto>> GetSelectedProductDiscount(List<int> productIds)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            //只印出活動當中的資料
            //var selectedProductDiscounts = await _productsDiscountRepository
            //    .ListAsync(spds => productIds.Contains(spds.ProductId) &&
            //              spds.SalesStartDate <= currentDate &&
            //              spds.SalesEndDate >= currentDate);


            //不印出過去的折扣活動資料
            var selectedProductDiscounts = await _productsDiscountRepository
                .ListAsync(spds => productIds.Contains(spds.ProductId) && currentDate <= spds.SalesEndDate);

            var discountsToRead = new List<DiscountReadDto>();

            var allProduct = await _productRepository.ListAsync();

            foreach (var spd in selectedProductDiscounts)
            {
                var discountProductName = allProduct.FirstOrDefault(ap => ap.ProductId == spd.ProductId)?.ProductName;

                decimal formatedDiscountValue = (1 - spd.Discount) * 100;

                var discountDto = new DiscountReadDto
                {
                    ProductId = spd.ProductId,
                    ProductName = discountProductName,
                    DiscountStartDate = spd.SalesStartDate,
                    DiscountEndDate = spd.SalesEndDate,
                    DiscountValue = formatedDiscountValue
                };

                discountsToRead.Add(discountDto);
            }

            return discountsToRead.OrderBy(d => d.ProductId).ToList();
        }
        
        /*產品上下架停用區*/
        public async Task ActivateProducts(List<int> productIds)
        {

            var productsInDB = await _productRepository.ListAsync(p => productIds.Contains(p.ProductId));

            if (productsInDB != null && productsInDB.Any())
            {

                foreach (var product in productsInDB)
                {
                    product.ProductStatus = 1;
                }

                await _productRepository.UpdateRangeAsync(productsInDB);
            }
        }
        public async Task DeactivateProducts(List<int> productIds)
        {
            // 批量查詢需要更新的產品
            var productsInDB = await _productRepository.ListAsync(p => productIds.Contains(p.ProductId));

            if (productsInDB != null && productsInDB.Any())
            {
                // 批量更新產品狀態為已下架 (2)
                foreach (var product in productsInDB)
                {
                    product.ProductStatus = 2;
                }

                // 批量更新到資料庫
                await _productRepository.UpdateRangeAsync(productsInDB);
            }
        }

        //public async Task<List<AllProductDto>> GetProducts()
        //{
        //    var currentDate = DateOnly.FromDateTime(DateTime.Now);

        //    var products = await _productRepository.ListAsync();

        //    var productDtos = new List<AllProductDto>();

        //    foreach (var p in products)
        //    {
        //        //原始售價
        //        var formattedPrice = $"NT$ {Math.Round(p.ProductPrice):N0}";

        //        //在每個產品上查詢對應的折扣
        //        var currentProductDiscounts = await _productsDiscountRepository.ListAsync(pd =>
        //            pd.ProductId == p.ProductId &&
        //            pd.SalesStartDate <= currentDate &&
        //            pd.SalesEndDate >= currentDate);

        //        //判斷是否有折扣，並選擇最大的折扣
        //        var currentDiscount = currentProductDiscounts.Any() ? currentProductDiscounts.Min(x => x.Discount) : 1m;

        //        var formattedDiscount = currentDiscount < 1 ? $"- {(1 - currentDiscount) * 100:0}%" : "";

        //        //實際售價
        //        var productSalesPrice = (p.ProductPrice * currentDiscount);

        //        //銷售狀態
        //        var ProductSellStatus = currentDate >= p.ProductShelfTime;

        //        //評論
        //        var comments = await _productCommentRepository.ListAsync(comment => comment.ProductId == p.ProductId);
        //        var goodComments = comments.Count(comment => comment.Comment);
        //        var badComments = comments.Count(comment => !comment.Comment);

        //        var totalComments = goodComments + badComments;

        //        double goodCommentsPercentage = 0;
        //        double badCommentsPercentage = 0;

        //        //公告
        //        var announcements = await _productPageEventRepository.ListAsync(x => x.ProductId == p.ProductId) ?? new List<ProductPageEvent>();

        //        if (announcements == null)
        //        {
        //            announcements = new List<ProductPageEvent>();
        //        }

        //        //類別
        //        var category = await _categoryRepository.FirstOrDefaultAsync(x => x.CategoryId == p.CategoryId);

        //        //發行商
        //        var publisher = await _publisherRepository.FirstOrDefaultAsync(x => x.PublisherId == p.PublisherId);

        //        if (totalComments > 0)
        //        {
        //            // 四捨五入好評和差評百分比
        //            goodCommentsPercentage = Math.Round((double)goodComments / totalComments * 100, 0);
        //            badCommentsPercentage = Math.Round((double)badComments / totalComments * 100, 0);
        //        }

        //        //構建DTO並添加到結果列表中
        //        productDtos.Add(new AllProductDto
        //        {
        //            ProductId = p.ProductId,
        //            ProductMainImageUrl = p.ProductMainImageUrl,
        //            ProductName = p.ProductName,
        //            ProductPrice = p.ProductPrice,
        //            ProductSalesPrice = productSalesPrice,
        //            ProductMainDescription = p.Description,
        //            ProductDiscount = formattedDiscount,
        //            ProductShelfTime = p.ProductShelfTime,
        //            ProductSellStatus = ProductSellStatus,
        //            ProductStatus = p.ProductStatus,
        //            ProductGoodCommentPercentage = goodCommentsPercentage,
        //            ProductBadCommentPercentage = badCommentsPercentage,
        //            ProductCategory = category.CategoryName,
        //            ProductPublisher = publisher.PublisherName,
        //            ProductAnnouncements = announcements.Select(a => new ProductAnnouncement
        //            {
        //                EventsId = a.EventsId,
        //                Title = a.Title,
        //                AnnounceText = a.AnnounceText,
        //                Content = a.Content,
        //                ProductImgUrl = a.ProductImgUrl,
        //                AnnouncementDate = a.AnnouncementDate
        //            }).ToList()
        //        });
        //    }


        //    return productDtos;
        //}

        /*上架產品抓取類別及標籤*/
        public async Task<List<AllCategoryDto>> GetCategories()
        {
            var category = (await _categoryRepository.ListAsync()).Select(c => new AllCategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
            }).ToList();

            return category;
        }
        public async Task<List<AllTagDto>> GetTags()
        {
            var tag = (await _tagRepository.ListAsync()).Select(t => new AllTagDto
            {
                TagId = t.TagId,
                TagName = t.TagName,
            }).ToList();

            return tag;
        }
        
        /*產品公告區*/
        public async Task AddProductEvent(CreateEventDto request)
        {
            var eventImageUrl = _cloudinaryService.UploadImage(request.EventImgUrl);

            var Productevent = new ProductPageEvent
            {
                ProductId = request.ProductId,
                Title = request.Title,
                AnnounceText = request.AnnounceText,
                Content = request.Content,
                ProductImgUrl = eventImageUrl,
                AnnouncementDate = request.AnnouncementDate
            };

            await _productPageEventRepository.AddAsync(Productevent);

        }
        
        /*public async Task<List<ProductAnnouncement>> GetProductEvents(int productId)
        {
            var productEvents = await _productPageEventRepository.ListAsync(x => x.ProductId == productId);

            var productEventsToRead = new List<ProductAnnouncement>();

            foreach (var pe in productEvents)
            {
                var productEvent = new ProductAnnouncement
                {
                    EventsId = pe.EventsId,
                    Title = pe.Title,
                    AnnounceText = pe.AnnounceText,
                    Content = pe.Content,
                    ProductImgUrl = pe.ProductImgUrl,
                    AnnouncementDate = pe.AnnouncementDate
                };
                productEventsToRead.Add(productEvent);
            }
            return productEventsToRead.ToList();
        }*/
        public async Task EditProductEvent(CreateEventDto request)
        {
            var productEvent = await _productPageEventRepository.GetByIdAsync(request.EventsId);
            productEvent.Title = request.Title;
            productEvent.AnnounceText = request.AnnounceText;
            productEvent.Content = request.Content;
            productEvent.AnnouncementDate = request.AnnouncementDate;
            if (request.EventImgUrl != null)
            {
                var eventImageUrl = _cloudinaryService.UploadImage(request.EventImgUrl);
                productEvent.ProductImgUrl = eventImageUrl;
            }

            await _productPageEventRepository.UpdateAsync(productEvent);
        }
        public async Task RemoveProductEvent(int eventId)
        {
            var productEvent = await _productPageEventRepository.GetByIdAsync(eventId);
            await _productPageEventRepository.DeleteAsync(productEvent);
        }

        /*產品輪播圖區*/
        public async Task UpdateAndCreateCarousels(List<ProductsCarouselDto> request)
        {
            foreach (var carouselDto in request)
            {
                if (carouselDto.CarouselId != 0)
                {
                    var carousel = await _productCarouselRepository.GetByIdAsync(carouselDto.CarouselId);

                    carousel.Type = (byte)(carouselDto.Sort + 1);
                    await _productCarouselRepository.UpdateAsync(carousel);
                }
                else
                {
                    var carouselImageUrl = _cloudinaryService.UploadImage(carouselDto.CarouselImages);

                    var productCarousel = new ProductCarousel
                    {
                        ProductId = carouselDto.ProductID,
                        CarouselUrl = carouselImageUrl,
                        DataSourceUrl = carouselImageUrl,
                        Type = (byte)(carouselDto.Sort + 1)
                    };
                    await _productCarouselRepository.AddAsync(productCarousel);
                }
            }
        }
        public async Task<List<ProductsCarouselDto>> GetAllCarousels(int productId)
        {
            var carousels = await _productCarouselRepository.ListAsync(x => x.ProductId == productId);

            var carouselsToRead = new List<ProductsCarouselDto>();

            foreach (var ca in carousels)
            {
                var carouselDto = new ProductsCarouselDto
                {
                    CarouselId = ca.CarouselId,
                    ProductID = ca.ProductId,
                    DataSourceUrl = ca.DataSourceUrl,
                    Sort = ca.Type
                };
                carouselsToRead.Add(carouselDto);
            }
            return carouselsToRead.OrderBy(x => x.Sort).ToList();
        }
        public async Task DeleteCarousel(int carouselId)
        {
            var carousel = await _productCarouselRepository.GetByIdAsync(carouselId);
            await _productCarouselRepository.DeleteAsync(carousel);
        }
    }
}
