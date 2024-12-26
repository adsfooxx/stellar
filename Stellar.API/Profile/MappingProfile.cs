//using AutoMapper.Extensions.Microsoft.DependencyInjection;
using AutoMapper;
using ApplicationCore.Entities;
using Stellar.API.Dtos.Category_Tag;
namespace Stellar.API.Profile
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<Tag, TagDto>();

            CreateMap<CategoryDto, Category>()
                 .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                 .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName))
                 .ForMember(dest => dest.CategoryImgUrl, opt => opt.MapFrom(src => src.CategoryImgUrl));
        }
    }
}
