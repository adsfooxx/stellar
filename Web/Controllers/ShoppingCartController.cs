using ApplicationCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web.Services.ShoppingCart;
using Web.ViewModels.ShoppingCart;

namespace Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ShoppingCartService _shoppingCartService;

        public ShoppingCartController(ShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> ShoppingCart()
        {
            int userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            ShoppingCartViewModel shoppingCart = await _shoppingCartService.GetShoppingCartData(userId);
            return View(shoppingCart);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveShoppingCart([FromBody] ShoppingCartProduct model)
        {
            int userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");

            // 移除購物車中的商品
            var result = await _shoppingCartService.RemoveShoppingCart(userId, model.ProductId);

            // 如果移除成功，重新計算總價
            if (result)
            {
                var cart = await _shoppingCartService.GetShoppingCartData(userId);

                return Json(new
                {
                    success = true,
                    totalPrice = cart.TotalPrice.ToString("N0"), // 格式化為千分位
                    isCartEmpty = !cart.ShoppingCartProducts.Any(),
                    cartItemCount = cart.ShoppingCartProducts.Count, // 返回購物車數量
                    productStatuses = cart.ShoppingCartProducts.Select(p => p.ProductStatus).ToList()
                });
            }
            else
            {
                return Json(new { success = false, message = "Product not found or could not be removed." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAllShoppingCart()
        {
            int userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            var result = await _shoppingCartService.RemoveAllShoppingCart(userId);

            // 如果移除成功，重新計算總價
            if (result)
            {
                var cart = await _shoppingCartService.GetShoppingCartData(userId);

                return Json(new
                {
                    success = true,
                    totalPrice = cart.TotalPrice.ToString("N0"), // 格式化為千分位
                    isCartEmpty = !cart.ShoppingCartProducts.Any(),
                    cartItemCount = cart.ShoppingCartProducts.Count, // 返回購物車數量
                    productStatuses = cart.ShoppingCartProducts.Select(p => p.ProductStatus).ToList()
                });
            }
            else
            {
                return Json(new { success = false, message = "Product not found or could not be removed." });
            }
        }
    }
}
