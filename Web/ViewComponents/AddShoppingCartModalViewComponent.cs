using Microsoft.AspNetCore.Mvc;
using Web.Services.Partial;
namespace Web.ViewComponents
{
    public class AddShoppingCartModalViewComponent : ViewComponent
    {
        private readonly AddShoppingCartService _addToCartService;

        public AddShoppingCartModalViewComponent(AddShoppingCartService addToCartService)
        {
            _addToCartService = addToCartService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentProductId)
        {
            var addToCart = await _addToCartService.GetAddToCart(currentProductId);
            return View("_AddShoppingCartModal", addToCart);
        }
    }
}
