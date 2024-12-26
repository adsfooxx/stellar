using CloudinaryDotNet;
using Infrastructure.Data.Cloudnary;
using Infrastructure.Services;
using Infrastructure.Services.MailKit;
using Microsoft.Extensions.Options;
using Web.Service.Layout;
using Web.Services;
using Web.Services.Authentication;
using Web.Services.CustomerService;
using Web.Services.CustomerSupport;
using Web.Services.EcPay;
using Web.Services.Home;
using Web.Services.LinePay;
using Web.Services.Member;
using Web.Services.Message;
using Web.Services.Partial;
using Web.Services.Payment;
using Web.Services.ProductNs;
using Web.Services.Search;
using Web.Services.ShoppingCart;
using Web.ViewModels.Home;
namespace Web.Configurations
{
    public static class ConfigureWebService
    {
        public static IServiceCollection AddWebService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDistributedMemoryCache(); // 確保使用內存作為 Session 存儲
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // 設定 Session 過期時間
                options.Cookie.HttpOnly = true; // 增加安全性
                options.Cookie.IsEssential = true; // 確保 cookie 在非必要情況下也可以使用
            });

            services.AddScoped<LoginService, LoginService>();
            services.AddScoped<RegiterService, RegiterService>();


            services.AddScoped<CustomerSupportService, CustomerSupportService>();
            services.AddScoped<ProductSupportService, ProductSupportService>();

            services.AddScoped<HomeService, HomeService>();

            services.AddScoped<LayoutService, LayoutService>();


            services.AddScoped<AccountPageService, AccountPageService>();
            services.AddScoped<EditDataService, EditDataService>();
            services.AddScoped<MemberIndexServices, MemberIndexServices>();
            services.AddScoped<NotifyPageService, NotifyPageService>();
            services.AddScoped<OrderDetailService, OrderDetailService>();
            services.AddScoped<PurchaseHistoryService, PurchaseHistoryService>();
            services.AddScoped<TopUpService, TopUpService>();
            services.AddScoped<WishListServices, WishListServices>();

            services.AddScoped<MessageServices, MessageServices>();

            services.AddScoped<AddShoppingCartService, AddShoppingCartService>();
            services.AddScoped<StoreNavbarService, StoreNavbarService>();

            services.AddScoped<PayChoiceAndDetailCheckService, PayChoiceAndDetailCheckService>();
            services.AddScoped<PayFinishedService, PayFinishedService>();

            services.AddScoped<ProductPageService, ProductPageService>();


            services.AddScoped<ProductSearchServices, ProductSearchServices>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "ProductSearch-Cache";
            });

            services.AddScoped<SearchRequestServices, SearchRequestServices>();

            services.AddScoped<ShoppingCartService, ShoppingCartService>();
            services.AddScoped<EcPayService, EcPayService>();

            services.AddScoped<GameLibraryService>(); // 註冊 GameLibraryService api
            services.AddScoped<PurchaseHistoryService>(); // 註冊 GameLibraryService api
            services.AddScoped<ProductSearchServices>();

            services.AddScoped<ChangePasswordService>();

            services.AddScoped<CloudinaryService>();
            services.AddScoped<EcPayService, EcPayService>();


            services.AddScoped<AddPhoneService>();
            services.AddScoped<ChangeEmailService>();
            services.AddScoped<DifyCustomerSupportService>();
            services.AddScoped<LinePayService>(); // 註冊 LinePayService

            // 將 IChangePasswordService 接口與 ChangePasswordService 實現綁定
            services.AddScoped<IChangePasswordService, ChangePasswordService>();

            // 其他服務註冊
            services.AddControllersWithViews();

            return services;
        }
    }
}
