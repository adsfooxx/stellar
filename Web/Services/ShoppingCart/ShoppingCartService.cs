using ApplicationCore.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Web.ViewModels.ShoppingCart;
namespace Web.Services.ShoppingCart
{
    public class ShoppingCartService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ShoppingCartCard> _shoppingCartCardRepository;
        private readonly IRepository<ProductsDiscount> _productsDiscountRepository;
        private readonly IRepository<ProductCollection> _productCollectionRepository;

        public ShoppingCartService(IRepository<Product> productRepository, IRepository<ShoppingCartCard> shoppingCartCardRepository, IRepository<ProductsDiscount> productsDiscountRepository, IRepository<ProductCollection> productCollectionRepository)
        {
            _productRepository = productRepository;
            _shoppingCartCardRepository = shoppingCartCardRepository;
            _productsDiscountRepository = productsDiscountRepository;
            _productCollectionRepository = productCollectionRepository;
        }

        public async Task<ShoppingCartViewModel> GetShoppingCartData(int userId)
        {
            var shoppingCarts = await _shoppingCartCardRepository.ListAsync(u => u.UserId == userId);
            var productIdsInCart = shoppingCarts.Select(p => p.ProductId).ToList();
            var productCollection = await _productCollectionRepository.ListAsync(u => u.UserId == userId);
            //抓取user購物車裡的productId & 價格不等於0 & 排除收藏庫裡的產品
            var products = await _productRepository.ListAsync(p => productIdsInCart.Contains(p.ProductId) && p.ProductPrice != 0 && !productCollection.Select(p => p.ProductId).Contains(p.ProductId));
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var discounts = await _productsDiscountRepository.ListAsync(d => d.SalesStartDate <= currentDate && d.SalesEndDate >= currentDate);

            var shoppingCartData = from cart in shoppingCarts
                                   join product in products on cart.ProductId equals product.ProductId
                                   join discount in discounts on product.ProductId equals discount.ProductId into productDiscounts
                                   from discount in productDiscounts.DefaultIfEmpty()
                                   select new
                                   {
                                       ShoppingCart = cart,
                                       Product = product,
                                       Discount = discount
                                   };

            //排除購物車中已經存在的產品 & 排除免費 & 排除收藏庫已經存在的產品 & 排除未上架的產品
            var ExcludeProductsFromCart = await _productRepository.ListAsync(p => !productIdsInCart.Contains(p.ProductId) && p.ProductPrice > 0 && !productCollection.Select(p => p.ProductId).Contains(p.ProductId) && p.ProductStatus == 1);

            var random = new Random();

            var recommend = from product in ExcludeProductsFromCart
                            join discount in discounts on product.ProductId equals discount.ProductId into productDiscounts
                            from discount in productDiscounts.DefaultIfEmpty()
                            orderby random.Next()
                            select new
                            {
                                Product = product,
                                Discount = discount
                            };

            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                ShoppingCartProducts = shoppingCartData.Distinct()
                                            .Select(data => CreateShoppingCartProduct(data))
                                            .ToList(),

                TotalPrice = shoppingCartData.Where(p => p.Product.ProductStatus == 1).Sum(p => Math.Round(p.Product.ProductPrice * (p.Discount?.Discount ?? 1))),
                //推薦三個月內的產品
                RecommendWithinHalfYear = recommend.Where(p => p.Product.ProductShelfTime > currentDate.AddMonths(-6))
                                            .Select(p => CreateShoppingCartProduct(p))
                                            .Take(3)
                                            .ToList(),
                //推薦三個月前至一年內的產品
                RecommendProducts2 = recommend.Where(p => 
                                            p.Product.ProductShelfTime >= currentDate.AddYears(-1) &&
                                            p.Product.ProductShelfTime <= currentDate.AddMonths(-6))
                                            .Select(p => CreateShoppingCartProduct(p))
                                            .Take(4)
                                            .ToList(),

            };

            return await Task.FromResult(shoppingCartViewModel);
        }

        public async Task<bool> RemoveShoppingCart(int userId, int productId)
        {
            // 查找購物車中的商品
            var shoppingItem = await _shoppingCartCardRepository.FirstOrDefaultAsync(x => x.ProductId == productId && x.UserId == userId);

            if (shoppingItem != null)
            {
                _shoppingCartCardRepository.Delete(shoppingItem); // 刪除單筆商品
                return true; // 返回成功
            }

            return false; // 如果找不到商品，返回 false
        }
        public async Task<bool> RemoveAllShoppingCart(int userId)
        {
            // 查找購物車中的商品
            var shoppingItems = await _shoppingCartCardRepository.ListAsync(x => x.UserId == userId);

            if (shoppingItems.Any())
            {
                _shoppingCartCardRepository.DeleteRange(shoppingItems); // 刪除所有項目
                return true; // 返回成功
            }

            return false; // 如果找不到項目，返回 false
        }

        private ShoppingCartProduct CreateShoppingCartProduct(dynamic data)
        {
            var discount = data.Discount?.Discount ?? 1;
            var discountPercentage = (1 - discount) * 100;
            var salesPrice = Math.Round(data.Product.ProductPrice * discount);
            var productStatus = _productRepository.List()
                    .Where(x => x.ProductId == data.Product.ProductId)
                    .Select(x => x.ProductStatus)
                    .FirstOrDefault();

            return new ShoppingCartProduct
            {
                ProductId = data.Product.ProductId,
                ProductName = data.Product.ProductName, // 這是 ShoppingCartProduct 的屬性
                ProductImgUrl = data.Product.ProductMainImageUrl,
                ProductPrice = data.Product.ProductPrice,
                ProductPriceFormat = data.Product.ProductPrice.ToString("#,###"),
                Discount = discountPercentage,
                DiscountFormat = discountPercentage.ToString("##"),
                SalesPrice = salesPrice,
                SalesPriceFormat = salesPrice.ToString("#,###"),
                ProductStatus = Convert.ToByte(productStatus)
            };
        }
    }
}
