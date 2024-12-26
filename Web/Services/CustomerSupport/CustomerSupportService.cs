using ApplicationCore.Interfaces;
using Microsoft.CodeAnalysis;
using Web.ViewModels.CustomerSupport;

namespace Web.Services.CustomerService
{
    public class CustomerSupportService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ProductCollection> _productCollectionRepository;
        private readonly IRepository<Product> _productRepository;

        public CustomerSupportService(IRepository<User> userRepository, IRepository<ProductCollection> productCollectionRepository, IRepository<Product> productRepository)
        {
            _userRepository = userRepository;
            _productCollectionRepository = productCollectionRepository;
            _productRepository = productRepository;
        }

        public  async Task<CustomerSupportViewModel> GetCustomerSupportData(int userId)
        {

            var userinfo = await _userRepository.FirstOrDefaultAsync((x) => x.UserId == userId);

            var userGameCollection = await _productCollectionRepository.ListAsync((x) => x.UserId == userId);
            var userGameData =await _productRepository.ListAsync((y) => userGameCollection.Select((y) => y.ProductId).Contains(y.ProductId));
            var userGameList = userGameData.Select((y) => new CustomerSupportGame
            {
                ProductId = y.ProductId,
                ProductName = y.ProductName,
                ProductImgUrl = y.ProductMainImageUrl,
            }).ToList();


            var userData = new CustomerSupportViewModel
            {
                UserId = userinfo.UserId,
                Account = userinfo.Account,
                Games = userGameList
            };
            return userData;


        }
    }
}
