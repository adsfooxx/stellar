
using Autofac.Extensions.DependencyInjection;
using CloudinaryDotNet;

using Infrastructure;
using Infrastructure.Data.Mongo;
using Infrastructure.Data.Cloudnary;
using Infrastructure.Data.Mongo.Repository;
using Infrastructure.Services;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Stellar.API.Configurations;
using Stellar.API.Filters;
using Stellar.API.Settings;
using Autofac.Core;




//using Microsoft.Extensions.DependencyInjection;
namespace Stellar.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddInfrastructureService(builder.Configuration);

            builder.Services.AddAutoMapper(typeof(Program)); // ª`¤JAutoMapper
            
            
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
         

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Welcome to the Stellar.API",
                    Description = "",
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                        {
                            {
                               new OpenApiSecurityScheme
                                   {
                                        Reference = new OpenApiReference
                                                {
                                                   Type = ReferenceType.SecurityScheme,
                                                   Id = "Bearer"
                                                 }
                                    },
                                        new string[] {}
                            }
                });

                options.OperationFilter<SwaggerFileOperationFilter>();
            });



            builder.Services.AddHttpClient();

         

            builder.Services.AddWebApiService(builder.Configuration);


            var origins = new[] { "http://localhost:5173", "https://50s977n6-7168.asse.devtunnels.ms/", "https://stellarstellaradmin.azurewebsites.net" };
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
            { policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials(); }
            );
            });


            var app = builder.Build();

       

            // Configure the HTTP request pipeline.

            
            app.UseHttpsRedirection();

          
            app.UseRouting();


            app.UseSwagger();
            app.UseSwaggerUI();




            app.UseHttpsRedirection();



            app.UseCors();
          


            app.UseAuthentication();

 
            app.UseAuthorization();

            app.MapControllers();





            app.Run();
        }
    }
}
