using ApplicationCore.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.Extensions.Options;
using Web.Configurations;
using Web.Service.Layout;
using Web.Services.Home;
using Web.Services.Member;
using Web.Services.Partial;
using Web.Services.Search;
using Web.Hubs;
using CloudinaryDotNet;
using Infrastructure.Data.Cloudnary;
using Infrastructure.Data.MailKit;
using Infrastructure.Data.Mail;


using ApplicationCore.LinePay.Dtos;
using Web.Services.LinePay;
using Web.Services.Message;

using Web.Services.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Web.MemoryCatch;
namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddInfrastructureService(builder.Configuration);
           
            //cloudnary
            builder.Services
            .Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)))
    .       AddSingleton(settings => settings.GetRequiredService<IOptions<CloudinarySettings>>().Value);
            builder.Services.AddSingleton(sp =>
            {
                var cloudinarySettings = sp.GetRequiredService<CloudinarySettings>();
                return new Cloudinary(new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret));
            });


            builder.Services.AddWebService(builder.Configuration);
            builder.Services.AddApplicationCoreService(builder.Configuration);
            //MailKit
            //Ioption�����N�Oappsetting���ȱa�J�覡��class�����a�J �o�˥i�H���Τ@�Ӥ@�Ӽg�X��
            builder.Services.Configure<MailServerSettings>(builder.Configuration.GetSection(MailServerSettings.MailServerSettingsKey));
            builder.Services.Configure<VerifyEmailSettings>(builder.Configuration.GetSection(VerifyEmailSettings.VerifyEmailSettingsKey))
                .AddSingleton(provider => provider.GetRequiredService<IOptions<VerifyEmailSettings>>().Value);

          
            builder.Services.AddRazorPages();

            builder.Services.AddHttpClient();
            builder.Services.AddAutoMapper(typeof(Program)); // �`�JAutoMapper
            //Coravel
            builder.Services.AddSingleton<Invocable>();

            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IHomeCacheService, HomeCacheService>();


            string[] urls = new[]
            {
                "http://127.0.0.1:5500",
                "http://127.0.0.1:5502",
                "https://stellarstellaradmin.azurewebsites.net",
                "https://agent.build-school.com/"
            };

            builder.Services.AddCors(options =>
                options.AddDefaultPolicy(builder =>
                    builder.WithOrigins(urls)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()));


            //��ѫǥΪ�SignalR
            builder.Services.AddSignalR();
            //builder.Services.AddScoped<HomeService, HomeService>();
            //builder.Services.AddScoped<StoreNavbarService, StoreNavbarService>();
            //builder.Services.AddScoped<LayoutService, LayoutService>();


            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //swagger
            //app.UseSwagger();
            //app.UseSwaggerUI();




            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors();



            app.UseRouting();


            // ���� Cross-Origin-Opener-Policy
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Remove("Cross-Origin-Opener-Policy");
            //    await next();
            //});

            //ErorPage
            //�B�z ASP.NET Core �������~ - https://docs.microsoft.com/zh-tw/aspnet/core/fundamentals/error-handling?view=aspnetcore-6.0
            //�B�z ASP.NET Core web api �������~ - https://docs.microsoft.com/zh-tw/aspnet/core/web-api/handle-errors?view=aspnetcore-6.0

            //app.UseStatusCodePagesWithRedirects("~/Errors/Error404/{0}");

            //UseStatusCodePagesWithReExecute�襲����/�}�Y, ���঳~�Ÿ�
            app.UseStatusCodePagesWithReExecute("/Errors/ErrorPage", "?statuscode={0}");

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.MapRazorPages(); // ���U Razor Pages ����

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //�]�w��ѫǪ�hub
            app.MapHub<ChatHub>("/chatHub");

          

            app.Run();
        }
    }
}
