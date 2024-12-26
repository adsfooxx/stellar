using AutoMapper;
using Web.ViewModels.Product;

namespace Web.Services.Search
{
    public class ProductProfile : Profile //定義物件之間的映射規則
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductInfoVM>(); // 將 Product 映射到 ProductInfoVM
            CreateMap<Category, CategoryVM>();    // 將 Category 映射到 CategoryVM
            CreateMap<ApplicationCore.Entities.Tag, TagVM>(); // 將 Tag 映射到 TagVM
        }
    }
}
