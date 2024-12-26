using ApplicationCore.Interfaces;
using Web.ViewModels.Partial;
namespace Web.Services.Partial
{
    public class AddShoppingCartService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductsDiscount> _productsDiscountRepository;
        public AddShoppingCartService(IRepository<Product> productRepository, IRepository<ProductsDiscount> productsDiscountRepository)
        {
            _productRepository = productRepository;
            _productsDiscountRepository = productsDiscountRepository;
        }

        public async Task<AddShoppingCartViewModel> GetAddToCart(int currentProductId)
        {
            //int currentProductId = 2;
            //var repository = new StellarRepository(new StellarDBContext());
            var products = await _productRepository.FirstOrDefaultAsync(x => x.ProductId == currentProductId);
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var productdiscount = await _productsDiscountRepository.FirstOrDefaultAsync(x => x.ProductId == currentProductId && x.SalesStartDate <= currentDate && x.SalesEndDate >= currentDate);

            // 合併資料
            //var addToCartData = from product in products
            //                    join discount in productdiscount on products.pr equals discount.ProductId
            //                    select new
            //                    {
            //                        ShoppingCart = cart,
            //                        Product = product,
            //                        Discount = discount
            //                    };


            var addToCart = new AddShoppingCartViewModel()
            {
                ProductId = currentProductId,
                ProductName = products.ProductName,
                ProductImgUrl = products.ProductMainImageUrl,
                ProductPrice = products.ProductPrice,
                ProductPriceFormat = products.ProductPrice.ToString("#,###.00"),
                Discount = (1 - (productdiscount?.Discount ?? 1)) * 100,
                DiscountFormat = ((1 - (productdiscount?.Discount ?? 1)) * 100).ToString("##"),
                SalesPrice = Math.Round(products.ProductPrice * (productdiscount?.Discount ?? 1)),
                SalesPriceFormat = Math.Round(products.ProductPrice * (productdiscount?.Discount ?? 1)).ToString("#,###.00")
            };

            return addToCart;
        }
    }   

}
