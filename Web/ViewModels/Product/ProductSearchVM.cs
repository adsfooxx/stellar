
//using ApplicationCore.Interfaces;
//using ApplicationCore.Entities;
using NuGet.Protocol.Core.Types;
using Web.Services.Search;
using ApplicationCore.Entities;
namespace Web.ViewModels.Product
{
    public class ProductSearchVM
    {
      
        public List<ProductInfoVM> Products { get; set; }
        public int TotalCount { get; set; }
        public List<CategoryVM> Categorys { get; set; }
        public List<TagVM> Tags { get; set; }
        public List<SearchCategoryVM> searchCategorys { get; set; }
        public List<SearchTagVM> searchTags { get; set; }

        public int FilterCount { get; set; }
    }

    public class SearchCategoryVM
    {

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

    }


    public class SearchTagVM
    {

        public int TagId { get; set; }
        public string TagName { get; set; }

    }


    public class TagVM
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
    }

    public class CategoryVM
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

    }
    public class ProductInfoVM
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public List<int> TagIds { get; set; }
        public string ProductImgUrl { get; set; }
        public string OperatingSystemImg { get; set; } //ios windows
        public string CommentImgUrl { get; set; } //讚 倒讚
        public string Startdate { get; set; }
        public int DiscountPercent { get; set; } //百分比
        public bool HasDiscount { get; set; }
        public decimal UnitlPrice { get; set; } //原價
        public decimal SalsePrice { get; set; } //折扣後的價格
    }
}
