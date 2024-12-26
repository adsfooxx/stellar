using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stellar.API.Service.Category_Tag;
using Stellar.API.Service.JWT;
using Stellar.API.Service.Charts;
using Stellar.API.Service.Product;
using Stellar.API.Service.Publisher;
using Stellar.API.Settings;
using System.Security.Claims;
using System.Text;
//using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Data.Mail;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Infrastructure.Data.Mongo.Repository;
using Infrastructure.Data.Mongo;
using MongoDB.Driver;
using CloudinaryDotNet;
using Infrastructure.Data.Cloudnary;
using Infrastructure.Services;


using Autofac.Core;


namespace Stellar.API.Configurations
{
    [Experimental("SKEXP0001")]
    public static class ConfigureWebApiService
    {

        public static IServiceCollection AddWebApiService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<PublisherService>();
            services.AddScoped<ProductService>();
            services.AddScoped<ChartsService>();

            services.AddScoped<CategoryService>();
            services.AddScoped<TagService>();
            //services.AddAutoMapper(typeof(Program)); // 注入AutoMapper


            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
       

            services.AddSingleton<JwtService>();

            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
              // 獲取 JwtSettings
              var jwtSettings = services.BuildServiceProvider().GetRequiredService<IOptions<JwtSettings>>().Value;

              options.TokenValidationParameters = new TokenValidationParameters
              {
                  // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
                  NameClaimType = ClaimTypes.NameIdentifier,

                  // 透過這項宣告，就可以從 "ClaimTypes.Role" 取值，並可讓 [Authorize] 判斷角色
                  RoleClaimType = ClaimTypes.Role,

                  // 一般我們都會驗證 Issuer
                  ValidateIssuer = true,
                  ValidIssuer = jwtSettings.Issuer, // 使用 JwtSettings

                  // 通常不太需要驗證 Audience
                  ValidateAudience = false,

                  // 一般我們都會驗證 Token 的有效期間
                  ValidateLifetime = true,
                  ClockSkew = TimeSpan.Zero,

                  // 如果IssuerSigningKey有設定時，ValidateIssuerSigningKey的值一定會是true，也就是會檢查SignKey的真偽
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SignKey)) // 使用 JwtSettings
              };
          });
            




            return services;
        }
    }
}



//.AddGoogle(options =>
// {

//     var googleSettings = configuration.GetSection("GoogleSettings").Get<GoogleSettings>();

//     options.ClientId = googleSettings.Google.ClientId;
//     options.ClientSecret = googleSettings.Google.ClientSecret;

//     // 設置 Google 的回調 URI
//     var callbackUri = new Uri(googleSettings.Google.RedirectUri);
//     options.CallbackPath = callbackUri.AbsolutePath;

// });