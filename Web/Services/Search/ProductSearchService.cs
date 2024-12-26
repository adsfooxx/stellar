using ApplicationCore.Interfaces;
using AutoMapper;
using Humanizer;
using Infrastructure.Data.Mongo.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver.Linq;
using System.Data;
using System.Linq;
using Web.Extensions;
using Web.Helpers;
using Web.ViewModels.Product;

namespace Web.Services.Search
{
    public class ProductSearchServices
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductComment> _productCommentRepository;
        private readonly IRepository<ApplicationCore.Entities.Tag> _tagRepository;
        private readonly IRepository<ApplicationCore.Entities.Category> _categoryRepository;
        private readonly IRepository<ProductsDiscount> _productsDiscountRepository;
        private readonly IRepository<TagConnect> _tagConnectRepository;
        private readonly IRepository<ProductCollection> _productCollectionRepository;
        private readonly List<TagVM> _tags;

        private readonly List<CategoryVM> _categorys;
        private readonly IDbConnection _dbConnection;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductSearchServices> _logger;

        private readonly IDistributedCache _distributedCache; //快取的東西
        public ProductSearchServices(
            IRepository<Product> productRepository,
            IRepository<ProductComment> productCommentRepository,
            IRepository<ApplicationCore.Entities.Tag> tagRepository,
            IRepository<ProductsDiscount> ProductsDiscountRepository,
            IRepository<ApplicationCore.Entities.Category> categoryRepository,
            IRepository<TagConnect> tagConnectRepository,
            IRepository<ProductCollection> productCollectionRepository,
            IMapper mapper,
            ILogger<ProductSearchServices> logger,
            IDistributedCache distributedCache,
            IDbConnection dbConnection)
        {
            _productRepository = productRepository;
            _productCommentRepository = productCommentRepository;
            _tagRepository = tagRepository;
            _productsDiscountRepository = ProductsDiscountRepository;
            _categoryRepository = categoryRepository;
            _tagConnectRepository = tagConnectRepository;
            _productCollectionRepository = productCollectionRepository;
            _mapper = mapper;
            _logger = logger;
            _categorys = GetItems<ApplicationCore.Entities.Category, CategoryVM>(categoryRepository);
            _tags = GetItems<ApplicationCore.Entities.Tag, TagVM>(tagRepository);
            _distributedCache = distributedCache;
            _dbConnection = dbConnection;
        }



        private List<TVM> GetItems<T, TVM>(IRepository<T> repository)
    where T : class
    where TVM : class
        {
            try
            {
                var items = repository.List();

                if (items == null || !items.Any())
                {
                    return new List<TVM>();
                }

                return _mapper.Map<List<TVM>>(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping items from {Repository}", repository.GetType().Name);
                throw;
            }
        }



        private async Task<ProductSearchVM> TryGetProductSearchVMFromCacheAsync(string cacheKey)
        {
            var cacheValue = await _distributedCache.GetAsync(cacheKey);
            var cachedProducViewModel = SerializationHelper.ByteArrayToObj<ProductSearchVM>(cacheValue);
            return cachedProducViewModel;
        }

        private async Task SetCachedProductSearchVMAsync(string cacheKey, ProductSearchVM model)
        {
            var byteResult = SerializationHelper.ObjectToByteArray(model);

            await _distributedCache.SetAsync(cacheKey, byteResult, new DistributedCacheEntryOptions()
            {
                SlidingExpiration = slidingExpiration,
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            });
        }



        TimeSpan slidingExpiration = TimeSpan.FromMinutes(1);
        TimeSpan absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        string cacheKeyRedisStr()
        {
            return "ProductSearchViewModel-Redis";
        }



        private ProductSearchVM CreateProductSearchViewModel(List<ProductInfoVM> products, List<SearchTagVM> selectedTags = null, List<SearchCategoryVM> selectedCategories = null, int count = 0)
        {
            return new ProductSearchVM
            {
                Categorys = _categorys,
                Tags = _tags,
                searchCategorys = selectedCategories,
                searchTags = selectedTags,
                Products = products,
                TotalCount = count == 0 ? products.Count() : count,
            };
        }





        public async Task<ProductSearchVM> GetProductData(int page, int pageSize, decimal min, decimal max)
        {

            var cacheKey = $"{cacheKeyRedisStr()}-product-{page}-{pageSize}-{min}-{max}";

            var cachedProducViewModel = await TryGetProductSearchVMFromCacheAsync(cacheKey);
            if (cachedProducViewModel != null)
            {
                return cachedProducViewModel;
            }

            var isHasValue = min > 0 || max < 10000;
        
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            int filterCount = 0;
            var totalCount = _productRepository.Query(p => p.ProductStatus == 1).Count();
            var products = new List<ProductInfoVM>().AsEnumerable();

            if (isHasValue)
            {

                products = (await _productRepository.ListAsync(p => p.ProductStatus == 1 && p.ProductPrice >= min && p.ProductPrice <= max)).CreateProductInfo();
                var discounts1 = await products.GetDiscountsByProductsAsync(_productsDiscountRepository);          
                products = products.ApplyDiscounts(discounts1);
                products = products.Where(p => p.SalsePrice >= min && p.SalsePrice <= max); 
                filterCount = totalCount - products.Count();
             

            }
            else
            {
                products = (await _productRepository.ListAsync(x => x.ProductStatus == 1, page, pageSize)).CreateProductInfo();

                var discounts = await products.GetDiscountsByProductsAsync(_productsDiscountRepository);
                products = products.ApplyDiscounts(discounts); 

            }



            var tagDic = await products.GetTagDictionaryByProductsAsync(_tagConnectRepository);

            products = products.ApplyTags(tagDic);

            var commentsByProduct = await products.GetCommentsCountByProductAsync(_productCommentRepository);

            products = products.ApplyCommentImages(commentsByProduct);


            var model = CreateProductSearchViewModel(products.ToList(), null, null, totalCount);

            if (filterCount != 0) { model.FilterCount = filterCount; }

            await SetCachedProductSearchVMAsync(cacheKey, model);

            return model;
        }


        public async Task<ProductSearchVM> GetProductDataByPriceRange(decimal minPrice, decimal maxPrice)
        {

            var cacheKey = $"{cacheKeyRedisStr()}-price-{minPrice}-{maxPrice}";
            var cachedProducViewModel = await TryGetProductSearchVMFromCacheAsync(cacheKey);
            if (cachedProducViewModel != null)
            {
                return cachedProducViewModel;
            }

            var min = minPrice == 0 ? minPrice : minPrice - 1;
            var max = maxPrice == 0 ? maxPrice : maxPrice - 1;

            var products = (await _productRepository.ListAsync(p => p.ProductStatus == 1 && p.ProductPrice > min && p.ProductPrice < max)).CreateProductInfo();

            var discounts = await products.GetDiscountsByProductsAsync(_productsDiscountRepository);
            products = products.ApplyDiscounts(discounts);

            products.ToList().RemoveAll(p => p.SalsePrice < min || p.SalsePrice > max);


            var tagDic = (await products.GetTagDictionaryByProductsAsync(_tagConnectRepository));

            products = products.ApplyTags(tagDic);

            var commentsByProduct = await products.GetCommentsCountByProductAsync(_productCommentRepository);

            products = products.ApplyCommentImages(commentsByProduct);

            var selectedTags = await products.GetTagsByProductsAsync(_tagRepository);
            var selectedCategories = await products.GetCategoriesByProductsAsync(_categoryRepository);

            var model = CreateProductSearchViewModel(products.ToList(), selectedTags, selectedCategories);

            await SetCachedProductSearchVMAsync(cacheKey, model);

            return model;
        }




        public async Task<ProductSearchVM> GetProductDataByQuery(string gameKeywords)
        {

            var cacheKey = $"{cacheKeyRedisStr()}-gameKeywords-{gameKeywords}";

            var cachedProducViewModel = await TryGetProductSearchVMFromCacheAsync(cacheKey);
            if (cachedProducViewModel != null)
            {
                return cachedProducViewModel;
            }

            var keywords = gameKeywords.ToLower().Trim().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var products = (await _productRepository.ListAsync(p => keywords.Any(key => p.ProductName.ToLower().Contains(key)) && p.ProductStatus == 1)).
             CreateProductInfo();

            var discounts = await products.GetDiscountsByProductsAsync(_productsDiscountRepository);

            products = products.ApplyDiscounts(discounts);


            var tagDic = await products.GetTagDictionaryByProductsAsync(_tagConnectRepository);

            products = products.ApplyTags(tagDic);


            var commentsByProduct = await products.GetCommentsCountByProductAsync(_productCommentRepository);

            products = products.ApplyCommentImages(commentsByProduct);

            var selectedTags = await products.GetTagsByProductsAsync(_tagRepository);
            var selectedCategories = await products.GetCategoriesByProductsAsync(_categoryRepository);


            var model = CreateProductSearchViewModel(products.ToList(), selectedTags, selectedCategories);

            await SetCachedProductSearchVMAsync(cacheKey, model);
            return model;
        }





        public async Task<ProductSearchVM> GetProductDataByCategoryAndTag(List<int> categoryIds, List<int> tagIds, List<string> categoryNames, List<string> tagNames)
        {

        

            var selectedCategories = (await
                _categoryRepository.ListAsync(ca => categoryIds.Contains(ca.CategoryId) ||
                                                  categoryNames.Select(name => name.ToLower()).Contains(ca.CategoryName.ToLower()))).Distinct()
                                    .Select(c => new SearchCategoryVM { CategoryId = c.CategoryId, CategoryName = c.CategoryName })
                                    .ToList();



            var selectedTags = (await _tagRepository
                                .ListAsync(ta => tagIds.Contains(ta.TagId) ||
                                              tagNames.Select(name => name.ToLower()).Contains(ta.TagName.ToLower())))
                                .Distinct().Select(t => new SearchTagVM { TagId = t.TagId, TagName = t.TagName }).ToList();

            var selectedProductIdByTag = (await _tagConnectRepository.ListAsync(ta => selectedTags.Select(t => t.TagId).Contains(ta.TagId))).Select(ta => new TagConnect
            {
                ProductId = ta.ProductId,

                TagId = ta.TagId
            });

            var products = (await _productRepository.ListAsync(p => (selectedCategories.Select(c => c.CategoryId).Contains(p.CategoryId) || selectedProductIdByTag.Select(p => p.ProductId).Contains(p.ProductId)) && p.ProductStatus == 1)).Distinct().CreateProductInfo();

            var discounts = await products.GetDiscountsByProductsAsync(_productsDiscountRepository);

            products = products.ApplyDiscounts(discounts);

            var tagDic = await products.GetTagDictionaryByProductsAsync(_tagConnectRepository);

            products = products.ApplyTags(tagDic);

            var commentsByProduct = await products.GetCommentsCountByProductAsync(_productCommentRepository);

            products = products.ApplyCommentImages(commentsByProduct);

            var model = CreateProductSearchViewModel(products.ToList(), selectedTags, selectedCategories);
       
            return model;
        }

        public async Task<ProductSearchVM> GetProductDataByFree(string type)
        {

            var cacheKey = $"{cacheKeyRedisStr()}-type-{type}";
            var cachedProducViewModel = await TryGetProductSearchVMFromCacheAsync(cacheKey);
            if (cachedProducViewModel != null)
            {
                return cachedProducViewModel;
            }


            var products = (await _productRepository.ListAsync(p => p.ProductPrice <= 0 && p.ProductStatus == 1)).CreateProductInfo();

            var discounts = await products.GetDiscountsByProductsAsync(_productsDiscountRepository);


            products = products.ApplyDiscounts(discounts);


            products.ToList().RemoveAll(p => p.SalsePrice <= 0);


            var tagDic = await products.GetTagDictionaryByProductsAsync(_tagConnectRepository);

            products = products.ApplyTags(tagDic);


            var commentsByProduct = await products.GetCommentsCountByProductAsync(_productCommentRepository);

            products = products.ApplyCommentImages(commentsByProduct);



            var selectedTags = await products.GetTagsByProductsAsync(_tagRepository);
            var selectedCategories = await products.GetCategoriesByProductsAsync(_categoryRepository);


            var model = CreateProductSearchViewModel(products.ToList(), selectedTags, selectedCategories);

            await SetCachedProductSearchVMAsync(cacheKey, model);

            return model;
        }





        public async Task<ProductSearchVM> GetProductDataBySellingHighest(string type)
        {
            var cacheKey = $"{cacheKeyRedisStr()}-type-{type}";
            var cachedProducViewModel = await TryGetProductSearchVMFromCacheAsync(cacheKey);
            if (cachedProducViewModel != null)
            {
                return cachedProducViewModel;
            }

            var productIds = (await _productCollectionRepository.ListAsync()).GroupBy(p => p.ProductId).Select(
                g => new
                {
                    productId = g.Key,
                    SaleCount = g.Count()
                }).OrderByDescending(g => g.SaleCount)
                .Take(10).Select(p => p.productId);


            var now = DateOnly.FromDateTime(DateTime.Now); 
            var oneYearAgo = now.AddYears(-1);  


            var products = (await _productRepository.ListAsync(p => productIds.Contains(p.ProductId) && p.ProductStatus == 1 && p.ProductShelfTime > oneYearAgo &&
    p.ProductShelfTime < now)).CreateProductInfo();


            var discounts = await products.GetDiscountsByProductsAsync(_productsDiscountRepository);

            products = products.ApplyDiscounts(discounts);

            var tagDic = await products.GetTagDictionaryByProductsAsync(_tagConnectRepository);

            products = products.ApplyTags(tagDic);

            var commentsByProduct = await products.GetCommentsCountByProductAsync(_productCommentRepository);

            products = products.ApplyCommentImages(commentsByProduct);

            var selectedTags = await products.GetTagsByProductsAsync(_tagRepository);
            var selectedCategories = await products.GetCategoriesByProductsAsync(_categoryRepository);


            var model = CreateProductSearchViewModel(products.ToList(), selectedTags, selectedCategories);

            await SetCachedProductSearchVMAsync(cacheKey, model);

            return model;
        }



        public async Task<ProductSearchVM> GetProductDataByNew(string type)
        {

            var cacheKey = $"{cacheKeyRedisStr()}-type-{type}";
            var cachedProducViewModel = await TryGetProductSearchVMFromCacheAsync(cacheKey);
            if (cachedProducViewModel != null)
            {
                return cachedProducViewModel;
            }

            var now = DateTime.Now;
            var oneMonthAgo = now.AddMonths(-1); // 計算一個月前的時間
            var taggetTime = new DateOnly(oneMonthAgo.Year, oneMonthAgo.Month, oneMonthAgo.Day).AddDays(-1); ;
            var currTime = new DateOnly(now.Year, now.Month, now.Day).AddDays(1); ;

            var products = (await _productRepository.ListAsync(p => p.ProductShelfTime > taggetTime && p.ProductShelfTime < currTime && p.ProductStatus == 1)).OrderBy(p => p.ProductShelfTime).Take(10).CreateProductInfo();
            var discounts = await products.GetDiscountsByProductsAsync(_productsDiscountRepository);
            products = products.ApplyDiscounts(discounts);
            var tagDic = await products.GetTagDictionaryByProductsAsync(_tagConnectRepository);
            products = products.ApplyTags(tagDic);

            var commentsByProduct = await products.GetCommentsCountByProductAsync(_productCommentRepository);

            products = products.ApplyCommentImages(commentsByProduct);

            var selectedTags = await products.GetTagsByProductsAsync(_tagRepository);
            var selectedCategories = await products.GetCategoriesByProductsAsync(_categoryRepository);

            var model = CreateProductSearchViewModel(products.ToList(), selectedTags, selectedCategories);

            await SetCachedProductSearchVMAsync(cacheKey, model);

            return model;
        }



        public async Task<ProductSearchVM> GetProductDataBySoon(string type)
        {
            var cacheKey = $"{cacheKeyRedisStr()}-type-{type}";
            var cachedProducViewModel = await TryGetProductSearchVMFromCacheAsync(cacheKey);
            if (cachedProducViewModel != null)
            {
                return cachedProducViewModel;
            }
            var products = (await _productRepository.ListAsync(p => p.ProductShelfTime > DateOnly.FromDateTime(DateTime.Now).AddDays(1) && p.ProductStatus == 1)).OrderBy(p => p.ProductShelfTime).CreateProductInfo();

            var discounts = await products.GetDiscountsByProductsAsync(_productsDiscountRepository);

            products = products.ApplyDiscounts(discounts);


            var tagDic = await products.GetTagDictionaryByProductsAsync(_tagConnectRepository);

            products = products.ApplyTags(tagDic);


            var commentsByProduct = await products.GetCommentsCountByProductAsync(_productCommentRepository);

            products = products.ApplyCommentImages(commentsByProduct);

            var selectedTags = await products.GetTagsByProductsAsync(_tagRepository);

            var selectedCategories = await products.GetCategoriesByProductsAsync(_categoryRepository);

            var model = CreateProductSearchViewModel(products.ToList(), selectedTags, selectedCategories);

            await SetCachedProductSearchVMAsync(cacheKey, model);

            return model;
        }


        public async Task<ProductSearchVM> GetProductDataByDiscount(string type)
        {
            var cacheKey = $"{cacheKeyRedisStr()}-type-{type}";
            var cachedProducViewModel = await TryGetProductSearchVMFromCacheAsync(cacheKey);
            if (cachedProducViewModel != null)
            {
                return cachedProducViewModel;
            }

            var currTime = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
            var discounts = (await _productsDiscountRepository.ListAsync
                (p => p.SalesStartDate < currTime &&
                p.SalesEndDate > currTime &&
                p.Discount < 1))
                .Select(d => new ProductsDiscount { ProductId = d.ProductId, Discount = d.Discount });

            var products = (await _productRepository.ListAsync(p => discounts.Select(d => d.ProductId).Contains(p.ProductId) && p.ProductStatus == 1)).Distinct().CreateProductInfo();

            products = products.ApplyDiscounts(discounts);

            var tagDic = await products.GetTagDictionaryByProductsAsync(_tagConnectRepository);

            products = products.ApplyTags(tagDic);

            var commentsByProduct = await products.GetCommentsCountByProductAsync(_productCommentRepository);

            products = products.ApplyCommentImages(commentsByProduct);

            var selectedTags = await products.GetTagsByProductsAsync(_tagRepository);
            var selectedCategories = await products.GetCategoriesByProductsAsync(_categoryRepository);


            var model = CreateProductSearchViewModel(products.ToList(), selectedTags, selectedCategories);

            await SetCachedProductSearchVMAsync(cacheKey, model);
            return model;
        }


        public async Task<List<ProductInfoVM>> GetProductBySuggestions(string keywords)
        {

            var cacheKey = $"{cacheKeyRedisStr()}-Keywords-{keywords}";


            var cacheValue = await _distributedCache.GetAsync(cacheKey);
            var cachedProducViewModel = SerializationHelper.ByteArrayToObj<List<ProductInfoVM>>(cacheValue);

            if (cachedProducViewModel != null)
            {
                return cachedProducViewModel;
            }

            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var keyword = keywords.ToLower().Trim().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var products = (await _productRepository.ListAsync(p => keyword.Any(key => p.ProductName.ToLower().Contains(key)) && p.ProductStatus == 1)).CreateProductInfo();

            var discounts = await products.GetDiscountsByProductsAsync(_productsDiscountRepository);

            var model = products.ApplyDiscounts(discounts).ToList();


            var byteResult = SerializationHelper.ObjectToByteArray(model);

            await _distributedCache.SetAsync(cacheKey, byteResult, new DistributedCacheEntryOptions()
            {
                SlidingExpiration = slidingExpiration,
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            });

            return model;
        }

    }
}






