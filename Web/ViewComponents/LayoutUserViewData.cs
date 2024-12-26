using ApplicationCore.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Web.Service.Layout;
using Web.ViewModels.Layout;

namespace Web.ViewComponents
{
    [ViewComponent(Name = "LayoutUserViewData")]
    public class LayoutUserViewData : ViewComponent
    {
        
        private readonly LayoutService _layoutService;

        public LayoutUserViewData(LayoutService layoutService)
        {
            _layoutService = layoutService;
        }

        public IViewComponentResult Invoke(string section)
        {

            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            LayoutViewModel userData;
            //想想怎麼帶
            if (userId != 0) { 
             userData =  _layoutService.GetUserData(userId);
            
            if (section == "LayoutUserForDesktop")
            {
                return View("LayoutUserForDesktop", userData);
            }else if(section == "LayoutUserForMobile")
            {
                return View("LayoutUserForMobile",userData);
            }
            }
            else
            {
                userData = new LayoutViewModel();
                userData.WishListCount = HttpContext.Session.GetInt32("WishListProductIdToAdd") is null ? 0 : 1;
                userData.ShoppingCartCount = HttpContext.Session.GetInt32("ProductIdToAdd") is null ? 0 : 1;
                if (section == "LayoutUserForDesktop")
                {
                    return View("LayoutUserForDesktop", userData);
                }
                else if (section == "LayoutUserForMobile")
                {
                    return View("LayoutUserForMobile", userData);
                }


            }

            return View("LayoutUserForDesktop");
        }
    }
}
