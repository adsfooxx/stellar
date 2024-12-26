using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Services.CustomerService;
using Web.Services.Member;
using Web.ViewModels.CustomerSupport;

namespace Web.Controllers
{
    public class CustomerSupportController : Controller
    {
        private readonly CustomerSupportService _customerSupportService;
        private readonly ProductSupportService _productSupportService;
        private readonly IRepository<ProductCollection> _productCollectionRepository;

        public CustomerSupportController(CustomerSupportService customerSupportService, ProductSupportService productSupportService, IRepository<ProductCollection> productCollectionRepository)
        {
            _customerSupportService = customerSupportService;
            _productSupportService = productSupportService;
            _productCollectionRepository = productCollectionRepository;
        }

        public async Task<IActionResult> CustomerSupport()
        {
            CustomerSupportViewModel model;
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            if (userId != 0) {
             model = await _customerSupportService.GetCustomerSupportData(userId);
            }
            else
            {
                model = new CustomerSupportViewModel();
            }
            return View(model);
        }
        [HttpGet("/CustomerSupport/ProductSupport/{productId}")]
        public async Task<IActionResult> ProductSupport(int productId)
        {

            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");
            var isHave = _productCollectionRepository.Any(x => x.UserId == userId && x.ProductId == productId);
            if (isHave)
            {
                var model = await _productSupportService.GetProductSupportData(userId, productId);
                return View(model);
            }
            else
            {
                TempData["ErrorMessage"] = "操作失敗，請勿直接修改路徑到您沒有的產品資訊。";
                return RedirectToAction("CustomerSupport", "CustomerSupport");
            }
        }

    }
}
