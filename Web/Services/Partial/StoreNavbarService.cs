using ApplicationCore.Interfaces;
using System.Security.Claims;
using Web.ViewModels.Partial;
namespace Web.Services.Partial
{
    public class StoreNavbarService
    {
        private readonly IRepository<WishCard> _wishCardRepository;
        private readonly IRepository<ShoppingCartCard> _shoppingCartCardRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCollection> _productCollectionRepository;
        private readonly IRepository<ApplicationCore.Entities.Category> _categoryRepository;

        public StoreNavbarService(IRepository<WishCard> wishCardRepository, IRepository<ShoppingCartCard> shoppingCartCardRepository, IRepository<Product> productRepository, IRepository<ProductCollection> productCollectionRepository, IRepository<ApplicationCore.Entities.Category> categoryRepository)
        {
            _wishCardRepository = wishCardRepository;
            _shoppingCartCardRepository = shoppingCartCardRepository;
            _productRepository = productRepository;
            _productCollectionRepository = productCollectionRepository;
            _categoryRepository = categoryRepository;
        }
        public  StoreNavbarViewModel GetNavbar(int userId)
        {
            var productCollection = _productCollectionRepository.List(collection => collection.UserId ==  userId);

            var wishItems = _wishCardRepository.List(wish => wish.UserId == userId);
            var productsInWish = _productRepository.List(p => wishItems.Select(x => x.ProductId).Contains(p.ProductId) && !productCollection.Select(x => x.ProductId).Contains(p.ProductId));

            var cartItems = _shoppingCartCardRepository.List(cart => cart.UserId == userId);
            var productsInCart = _productRepository.List(p => cartItems.Select(x => x.ProductId).Contains(p.ProductId) && p.ProductPrice > 0 && !productCollection.Select(x => x.ProductId).Contains(p.ProductId));

            var category = _categoryRepository.List();

            var navbar = new StoreNavbarViewModel
            {
                UserId = userId,
                WishCount = productsInWish.Count(),
                CartCount = productsInCart.Count(),
                Titles = new List<Titles>
                {
                    new Titles { Title = "免費遊玩", TitleUrl = "/Product/ProductSearchMin?typeBy=free"},
                    new Titles { Title = "暢銷商品", TitleUrl = "/Product/ProductSearchMin?typeBy=highest"},
                    new Titles { Title = "特別優惠", TitleUrl = "/Product/ProductSearchMin?typeBy=Discount"},
                    new Titles { Title = "新推出", TitleUrl = "/Product/ProductSearchMin?typeBy=New"},
                    new Titles { Title = "即將發行", TitleUrl = "/Product/ProductSearchMin?typeBy=Soon"},
                },
                categories = category.Select(x => new ViewModels.Partial.Category
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                }).ToList(),
            };

            return navbar;
        }
    }
}
