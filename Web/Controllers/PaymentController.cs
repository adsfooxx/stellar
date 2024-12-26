using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Azure.Core;
using FluentEcpay;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Web.Services.EcPay;
using Web.Services.Payment;
using Web.ViewModels.EcPay;
using Web.ViewModels.Member;
using Web.ViewModels.Payment;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Web.Controllers
{

    public class PaymentController : Controller
    {
        private readonly PayChoiceAndDetailCheckService _payChoiceAndDetailCheckService;
        private readonly PayFinishedService _payFinishedService;
        private readonly EcPayService _ecPayService;
        private readonly IRepository<EcPay> _ecPayRepository;
        

        public PaymentController(PayChoiceAndDetailCheckService payChoiceAndDetailCheckService, PayFinishedService payFinishedService, EcPayService ecPayService, IRepository<EcPay> ecPayRepository)
        {
            _payChoiceAndDetailCheckService = payChoiceAndDetailCheckService;
            _payFinishedService = payFinishedService;
            _ecPayService = ecPayService;
            _ecPayRepository = ecPayRepository;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PayChoiceAndDetailCheck()
        {
            
            int userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "0");

            var model = await _payChoiceAndDetailCheckService.GetUserDatas(userId);
            if (model.ShoppingCart.Count == 0)
            {
                
                return RedirectToAction("ShoppingCart", "ShoppingCart");
            }
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PayChoiceAndDetailCheck(PayChoiceAndDetailCheckViewModel ecpayData)
        {

            try
            {

                var result = await _ecPayService.CreatePayment(ecpayData);
                var orderId = _ecPayRepository.FirstOrDefault(x => x.MerchantTradeNo == result.MerchantTradeNo).OrderId;
                
                Response.Cookies.Append("orderId", orderId.ToString());
                return View("EcPayCheck", result);

            }
            catch (Exception ex)
            {
                return Redirect("/Payment/PayFail");
            }



        }

        public async Task<IActionResult> PayFinished()
        {


            int userId = Int32.Parse(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value ?? "-1");
            var orderId=Request.Cookies["orderId"]??"-1";
            if(userId!=-1 && Int32.Parse(orderId) != -1) { 
            var model = await _payFinishedService.GetOrderData(userId, Int32.Parse(orderId));
                Response.Cookies.Delete("orderId");
                return View(model);
            }
            return RedirectToAction("Login", "Authentication");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Callback(PaymentResult result)
        {

            var hashKey = "pwFHCqoQZGmho4w6";
            var hashIV = "EkRm7iFT261dpevs";
            if (result.RtnCode != 1)
            {
                return Redirect("/Payment/PayFail");
            }
            // 判斷檢查碼是否正確。
            if (!CheckMac.PaymentResultIsValid(result, hashKey, hashIV)) return BadRequest();
            var merchanttradeNo = result.MerchantTradeNo;

            // 處理後續訂單狀態的更動等等...。
            await _payFinishedService.PaymentFinishedNextStep(merchanttradeNo);
            return Ok("1|OK");
        }
        public async Task<IActionResult> IsSuccess()
        {
            var orderId = Request.Cookies["orderId"] ?? "-1";
            var rtnCode = (await _ecPayRepository.FirstOrDefaultAsync(x => x.OrderId == Int32.Parse(orderId)))?.RtnCode;
            
            if (rtnCode != 1)
            {
                return RedirectToAction("PayFail", "Payment");
            }
            else
            {
                return RedirectToAction("PayFinished", "Payment");
            }
        }
        public  IActionResult PayFail()
        {
            return  View();
        }


    }
}
