using Basket_API.GRPC_Services;
using Basket_API.Mapping;
using Basket_API.Repositories;
using Dicount_GRPC;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Basket_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetSection("CacheSettings:ConnectionString").Value;
            });

            // GRPC
            builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
               (o => o.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));
            builder.Services.AddScoped<IDiscountGrpcService, DiscountGrpcService>();

            // MassTransit-RabbitMQ Configuration
            builder.Services.AddMassTransit(config => {
                config.UsingRabbitMq((ctx, cfg) => {
                    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
                });
            });

            builder.Services.AddAutoMapper(typeof(BasketProfile).Assembly);

            builder.Services.AddScoped<IBasketRepository, BasketRepository>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}