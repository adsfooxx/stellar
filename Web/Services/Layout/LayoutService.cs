using ApplicationCore.Interfaces;
using Web.ViewModels.Layout;

namespace Web.Service.Layout
{
    public class LayoutService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Notify> _notifyRepository;
        private readonly IRepository<ShoppingCartCard> _shoppingCartRepository;
        private readonly IRepository<Notify> _notifyRespository;
        private readonly IRepository<WishCard> _wishListRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCollection> _productCollectionRepository;

        public LayoutService(IRepository<User> userRepository, IRepository<Notify> notifyRepository, IRepository<ShoppingCartCard> shoppingCartRepository, IRepository<Notify> notifyRespository, IRepository<WishCard> wishListRepository, IRepository<Product> productRepository, IRepository<ProductCollection> productCollectionRepository)
        {
            _userRepository = userRepository;
            _notifyRepository = notifyRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _notifyRespository = notifyRespository;
            _wishListRepository = wishListRepository;
            _productRepository = productRepository;
            _productCollectionRepository = productCollectionRepository;
        }

        public LayoutViewModel GetUserData(int user)
        {
            
            var userData=  _userRepository.GetById(user);
            var nortifyCount= _notifyRepository.List((y)=>y.UserId == user && y.ReadAlready == 0).Count();
            var shoppingCartCount=_shoppingCartRepository.List((z)=>z.UserId == user).Count();
            var readState = _notifyRespository.Any((r) => r.UserId==user && r.ReadAlready == 0 && r.Title.Contains("好友邀請"));
            var productCollection = _productCollectionRepository.List(collection => collection.UserId == user);
            var wishItems = _wishListRepository.List(wish => wish.UserId == user);
            var wishCount =  _productRepository.List(p => wishItems.Select(x => x.ProductId).Contains(p.ProductId) && !productCollection.Select(x => x.ProductId).Contains(p.ProductId)).Count();
            var userDataToView = new LayoutViewModel
            {
                UserId = userData.UserId,
                Account = userData.Account,
                NickName = userData.NickName,
                UserImgUrl = userData.UserImg!,
                NortifyCount = nortifyCount,
                ShoppingCartCount = shoppingCartCount,
                WishListCount = wishCount,
                WalletAmount = userData.WalletAmount.ToString("#,##0.00"),
                AlreadyRead = readState,
            };


            return userDataToView;
        }
    }
}
