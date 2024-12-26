using ApplicationCore.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Service.Layout;
using Web.Services.Partial;
using static System.Collections.Specialized.BitVector32;
using Web.ViewModels.Layout;
namespace Web.ViewComponents
{
    public class StoreNavbarViewComponent : ViewComponent
    {
        private readonly StoreNavbarService _storeNavbarService;

        public StoreNavbarViewComponent(StoreNavbarService storeNavbarService)
        {
            _storeNavbarService = storeNavbarService;
        }

        public IViewComponentResult Invoke()
        {
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            //想想怎麼帶
            if (userId == 0)
            {
                var storeNavbar = _storeNavbarService.GetNavbar(0);
                return View("_StoreNavbar", storeNavbar);
            }
            else
            {
                var storeNavbar = _storeNavbarService.GetNavbar(userId);
                return View("_StoreNavbar", storeNavbar);
            }
        }
    }
}
