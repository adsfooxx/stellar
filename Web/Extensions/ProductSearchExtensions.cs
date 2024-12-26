using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Web.Enums;
using Web.ViewModels.Product;

namespace Web.Extensions
{
    public static class ProductExtensions
    {
        public static IEnumerable<ProductInfoVM> CreateProductInfo(this IEnumerable<Product> products)
        {
            return products.Select(p => new ProductInfoVM
            {
                ProductID = p.ProductId,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                ProductImgUrl = p.ProductMainImageUrl,
                Startdate = p.ProductShelfTime.ToString("yyyy-MM-dd"),
                UnitlPrice = Math.Round(p.ProductPrice),
                OperatingSystemImg = PlatformIconExtensions.GetIconUrl(PlatformIcon.Windows),
            });
        }

        public static void GetProductByRange(this ProductSearchVM productSearchVM, decimal min, decimal max)
        {

            // 記錄篩選前的總產品數量
            int originalTotalCount = productSearchVM.TotalCount;

            productSearchVM.Products.RemoveAll(p => p.SalsePrice < min || p.SalsePrice > max);
            productSearchVM.FilterCount = originalTotalCount - productSearchVM.Products.Count();


            var existingCategoryIds = productSearchVM.Products
                               .Select(p => p.CategoryId)
                               .Distinct()
                               .ToHashSet();

            productSearchVM.searchCategorys.RemoveAll(s => !existingCategoryIds.Contains(s.CategoryId));

            var existingTagIds = productSearchVM.Products
                              .SelectMany(p => p.TagIds)
                              .Distinct()
                              .ToHashSet();

            productSearchVM.searchTags.RemoveAll(s => !existingTagIds.Contains(s.TagId));
   
                                      
  

        }



        public static IEnumerable<ProductInfoVM> ApplyDiscounts(
      this IEnumerable<ProductInfoVM> products,
      IEnumerable<ProductsDiscount> discounts)
        {

            var discountsDictionary = discounts.ToDictionary(d => d.ProductId, d => d.Discount);


            return products.Select(p =>
            {

                var discount = discountsDictionary.TryGetValue(p.ProductID, out var num) ? num : 1;
                p.SalsePrice = Math.Round(p.UnitlPrice * discount);
                p.DiscountPercent = (int)Math.Round((1 - discount) * 100);
                p.HasDiscount = discount < 1;

                return p;
            });
        }


        public static IEnumerable<ProductInfoVM> ApplyTags(
       this IEnumerable<ProductInfoVM> products,
       IDictionary<int, List<int>> tagDic)
        {
            return products.Select(p =>
            {
                p.TagIds ??= new List<int>();
                if (tagDic.TryGetValue(p.ProductID, out var tagIds))
                {
                    p.TagIds.AddRange(tagIds);
                }
                return p;
            });
        }



        public static async Task<List<SearchTagVM>> GetTagsByProductsAsync(
      this IEnumerable<ProductInfoVM> products,
      IRepository<ApplicationCore.Entities.Tag> tagRepository)
        {

            var tagIds = products.SelectMany(f => f.TagIds).Distinct();


            var selectedTags = (await tagRepository.ListAsync(t => tagIds.Contains(t.TagId)))
                .Select(t => new SearchTagVM { TagId = t.TagId, TagName = t.TagName })
                .ToList();

            return selectedTags;
        }


        public static async Task<List<SearchCategoryVM>> GetCategoriesByProductsAsync(
      this IEnumerable<ProductInfoVM> products,
      IRepository<Category> categoryRepository)
        {

            var categoryIds = products.Select(p => p.CategoryId).Distinct();


            var selectedCategories = (await categoryRepository.ListAsync(ca => categoryIds.Contains(ca.CategoryId)))
                .Select(c => new SearchCategoryVM
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToList();

            return selectedCategories;
        }


        public static async Task<Dictionary<int, List<int>>> GetTagDictionaryByProductsAsync(
       this IEnumerable<ProductInfoVM> products,
        IRepository<TagConnect> tagConnectRepository)
        {

            var productIds = products.Select(p => p.ProductID).Distinct();


            var tagConnects = await tagConnectRepository.ListAsync(t => productIds.Contains(t.ProductId));
            return tagConnects
                .GroupBy(g => g.ProductId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(t => t.TagId).ToList()
                );
        }


        public static async Task<IEnumerable<ProductsDiscount>> GetDiscountsByProductsAsync(
        this IEnumerable<ProductInfoVM> products,
        IRepository<ProductsDiscount> productsDiscountRepository)
        {

            var currentDate = DateOnly.FromDateTime(DateTime.Now);


            var productIds = products.Select(p => p.ProductID).Distinct();


            var discounts = await productsDiscountRepository.ListAsync(d =>
                productIds.Contains(d.ProductId) &&
                d.SalesStartDate <= currentDate &&
                d.SalesEndDate >= currentDate);


            return discounts.Select(d => new ProductsDiscount
            {
                ProductId = d.ProductId,
                Discount = d.Discount
            });
        }



        public static async Task<Dictionary<int, int>> GetCommentsCountByProductAsync(
       this IEnumerable<ProductInfoVM> products,
       IRepository<ProductComment> productCommentRepository)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products), "Products collection cannot be null.");
            }

            if (productCommentRepository == null)
            {
                throw new ArgumentNullException(nameof(productCommentRepository), "ProductCommentRepository cannot be null.");
            }


            var productIds = products.Select(p => p.ProductID).Distinct();
            var comments = await productCommentRepository.ListAsync(p => productIds.Contains(p.ProductId));

            return comments
                .GroupBy(p => p.ProductId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
        }


        public static IEnumerable<ProductInfoVM> ApplyCommentImages(
      this IEnumerable<ProductInfoVM> products,
      Dictionary<int, int> commentsByProduct)
        {
            return products.Select(p =>
            {

                p.CommentImgUrl ??= string.Empty;


                if (commentsByProduct.TryGetValue(p.ProductID, out var count))
                {
                    p.CommentImgUrl = count.GetReviewImageUrl();
                }

                return p;
            });
        }
    }

}
