using Dicount_GRPC.Data;
using Dicount_GRPC.Mapping;
using Dicount_GRPC.Repositories;
using Dicount_GRPC.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dicount_GRPC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

            // Add services to the container.
            builder.Services.AddGrpc();
            builder.Services.AddDbContext<DiscountContext>(o => o.UseNpgsql(builder.Configuration.GetValue<string>("DatabaseSettings:ConnectionString")));
            builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
            builder.Services.AddAutoMapper(typeof(DiscountProfile).Assembly);
            builder.Services.AddGrpc();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<DiscountService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}