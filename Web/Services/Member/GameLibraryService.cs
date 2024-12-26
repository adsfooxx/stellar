using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.CodeAnalysis;
using Web.ViewModels.Member;

namespace Web.Services.Member
{
    public  class  GameLibraryService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ProductCollection> _productCollectionRepository;
        private readonly IRepository<Product> _productRepository;

        public GameLibraryService(IRepository<User> userRepository, IRepository<ProductCollection> productCollectionRepository, IRepository<Product> productRepository)
        {
            _userRepository = userRepository;
            _productCollectionRepository = productCollectionRepository;
            _productRepository = productRepository;
        }



        public async Task< GameLiberyViewModel> GetData(int userId )
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var pr = await _productCollectionRepository.ListAsync(s => s.UserId == userId);

            var products = ( await _productRepository.ListAsync(x => pr.Select(p => p.ProductId).Contains(x.ProductId))).Select(x =>
                new GameViewModel()
                {
                    ProductId = x.ProductId,

                    ProductName = x.ProductName,
                    ProductImgUrl = x.ProductMainImageUrl,
                    IsDownload = false,
                }).ToList();


            var model = new GameLiberyViewModel
            {
                UserId = user.UserId,
                UserName =  user.NickName,
                UserImgUrl =  user.UserImg,
                Games = products,
                GameCount = products.Count()
            };

      
            return model;
        }



    }


}

