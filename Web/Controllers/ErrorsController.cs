using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ErrorPage([FromQuery] string statuscode)
        {
            //方法參數前面用[FromQuery]讓Model Binding自動繫結查詢字串

            //以下利用Request.Query["key"]取得查詢字串
            string StatusCode = Request.Query["statuscode"];

            ViewData["StatusCode"] = statuscode;

            //發生404錯誤時導向/
            /*
            if (StatusCode == "404")
            {
                return LocalRedirect("/");
            }
            */


            return View();
        }
    }
}
