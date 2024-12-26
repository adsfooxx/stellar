using ApplicationCore.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Mongo.Repository;
using Infrastructure.Data.Mongo;
using Infrastructure.Service.MongoDB;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Data;
using Infrastructure.Data.Mail;
using Infrastructure.Data.MailKit;
using Infrastructure.Services.MailKit;
using CloudinaryDotNet;
using Infrastructure.Data.Cloudnary;
using Infrastructure.Services;
using ApplicationCore.LinePay.Dtos;
using Infrastructure.Data.Mongo.Entity;
using Infrastructure.Data.Linebot;
using Microsoft.SemanticKernel;
using Infrastructure.Services.Linebot.SemanticKernel;
using Infrastructure.Services.Linebot.LineMessage;
using Infrastructure.Services.Linebot.SemanticProductSearch;



namespace Infrastructure
{
    public static class Dependencies
    {
        public static void AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionStr = configuration.GetConnectionString("StellarDB");
            services.AddDbContext<StellarDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("StellarDB"));

            });
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionStr));
            services.AddScoped<IUnitOfWork, EfUnitOfWorkByServiceProvider>();


            services.Configure<LinePayApiOptions>(configuration.GetSection(LinePayApiOptions.LinePayApiOptionsKey));

            
            //Mongo
            services.Configure<MongoDbVectorSettings>(configuration.GetSection(nameof(MongoDbVectorSettings)))
            .AddSingleton(settings => settings.GetRequiredService<IOptions<MongoDbVectorSettings>>().Value);
            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<MongoDbVectorSettings>();
                return new MongoClient(settings.ConnectionString);
            });
            services.AddScoped<MongoRepository<ChatDto>>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var dbName = configuration["MongoDbVectorSettings:DatabaseName"];

                return new MongoRepository<ChatDto>(client, dbName);
            }); ;
            services.AddScoped<MongoRepository<Products>>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var dbName = configuration["MongoDbVectorSettings:DatabaseName"];

                return new MongoRepository<Products>(client, dbName);
            });
            services.AddScoped<MongoRepository<Messages>>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var dbName = configuration["MongoDbVectorSettings:DatabaseName"];

                return new MongoRepository<Messages>(client, dbName);
            });
            services.AddScoped<SemanticKernelSearchService>();

            //MailKit
            services.Configure<MailServerSettings>(configuration.GetSection(MailServerSettings.MailServerSettingsKey));
            services.Configure<VerifyEmailSettings>(configuration.GetSection(VerifyEmailSettings.VerifyEmailSettingsKey))
                .AddSingleton(provider => provider.GetRequiredService<IOptions<VerifyEmailSettings>>().Value);

            services.AddScoped<IEmailSenderService, MailKitEmailSenderService>();

            services
            .Configure<CloudinarySettings>(configuration.GetSection(nameof(CloudinarySettings)))
            .AddSingleton(settings => settings.GetRequiredService<IOptions<CloudinarySettings>>().Value);
            services.AddSingleton(sp =>
            {
                var cloudinarySettings = sp.GetRequiredService<CloudinarySettings>();
                return new Cloudinary(new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret));
            });
            
            services.AddScoped<CloudinaryService>();


            //Line bot 
            services.Configure<LineMessagingApiSettings>(configuration.GetSection(nameof(LineMessagingApiSettings)))
            .AddSingleton(settings => settings.GetRequiredService<IOptions<LineMessagingApiSettings>>().Value);

            #pragma warning disable SKEXP0020 // 類型僅供評估之用，可能會在未來更新中變更或移除。抑制此診斷以繼續。
            services.AddScoped<StellarChatServicePlugin>();
            services.AddScoped<SemanticProductSearchService>();
            services.AddScoped<ProductDetailGenerateService>();
            services.AddScoped<StellarChatService>(sp =>
            {
                var kernelBuilder = Kernel.CreateBuilder();
                kernelBuilder.Services.AddOpenAIChatCompletion("gpt-4o-mini-2024-07-18", configuration["OpenAIApiKey"]);
                var semanticProductSearchService = sp.GetRequiredService<SemanticProductSearchService>();
                var mongoDbService = sp.GetRequiredService<SemanticKernelSearchService>();
                kernelBuilder.Plugins.AddFromObject(new StellarChatServicePlugin(semanticProductSearchService, mongoDbService));
                return new StellarChatService(kernelBuilder.Build());
            });
            services.AddScoped<ChatSummarizationService>();
            services.AddScoped<MessageService>();


            //快取

            // Semantic Kernel Service
            services.AddSingleton<Kernel>(sp =>
            {
                var kernelBuilder = Kernel.CreateBuilder();
                kernelBuilder.Services.AddOpenAIChatCompletion("gpt-4o-mini-2024-07-18", configuration["OpenAIApiKey"]);
                return kernelBuilder.Build();
            });
        }
    }
}
