using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Infrastructure.Services;
using Stellar.API.Dtos.Category_Tag;

namespace Stellar.API.Service.Category_Tag;
public class CategoryService
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<ApplicationCore.Entities.Product> _productRepository;
    private readonly IMapper _mapper; // 注入 IMapper
    private readonly CloudinaryService _cloudinaryService;

    public CategoryService(IRepository<Category> categoryRepository, IMapper mapper, IRepository<ApplicationCore.Entities.Product> productRepository, CloudinaryService cloudinaryService)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _productRepository = productRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<List<CategoryDto>> GetCategory()
    {
        var categories = await _categoryRepository.ListAsync();
        return _mapper.Map<List<CategoryDto>>(categories); // 使用 AutoMapper 映射到 DTO
    }


    public async Task CreateCategory(CategoryDto categoryDto)
    {

        var imgUrl = string.Empty;

        if (categoryDto.PhotoToCloudinary is not null) imgUrl =  _cloudinaryService.UploadImage(categoryDto.PhotoToCloudinary);

        else imgUrl = "https://dummyimage.com/300x200/cccccc/ffffff&text=No+Image";

        var category = new Category()
        {
            CategoryName = categoryDto.CategoryName,
            CategoryImgUrl = imgUrl,
        };

        await _categoryRepository.AddAsync(category);
    }

    public async Task UpdateCategory(CategoryDto categoryDto)
    {


        var category = await _categoryRepository.GetByIdAsync(categoryDto.CategoryId);

        var imgUrl = string.Empty;
        if (categoryDto.PhotoToCloudinary is not null)
        {
            imgUrl =  _cloudinaryService.UploadImage(categoryDto.PhotoToCloudinary);
            category.CategoryImgUrl = imgUrl;

        }

        category.CategoryName = categoryDto.CategoryName;

    
        await _categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteCategory(int id)
    {
        var products = await _productRepository.ListAsync(p => p.CategoryId == id);
        products.Select(p => p.CategoryId = 0);
        await _productRepository.UpdateRangeAsync(products);

        var category = await _categoryRepository.GetByIdAsync(id);

        await _categoryRepository.DeleteAsync(category);
    }

}
