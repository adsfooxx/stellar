using ApplicationCore.Interfaces;
using ApplicationCore.LinePay.Dtos;
using CloudinaryDotNet;
using Infrastructure.Data.Cloudnary;
using Infrastructure.Data.Mail;
using Infrastructure.Data.MailKit;
using Infrastructure.Services.MailKit;
using Infrastructure.Services.Member;
using Infrastructure.Services.Payment;
using Infrastructure.Services.Product;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using System.Configuration;


namespace Web.Configurations
{
    public static class ConfigureApplicationCoreService
    {
        public static IServiceCollection AddApplicationCoreService(this IServiceCollection services, IConfiguration configuration)
        {
            
            
            services.AddScoped<ICustomerSupportQueryService, CustomerSupportQueryService>();
            services.AddScoped<IPaymentQueryService, PaymentFinishedQueryService>();
            services.AddScoped<IProductPageQueryService, ProductPageQueryService>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = ".AspNetCore.Cookies";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.LoginPath = "/Authentication/Login";
            });
            return services;
        }
    }
}
