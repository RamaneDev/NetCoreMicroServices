using EventBus_Message.Common;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering_API.EventBusConsumer;
using Ordering_API.Extentions;
using Ordering_API.Mapping;
using Ordering_Application;
using Ordering_Infrastructure;
using Ordering_Infrastructure.Persistence;
using System;

namespace Ordering_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // MassTransit-RabbitMQ Configuration
            builder.Services.AddMassTransit(config => {

                config.AddConsumer<BasketCheckoutConsumer>();

                config.UsingRabbitMq((ctx, cfg) => {
                    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);              

                    cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c => {
                        c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
                    });
                });
            });

            builder.Services.AddScoped<BasketCheckoutConsumer>();
            builder.Services.AddAutoMapper(typeof(OrderingProfile).Assembly);



            var app = builder.Build();

            app.MigrateDatabase<OrderContext>((c, s) => { 
                
                var logger = s.GetService<ILogger<OrderContextSeed>>();
                OrderContextSeed.SeedAsync(c, logger).Wait();
            });

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