using Discount_API.Data;
using Discount_API.Entities;
using Discount_API.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discount_API
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<DiscountContext>(o => o.UseNpgsql(builder.Configuration.GetValue<string>("DatabaseSettings:ConnectionString")));
            builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetService<ILogger<DiscountContext>>();

                try
                {
                    var context = services.GetRequiredService<DiscountContext>();
                    await context.Database.MigrateAsync();

                    if (!context.Coupons.Any())
                    {
                        Coupon c1 = new Coupon() { ProductName = "IPhone X", Description = "IPhone Discount", Amount = 150 };
                        Coupon c2 = new Coupon() { ProductName = "Samsung 10", Description = "Samsung Discount", Amount = 100 };

                        context.Coupons.Add(c1);
                        context.Coupons.Add(c2);

                        await context.SaveChangesAsync();
                    }

                   
                }
                catch (Exception ex){
                    logger.LogError(ex.Message);
;                }
            }

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